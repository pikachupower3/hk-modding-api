using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MonoMod;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable all
#pragma warning disable 1591, 649, 414, 169, CS0108, CS0626

namespace Modding.Patches
{
    [MonoModPatch("global::GameManager")]
    public class GameManager : global::GameManager
    {
        public extern void orig_OnApplicationQuit();

        public void OnApplicationQuit()
        {
            orig_OnApplicationQuit();
            ModHooks.OnApplicationQuit();
        }

        public extern void orig_LoadScene(string destScene);

        public void LoadScene(string destScene)
        {
            destScene = ModHooks.BeforeSceneLoad(destScene);

            orig_LoadScene(destScene);

            ModHooks.OnSceneChanged(destScene);
        }

        public extern void orig_BeginSceneTransition(GameManager.SceneLoadInfo info);

        public void BeginSceneTransition(GameManager.SceneLoadInfo info)
        {
            info.SceneName = ModHooks.BeforeSceneLoad(info.SceneName);

            orig_BeginSceneTransition(info);
        }

        public extern void orig_ClearSaveFile(int saveSlot, Action<bool> callback);

        public void ClearSaveFile(int saveSlot, Action<bool> callback)
        {
            ModHooks.OnSavegameClear(saveSlot);
            orig_ClearSaveFile(saveSlot, callback);
            ModHooks.OnAfterSaveGameClear(saveSlot);
        }

        public extern IEnumerator orig_PlayerDead(float waitTime);

        public IEnumerator PlayerDead(float waitTime)
        {
            ModHooks.OnBeforePlayerDead();
            yield return orig_PlayerDead(waitTime);
            ModHooks.OnAfterPlayerDead();
        }

        #region SaveGame

        private ModSavegameData moddedData;

        [MonoModIgnore]
        private GameCameras gameCams;

        [MonoModIgnore]
        private float sessionPlayTimer;

        [MonoModIgnore]
        private extern void ResetGameTimer();

        private static string ModdedSavePath(int slot) => Path.Combine(
            Application.persistentDataPath,
            $"user{slot}.modded.json"
        );

        private UIManager _uiInstance;

        public UIManager ui
        {
            get
            {
                if (_uiInstance == null) _uiInstance = (UIManager)UIManager.instance;
                return _uiInstance;
            }
            private set => _uiInstance = value;
        }

        [MonoModReplace]
        public void SaveGame(int saveSlot)
        {
            if (saveSlot >= 0)
            {
                if (this.gameCams.saveIcon != null)
                {
                    this.gameCams.saveIcon.SendEvent("GAME SAVED");
                }
                else
                {
                    GameObject gameObject = GameObject.FindGameObjectWithTag("Save Icon");
                    if (gameObject != null)
                    {
                        PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Checkpoint Control");
                        if (playMakerFSM != null)
                        {
                            playMakerFSM.SendEvent("GAME SAVED");
                        }
                    }
                }
                this.SaveLevelState();
                if (!this.gameConfig.disableSaveGame)
                {
                    if (this.achievementHandler != null)
                    {
                        this.achievementHandler.FlushRecordsToDisk();
                    }
                    else
                    {
                        Debug.LogError("Error saving achievements (PlayerAchievements is null)");
                    }
                    if (this.playerData != null)
                    {
                        this.playerData.SetFloat(nameof(PlayerData.playTime), this.playerData.GetFloat(nameof(PlayerData.playTime)) + this.sessionPlayTimer);
                        this.ResetGameTimer();
                        this.playerData.SetString(nameof(PlayerData.version), Constants.GAME_VERSION);
                        this.playerData.SetInt(nameof(PlayerData.profileID), saveSlot);
                        this.playerData.CountGameCompletion();
                    }
                    else
                    {
                        Debug.LogError("Error updating PlayerData before save (PlayerData is null)");
                    }
                    try
                    {
                        SaveGameData saveGameData = new SaveGameData(this.playerData, this.sceneData);
                        ModHooks.OnBeforeSaveGameSave(saveGameData);
                        if (moddedData == null)
                        {
                            moddedData = new ModSavegameData();
                        }
                        ModHooks.OnSaveLocalSettings(moddedData);
                        try
                        {
                            string path = GameManager.ModdedSavePath(saveSlot);
                            string modded = JsonConvert.SerializeObject(
                                this.moddedData,
                                Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = ShouldSerializeContractResolver.Instance,
                                    TypeNameHandling = TypeNameHandling.Auto,
                                    Converters = JsonConverterTypes.ConverterTypes
                                }
                            );
                            if (File.Exists(path + ".bak")) File.Delete(path + ".bak");
                            if (File.Exists(path)) File.Move(path, path + ".bak");
                            using FileStream fileStream = File.Create(path);
                            using var writer = new StreamWriter(fileStream);
                            writer.Write(modded);
                        } 
                        catch (Exception e)
                        {
                            Logger.APILogger.LogError(e);
                        }
                        string text = null;

                        try
                        {
                            text = JsonConvert.SerializeObject(saveGameData, Formatting.Indented, new JsonSerializerSettings()
                            {
                                ContractResolver = ShouldSerializeContractResolver.Instance,
                                TypeNameHandling = TypeNameHandling.Auto,
                                Converters = JsonConverterTypes.ConverterTypes
                            });
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Failed to serialize save using Json.NET, trying fallback.");

                            Logger.APILogger.LogError(e);

                            // If this dies, not much we can do about it.
                            text = JsonUtility.ToJson(saveGameData);
                        }
                        bool flag = this.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected;
                        if (flag)
                        {
                            string graph = Encryption.Encrypt(text);
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            MemoryStream memoryStream = new MemoryStream();
                            binaryFormatter.Serialize(memoryStream, graph);
                            Platform.Current.WriteSaveSlot(saveSlot, memoryStream.ToArray());
                            memoryStream.Close();
                        }
                        else
                        {
                            Platform.Current.WriteSaveSlot(saveSlot, Encoding.UTF8.GetBytes(text));
                        }
                    }
                    catch (Exception arg)
                    {
                        Debug.LogError("GM Save - There was an error saving the game: " + arg);
                    }
                    ModHooks.OnSavegameSave(saveSlot);
                }
                else
                {
                    Debug.Log("Saving game disabled. No save file written.");
                }
            }
            else
            {
                Debug.LogError("Save game slot not valid: " + saveSlot);
            }
        }

