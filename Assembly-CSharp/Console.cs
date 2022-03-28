using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Modding
{
    internal class Console : MonoBehaviour
    {
        private static GameObject _overlayCanvas;
        private static GameObject _textPanel;
        private static Font _font;
        private List<string> _messages;
        private KeyCode _toggleKey;
        private int _maxMessageCount;
        private int _fontSize;
        private bool _enabled = true;

        private const int MSG_LENGTH = 80;

        private static readonly string[] OSFonts =
        {
            // Windows
            "Consolas",
            // Mac
            "Menlo",
            // Linux
            "Courier New",
            "DejaVu Mono"
        };

        [PublicAPI]
        public void Start()
        {
            _toggleKey = ModHooks.GlobalSettings.ConsoleSettings.ToggleHotkey;
            if (_toggleKey == KeyCode.Escape) {
                Logger.APILogger.LogError($"Esc cannot be used as hotkey for console togging");
                _toggleKey = ModHooks.GlobalSettings.ConsoleSettings.ToggleHotkey = KeyCode.F10;
            }

            _maxMessageCount = ModHooks.GlobalSettings.ConsoleSettings.MaxMessageCount;
            if (_maxMessageCount <= 0)
            {
                Logger.APILogger.LogError($"Specified max console message count {_maxMessageCount} is invalid");
                _maxMessageCount = ModHooks.GlobalSettings.ConsoleSettings.MaxMessageCount = 24;
            }
            _messages = new List<string>(_maxMessageCount + 1);

            _fontSize = ModHooks.GlobalSettings.ConsoleSettings.FontSize;
            if (_fontSize <= 0)
            {
                Logger.APILogger.LogError($"Specified console font size {_fontSize} is invalid");
                _fontSize = ModHooks.GlobalSettings.ConsoleSettings.FontSize = 12;
            }


            string userFont = ModHooks.GlobalSettings.ConsoleSettings.Font;
            if (!string.IsNullOrEmpty(userFont))
            {
                _font = Font.CreateDynamicFontFromOSFont(userFont, _fontSize);

                if (_font == null)
                    Logger.APILogger.LogError($"Specified font {userFont} not found.");
            }

            if (_font == null)
            {
                foreach (string font in OSFonts)
                {
                    _font = Font.CreateDynamicFontFromOSFont(font, _fontSize);

                    // Found a monospace OS font.
                    if (_font != null)
                        break;

                    Logger.APILogger.Log($"Font {font} not found.");
                }
            }

            // Fallback
            if (_font == null)
            {
                _font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            }

            DontDestroyOnLoad(gameObject);

            if (_overlayCanvas != null)
            {
                return;
            }

            _overlayCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            _overlayCanvas.name = "ModdingApiConsoleLog";
            CanvasGroup cg = _overlayCanvas.GetComponent<CanvasGroup>();
            cg.interactable = false;
            cg.blocksRaycasts = false;
            DontDestroyOnLoad(_overlayCanvas);

            GameObject background = CanvasUtil.CreateImagePanel
            (
                _overlayCanvas,
                CanvasUtil.NullSprite(new byte[] {0x80, 0x00, 0x00, 0x00}),
                new CanvasUtil.RectData(new Vector2(500, 800), Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero)
            );

            _textPanel = CanvasUtil.CreateTextPanel
            (
                background,
                string.Join(string.Empty, _messages.ToArray()),
                12,
                TextAnchor.LowerLeft,
                new CanvasUtil.RectData(new Vector2(-5, -5), Vector2.zero, Vector2.zero, Vector2.one),
                _font
            );

            _textPanel.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
        }

        [PublicAPI]
        public void Update()
        {
            if (!Input.GetKeyDown(_toggleKey))
            {
                return;
            }

            StartCoroutine
            (
                _enabled
                    ? CanvasUtil.FadeOutCanvasGroup(_overlayCanvas.GetComponent<CanvasGroup>())
                    : CanvasUtil.FadeInCanvasGroup(_overlayCanvas.GetComponent<CanvasGroup>())
            );

            _enabled = !_enabled;
        }

        public void AddText(string message, LogLevel level)
        {
            IEnumerable<string> chunks = Chunks(message, MSG_LENGTH);

            string color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.DefaultColor}>";

            if (ModHooks.GlobalSettings.ConsoleSettings.UseLogColors)
            {
                switch (level)
                {
                    case LogLevel.Fine:
                        color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.FineColor}>";
                        break;
                    case LogLevel.Info:
                        color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.InfoColor}>";
                        break;
                    case LogLevel.Debug:
                        color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.DebugColor}>";
                        break;
                    case LogLevel.Warn:
                        color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.WarningColor}>";
                        break;
                    case LogLevel.Error:
                        color = $"<color={ModHooks.GlobalSettings.ConsoleSettings.ErrorColor}>";
                        break;
                }
            }

            foreach (string s in chunks)
                _messages.Add(color + s + "</color>");

            while (_messages.Count > _maxMessageCount)
            {
                _messages.RemoveAt(0);
            }

            if (_textPanel != null)
            {
                _textPanel.GetComponent<Text>().text = string.Join(string.Empty, _messages.ToArray());
            }
        }

        private static IEnumerable<string> Chunks(string str, int maxChunkSize) 
        {
            for (int i = 0; i < str.Length; i += maxChunkSize) 
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    }
}