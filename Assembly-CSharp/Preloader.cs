using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UObject = UnityEngine.Object;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Modding.Utils;

namespace Modding;

internal class Preloader : MonoBehaviour
{
    private const int CanvasResolutionWidth = 1920;
    private const int CanvasResolutionHeight = 1080;
    private const int LoadingBarBackgroundWidth = 1000;
    private const int LoadingBarBackgroundHeight = 100;
    private const int LoadingBarMargin = 12;
    private const int LoadingBarWidth = LoadingBarBackgroundWidth - 2 * LoadingBarMargin;
    private const int LoadingBarHeight = LoadingBarBackgroundHeight - 2 * LoadingBarMargin;

    private GameObject _blanker;
    private GameObject _loadingBarBackground;
    private GameObject _loadingBar;
    private RectTransform _loadingBarRect;

    private float _commandedProgress;
    private float _shownProgress;
    private float _secondsSinceLastSet;

    public IEnumerator Preload
    (
        Dictionary<string, List<(ModLoader.ModInstance, List<string>)>> toPreload,
        Dictionary<string, List<(ModLoader.ModInstance, List<string>)>> preloadPrefabs,
        Dictionary<ModLoader.ModInstance, Dictionary<string, string>> preloadSceneNameMap,
        Dictionary<ModLoader.ModInstance, Dictionary<string, Dictionary<string, GameObject>>> preloadedObjects
    )
    {
        MuteAllAudio();

        CreateBlanker();

        CreateLoadingBarBackground();

        CreateLoadingBar();

        yield return DoPreload(toPreload, preloadPrefabs, preloadSceneNameMap, preloadedObjects);

        yield return CleanUpPreloading();

        UnmuteAllAudio();
    }

    public void Update()
    {
        _secondsSinceLastSet += Time.unscaledDeltaTime;
        _shownProgress = Mathf.Lerp(_shownProgress, _commandedProgress, _secondsSinceLastSet / 10.0f);
    }

    public void LateUpdate()
    {
        _loadingBarRect.sizeDelta = new Vector2(
            _shownProgress * LoadingBarWidth,
            _loadingBarRect.sizeDelta.y
        );
    }

    /// <summary>
    ///     Mutes all audio from AudioListeners.
    /// </summary>
    private static void MuteAllAudio() => AudioListener.pause = true;

    /// <summary>
    ///     Creates the canvas used to show the loading progress.
    ///     It is centered on the screen.
    /// </summary>
    private void CreateBlanker()
    {
        _blanker = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(CanvasResolutionWidth, CanvasResolutionHeight));
        
        DontDestroyOnLoad(_blanker);

        GameObject panel = CanvasUtil.CreateImagePanel
        (
            _blanker,
            CanvasUtil.NullSprite(new byte[] { 0x00, 0x00, 0x00, 0xFF }),
            new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
        );