        #endregion

        public extern void orig_SetupSceneRefs(bool refreshTilemapInfo);

        public void SetupSceneRefs(bool refreshTilemapInfo)
        {
            orig_SetupSceneRefs(refreshTilemapInfo);


            if (IsGameplayScene())
            {
                GameObject go = GameCameras.instance.soulOrbFSM.gameObject.transform.Find("SoulOrb_fill").gameObject;
                GameObject liquid = go.transform.Find("Liquid").gameObject;
                tk2dSpriteAnimator tk2dsa = liquid.GetComponent<tk2dSpriteAnimator>();
                tk2dsa.GetClipByName("Fill").fps = 15 * 1.05f;
                tk2dsa.GetClipByName("Idle").fps = 10 * 1.05f;
                tk2dsa.GetClipByName("Shrink").fps = 15 * 1.05f;
                tk2dsa.GetClipByName("Drain").fps = 30 * 1.05f;
            }

        }

        #region LoadGame

        [MonoModReplace]
        public bool LoadGame(int saveSlot)
        {
            if (!Platform.IsSaveSlotIndexValid(saveSlot))
            {
                Debug.LogErrorFormat("Cannot load from invalid save slot index {0}", new object[]
                {
                    saveSlot
                });
                return false;
            }
            if (!Platform.Current.IsSaveSlotInUse(saveSlot))
            {
                Debug.LogErrorFormat("Cannot load from empty save slot index {0}", new object[]
                {
                    saveSlot
                });
                return false;
            }
            try
            {
                var path = ModdedSavePath(saveSlot);
                if (File.Exists(path))
                {
                    using FileStream fileStream = File.OpenRead(path);
                    using var reader = new StreamReader(fileStream);
                    string json = reader.ReadToEnd();
                    moddedData = JsonConvert.DeserializeObject<ModSavegameData>(
                        json,
                        new JsonSerializerSettings()
                        {
                            ContractResolver = ShouldSerializeContractResolver.Instance,
                            TypeNameHandling = TypeNameHandling.Auto,
                            ObjectCreationHandling = ObjectCreationHandling.Replace,
                            Converters = JsonConverterTypes.ConverterTypes
                        }
                    );
                    if (moddedData == null)
                    {
                        Logger.APILogger.LogError($"Loaded mod savegame data deserialized to null: {json}");
                        this.moddedData = new ModSavegameData();
                    }
                }
                else
                {
                    moddedData = new ModSavegameData();
                }
            }
            catch (Exception e)
            {
                Logger.APILogger.LogError(e);
                moddedData = new ModSavegameData();
            }
            ModHooks.OnLoadLocalSettings(moddedData);

            bool result;
            try
            {
                bool flag = this.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected;
                string json;
                if (flag)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MemoryStream serializationStream = new MemoryStream(Platform.Current.ReadSaveSlot(saveSlot));
                    string encryptedString = (string)binaryFormatter.Deserialize(serializationStream);
                    json = Encryption.Decrypt(encryptedString);
                }
                else
                {
                    json = Encoding.UTF8.GetString(Platform.Current.ReadSaveSlot(saveSlot));
                }
                SaveGameData saveGameData;

                try
                {
                    saveGameData = JsonConvert.DeserializeObject<SaveGameData>(json, new JsonSerializerSettings()
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = JsonConverterTypes.ConverterTypes
                    });
                }
                catch (Exception e)
                {
                    Logger.APILogger.LogError("Failed to read save using Json.NET (GameManager::LoadGame), falling back.");
                    Logger.APILogger.LogError(e);

                    saveGameData = JsonUtility.FromJson<SaveGameData>(json);
                }
                global::PlayerData instance = saveGameData.playerData;
                SceneData instance2 = saveGameData.sceneData;
                global::PlayerData.instance = instance;
                this.playerData = instance;
                SceneData.instance = instance2;
                ModHooks.OnAfterSaveGameLoad(saveGameData);
                this.sceneData = instance2;
                this.profileID = saveSlot;
                this.inputHandler.RefreshPlayerData();
                ModHooks.OnSavegameLoad(saveSlot);
                result = true;
            }
            catch (Exception ex)
            {
                Debug.LogFormat("Error loading save file for slot {0}: {1}", new object[]
                {
                    saveSlot,
                    ex
                });
                result = false;
            }
            return result;
        }

        #endregion

        #region GetSaveStatsForSlot

        [MonoModReplace]
        public SaveStats GetSaveStatsForSlot(int saveSlot)
        {
            if (!Platform.IsSaveSlotIndexValid(saveSlot))
            {
                UnityEngine.Debug.LogErrorFormat("Cannot get save stats for invalid slot {0}", new object[]
                {
                saveSlot
                });
                return null;
            }
            if (!Platform.Current.IsSaveSlotInUse(saveSlot))
            {
                return null;
            }
            SaveStats result;
            try
            {
                bool flag = this.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected;
                string json;
                if (flag)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MemoryStream serializationStream = new MemoryStream(Platform.Current.ReadSaveSlot(saveSlot));
                    string encryptedString = (string)binaryFormatter.Deserialize(serializationStream);
                    json = Encryption.Decrypt(encryptedString);
                }
                else
                {
                    json = Encoding.UTF8.GetString(Platform.Current.ReadSaveSlot(saveSlot));
                }
                SaveGameData saveGameData;
                try
                {
                    saveGameData = JsonConvert.DeserializeObject<SaveGameData>(json, new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = JsonConverterTypes.ConverterTypes
                    });
                }
                catch (Exception)
                {
                    Modding.Logger.APILogger.LogWarn(string.Format("Failed to get save stats for slot {0} using Json.NET, falling back", saveSlot));
                    saveGameData = JsonUtility.FromJson<SaveGameData>(json);
                }
                global::PlayerData playerData = saveGameData.playerData;
                SaveStats saveStats = new SaveStats(playerData.maxHealthBase, playerData.geo, playerData.mapZone, playerData.playTime, playerData.MPReserveMax, playerData.permadeathMode, playerData.completionPercentage, playerData.unlockedCompletionRate);
                result = new SaveStats
                        (
                            playerData.GetInt(nameof(PlayerData.maxHealthBase)),
                            playerData.GetInt(nameof(PlayerData.geo)),
                            ((PlayerData)playerData).GetVariable<GlobalEnums.MapZone>(nameof(PlayerData.mapZone)),
                            playerData.GetFloat(nameof(PlayerData.playTime)),
                            playerData.GetInt(nameof(PlayerData.MPReserveMax)),
                            playerData.GetInt(nameof(PlayerData.permadeathMode)),
                            playerData.GetFloat(nameof(PlayerData.completionPercentage)),
                            playerData.GetBool(nameof(PlayerData.unlockedCompletionRate))
                        );
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Concat(new object[]
                {
                    "Error while loading save file for slot ",
                    saveSlot,
                    " Exception: ",
                    ex
                }));
                result = null;
            }
            return result;
        }

        #endregion

        #region LoadSceneAdditive

        [MonoModIgnore]
        private bool tilemapDirty;

        [MonoModIgnore]
        private bool waitForManualLevelStart;

        [MonoModIgnore]
        public event GameManager.DestroyPooledObjects DestroyPersonalPools;

        [MonoModIgnore]
        public event GameManager.UnloadLevel UnloadingLevel;

        [MonoModReplace]
        public IEnumerator LoadSceneAdditive(string destScene)
        {
            Debug.Log("Loading " + destScene);
            destScene = ModHooks.BeforeSceneLoad(destScene);
            this.tilemapDirty = true;
            this.startedOnThisScene = false;
            this.nextSceneName = destScene;
            this.waitForManualLevelStart = true;
            if (this.DestroyPersonalPools != null)
            {
                this.DestroyPersonalPools();
            }

            if (this.UnloadingLevel != null)
            {
                this.UnloadingLevel();
            }

            string exitingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            AsyncOperation loadop = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
            loadop.allowSceneActivation = true;
            yield return loadop;
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(exitingScene);
            ModHooks.OnSceneChanged(destScene);
            this.RefreshTilemapInfo(destScene);
            if (this.IsUnloadAssetsRequired(exitingScene, destScene))
            {
                Debug.LogFormat(this, "Unloading assets due to zone transition", new object[0]);
                yield return Resources.UnloadUnusedAssets();
            }

            GCManager.Collect();
            this.SetupSceneRefs(true);
            this.BeginScene();
            this.OnNextLevelReady();
            this.waitForManualLevelStart = false;
            Debug.Log("Done Loading " + destScene);
            yield break;
        }

        #endregion

        #region LoadFirstScene

        [MonoModReplace]
        public IEnumerator LoadFirstScene()
        {
            yield return new WaitForEndOfFrame();
            this.OnWillActivateFirstLevel();
            this.LoadScene("Tutorial_01");
            ModHooks.OnNewGame();
            yield break;
        }

        #endregion

        #region OnWillActivateFirstLevel

        public extern void orig_OnWillActivateFirstLevel();

        public void OnWillActivateFirstLevel()
        {
            orig_OnWillActivateFirstLevel();
            ModHooks.OnNewGame();
        }

        #endregion

        #region PauseToDynamicMenu
        [MonoModIgnore]
        public extern void SetTimeScale(float timescale);
        [MonoModIgnore]
        private bool timeSlowed;

        // code has been copied from PauseGameToggle
        public IEnumerator PauseToggleDynamicMenu(MenuScreen screen, bool allowUnpause = false)
        {
            if (!this.timeSlowed)
            {
                if (!this.playerData.GetBool(nameof(PlayerData.disablePause)) && this.gameState == GlobalEnums.GameState.PLAYING)
                {
                    this.gameCams.StopCameraShake();
                    this.inputHandler.PreventPause();
                    this.inputHandler.StopUIInput();
                    this.actorSnapshotPaused.TransitionTo(0f);
                    this.isPaused = true;
                    this.SetState(GlobalEnums.GameState.PAUSED);
                    this.ui.AudioGoToPauseMenu(0.2f);
                    this.ui.UIPauseToDynamicMenu(screen);
                    if (HeroController.instance != null)
                    {
                        HeroController.instance.Pause();
                    }
                    this.gameCams.MoveMenuToHUDCamera();
                    this.SetTimeScale(0f);
                    yield return new WaitForSecondsRealtime(0.8f);
                    this.inputHandler.AllowPause();
                }
                else if (allowUnpause && this.gameState == GlobalEnums.GameState.PAUSED)
                {
                    this.gameCams.ResumeCameraShake();
                    this.inputHandler.PreventPause();
                    this.actorSnapshotUnpaused.TransitionTo(0f);
                    this.isPaused = false;
                    this.ui.AudioGoToGameplay(0.2f);
                    this.ui.SetState( GlobalEnums.UIState.PLAYING);
                    this.SetState( GlobalEnums.GameState.PLAYING);
                    if (HeroController.instance != null)
                    {
                        HeroController.instance.UnPause();
                    }
                    MenuButtonList.ClearAllLastSelected();
                    this.SetTimeScale(1f);
                    yield return new WaitForSecondsRealtime(0.8f);
                    this.inputHandler.AllowPause();
                }
            }
            yield break;
        }
        #endregion

        [MonoModIgnore]
        private SceneLoad sceneLoad;

        /*
         * This will allow modders to access the scene loader.
         * Note that if there's no transition in progress, it will be null!
         * Example use case: Start a co-routine that checks for an non null
         * sceneLoad then hooks up a callback to the "Finish" delegate to do something when the game has completed loading a scene.
         */
        [MonoModIgnore]
        public SceneLoad SceneLoad
        {
            get { return sceneLoad; }
        }
    }
}