        panel
            .GetComponent<Image>()
            .preserveAspect = false;
    }

    /// <summary>
    ///     Creates the background of the loading bar.
    ///     It is centered in the canvas.
    /// </summary>
    private void CreateLoadingBarBackground()
    {
        _loadingBarBackground = CanvasUtil.CreateImagePanel
        (
            _blanker,
            CanvasUtil.NullSprite(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }),
            new CanvasUtil.RectData
            (
                new Vector2(LoadingBarBackgroundWidth, LoadingBarBackgroundHeight),
                Vector2.zero,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f)
            )
        );
        
        _loadingBarBackground.GetComponent<Image>().preserveAspect = false;
    }

    /// <summary>
    ///     Creates the loading bar with an initial width of 0.
    ///     It is centered in the canvas.
    /// </summary>
    private void CreateLoadingBar()
    {
        _loadingBar = CanvasUtil.CreateImagePanel
        (
            _blanker,
            CanvasUtil.NullSprite(new byte[] { 0x99, 0x99, 0x99, 0xFF }),
            new CanvasUtil.RectData
            (
                new Vector2(0, LoadingBarHeight),
                Vector2.zero,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f)
            )
        );
        
        _loadingBar.GetComponent<Image>().preserveAspect = false;
        _loadingBarRect = _loadingBar.GetComponent<RectTransform>();
    }

    /// <summary>
    ///     Updates the progress of the loading bar to the given progress.
    /// </summary>
    /// <param name="progress">The progress that should be displayed. 0.0f - 1.0f</param>
    private void UpdateLoadingBarProgress(float progress)
    {
        if (Mathf.Abs(_commandedProgress - progress) < float.Epsilon) 
            return;
        
        _commandedProgress = progress;
        _secondsSinceLastSet = 0.0f;
    }

    /// <summary>
    ///     This is the actual preloading process.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoPreload
    (
        Dictionary<string, List<(ModLoader.ModInstance, List<string>)>> toPreload,
        IReadOnlyDictionary<string, List<(ModLoader.ModInstance, List<string>)>> preloadPrefabs,
        IReadOnlyDictionary<ModLoader.ModInstance, Dictionary<string, string>> preloadSceneNameMap,
        IDictionary<ModLoader.ModInstance, Dictionary<string, Dictionary<string, GameObject>>> preloadedObjects
    )
    {
        List<string> sceneNames = toPreload.Keys.ToList();
        Dictionary<string, int> scenePriority = new();
        Dictionary<string, (AsyncOperation load, AsyncOperation unload)> sceneAsyncOperationHolder = new();
        
        sceneNames.AddRange(preloadPrefabs.Select(kvp => kvp.Key));
        
        foreach (string sceneName in sceneNames)
        {
            int priority = 0;
            
            if (toPreload.TryGetValue(sceneName, out var preloadObjs0)) 
                priority += preloadObjs0.Select(x => x.Item2.Count).Sum();
            
            if (preloadPrefabs.TryGetValue(sceneName, out var preloadObjs1)) 
                priority += preloadObjs1.Select(x => x.Item2.Count).Sum();
            
            scenePriority[sceneName] = priority;
            sceneAsyncOperationHolder[sceneName] = (null, null);
        }

        Dictionary<string, GameObject> GetModScenePreloadedObjects(ModLoader.ModInstance mod, string sceneName)
        {
            if (!preloadedObjects.TryGetValue
            (
                mod,
                out Dictionary<string, Dictionary<string, GameObject>> modPreloadedObjects
            ))
            {
                preloadedObjects[mod] = modPreloadedObjects = new Dictionary<string, Dictionary<string, GameObject>>();
            }
            
            // ReSharper disable once InvertIf
            if (!modPreloadedObjects.TryGetValue
            (
                sceneName,
                out Dictionary<string, GameObject> modScenePreloadedObjects
            ))
            {
                modPreloadedObjects[sceneName] = modScenePreloadedObjects = new Dictionary<string, GameObject>();
            }
            
            return modScenePreloadedObjects;
        }

        var preloadOperationQueue = new List<AsyncOperation>(5);

        void GetPreloadObjectsOperation(string sceneName)
        {
            Scene scene = USceneManager.GetSceneByName(sceneName);
            
            GameObject[] rootObjects = scene.GetRootGameObjects();
            
            foreach (var go in rootObjects)
                go.SetActive(false);
            
            if (preloadPrefabs.TryGetValue(sceneName, out var sharedObjects))
            {
                List<string> allObjects = sharedObjects.SelectMany(x => x.Item2).ToList();
                Dictionary<string, GameObject> objectsMap = new();
                
                foreach (
                    var obj in Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(x => !x.scene.IsValid())
                    .Where(x => x.transform.parent == null)
                    .Where(x => allObjects.Contains(x.name))
                )
                {
                    objectsMap[obj.name] = obj;
                    allObjects.Remove(obj.name);
                }
                
                foreach ((ModLoader.ModInstance mod, List<string> objNames) in sharedObjects)
                {
                    Logger.APILogger.LogFine($"Fetching prefab for mod \"{mod.Mod.GetName()}\"");
                    
                    string sn = preloadSceneNameMap[mod][sceneName];
                    
                    var modScenePreloadedObjects = GetModScenePreloadedObjects(mod, sn);
                    
                    foreach (string objName in objNames)
                    {
                        Logger.APILogger.LogFine($"Fetching prefab \"{objName}\"");

                        if (!objectsMap.TryGetValue(objName, out GameObject obj))
                        {
                            Logger.APILogger.LogWarn(
                                $"Could not find prefab \"{objName}\" in scene \"{scene}\"," + $" requested by mod `{mod.Mod.GetName()}`"
                            );
                            continue;
                        }
                        
                        modScenePreloadedObjects[objName] = obj;
                    }
                }
            }

            if (!toPreload.TryGetValue(sceneName, out var sceneObjects)) 
                return;
            
            // Fetch object names to preload
            foreach ((ModLoader.ModInstance mod, List<string> objNames) in sceneObjects)
            {
                Logger.APILogger.LogFine($"Fetching objects for mod \"{mod.Mod.GetName()}\"");

                Dictionary<string, GameObject> scenePreloads = GetModScenePreloadedObjects(mod, sceneName);

                foreach (string objName in objNames)
                {
                    Logger.APILogger.LogFine($"Fetching object \"{objName}\"");

                    GameObject obj;

                    try
                    {
                        obj = UnityExtensions.GetGameObjectFromArray(rootObjects, objName);
                    }
                    catch (ArgumentException)
                    {
                        Logger.APILogger.LogWarn($"Invalid GameObject name {objName}");
                        continue;
                    }

                    if (obj == null)
                    {
                        Logger.APILogger.LogWarn(
                            $"Could not find object \"{objName}\" in scene \"{sceneName}\"," + $" requested by mod `{mod.Mod.GetName()}`"
                        );
                        continue;
                    }

                    // Create inactive duplicate of requested object
                    obj = Instantiate(obj);
                    DontDestroyOnLoad(obj);
                    obj.SetActive(false);

                    // Set object to be passed to mod
                    scenePreloads[objName] = obj;
                }
            }
        }

        void CleanupPreloadOperation(string sceneName)
        {
            Logger.APILogger.LogFine($"Unloading scene \"{sceneName}\"");
            
            AsyncOperation unloadOp = USceneManager.UnloadSceneAsync(sceneName);
            
            sceneAsyncOperationHolder[sceneName] = (sceneAsyncOperationHolder[sceneName].load, unloadOp);
            
            unloadOp.completed += _ => preloadOperationQueue.Remove(unloadOp);
            
            preloadOperationQueue.Add(unloadOp);
        }

        void StartPreloadOperation(string sceneName)
        {
            Logger.APILogger.LogFine($"Loading scene \"{sceneName}\"");
            
            AsyncOperation loadOp = USceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            sceneAsyncOperationHolder[sceneName] = (loadOp, null);

            loadOp.priority = scenePriority[sceneName];
            loadOp.completed += _ =>
            {
                preloadOperationQueue.Remove(loadOp);
                GetPreloadObjectsOperation(sceneName);
                CleanupPreloadOperation(sceneName);
            };
            
            preloadOperationQueue.Add(loadOp);
        }

        int i = 0;
        
        float sceneProgressAverage = 0;
        
        while (sceneProgressAverage < 1.0f)
        {
            while (
                preloadOperationQueue.Count < ModHooks.GlobalSettings.PreloadBatchSize &&
                i < sceneNames.Count &&
                sceneProgressAverage < 1.0f
            )
            {
                StartPreloadOperation(sceneNames[i++]);
            }
            
            yield return null;
            
            sceneProgressAverage = sceneAsyncOperationHolder
                                   .Values
                                   .Select(x => (x.load?.progress ?? 0) * 0.5f + (x.unload?.progress ?? 0) * 0.5f)
                                   .Average();
            
            UpdateLoadingBarProgress(sceneProgressAverage);
        }

        if (!preloadPrefabs.TryGetValue("resources", out var prefabs))
        {
            UpdateLoadingBarProgress(1f);
            yield break;
        }

        List<string> allObjects = prefabs.SelectMany(x => x.Item2).ToList();
        Dictionary<string, GameObject> objectsMap = new();

        foreach (
            var obj in Resources.LoadAll<GameObject>("")
                                .Where(x => !x.scene.IsValid())
                                .Where(x => x.transform.parent == null)
                                .Where(x => allObjects.Contains(x.name))
        )
        {
            objectsMap[obj.name] = obj;
            allObjects.Remove(obj.name);
        }

        foreach ((ModLoader.ModInstance mod, List<string> objNames) in prefabs)
        {
            Logger.APILogger.LogFine($"Fetching prefabs for mod \"{mod.Mod.GetName()}\"");

            var preloads = GetModScenePreloadedObjects(mod, "resources");

            foreach (string objName in objNames)
            {
                Logger.APILogger.LogFine($"Fetching prefab \"{objName}\"");

                if (!objectsMap.TryGetValue(objName, out GameObject obj))
                {
                    Logger.APILogger.LogWarn($"Could not find prefab \"{objName}\" in \"resources.assets\", requested by mod `{mod.Mod.GetName()}`");

                    continue;
                }

                preloads[objName] = obj;
            }
        }

        UpdateLoadingBarProgress(1.0f);
    }

    /// <summary>
    ///     Clean up everything from preloading.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CleanUpPreloading()
    {
        // Reload the main menu to fix the music/shaders
        Logger.APILogger.LogDebug("Preload done, returning to main menu");

        ModLoader.LoadState |= ModLoader.ModLoadState.Preloaded;

        yield return USceneManager.LoadSceneAsync("Quit_To_Menu");

        while (USceneManager.GetActiveScene().name != Constants.MENU_SCENE)
        {
            yield return new WaitForEndOfFrame();
        }

        // Remove the black screen
        Destroy(_loadingBar);
        Destroy(_loadingBarBackground);
        Destroy(_blanker);
    }

    /// <summary>
    ///     Unmutes all audio from AudioListeners.
    /// </summary>
    private static void UnmuteAllAudio()
    {
        AudioListener.pause = false;
    }
}
