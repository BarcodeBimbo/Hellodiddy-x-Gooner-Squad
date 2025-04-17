using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace HelloDiddy.Mod.UI
{
    public static class UI
    {
        #region Styles

        public static GUIStyle HeaderStyle => new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.HSVToRGB(0.97f, 0.2f, 1f) }
        };

        public static GUIStyle SubHeaderStyle => new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        public static GUIStyle ThinScrollStyle => new GUIStyle(GUI.skin.verticalScrollbar)
        {
            fixedWidth = 8,
            margin = new RectOffset(0, 0, 2, 2),
            padding = new RectOffset(0, 0, 2, 2)
        };

        #endregion

        #region Layout Helpers

        public static void Label(string text) => GUILayout.Label(text);
        public static void Label(string text, GUIStyle style) => GUILayout.Label(text, style);
        public static bool Button(string text) => GUILayout.Button(text);
        public static bool Button(string text, params GUILayoutOption[] options) => GUILayout.Button(text, options);
        public static bool Toggle(bool value, string text) => GUILayout.Toggle(value, text);
        public static float Slider(float value, float min, float max, params GUILayoutOption[] options) => GUILayout.HorizontalSlider(value, min, max, options);

        public static void BeginVertical(params GUILayoutOption[] options) => GUILayout.BeginVertical(options);
        public static void EndVertical() => GUILayout.EndVertical();
        public static void BeginHorizontal(params GUILayoutOption[] options) => GUILayout.BeginHorizontal(options);
        public static void EndHorizontal() => GUILayout.EndHorizontal();
        public static void DragWindow() => GUI.DragWindow();

        #endregion

        #region Background

        public static Texture2D LoadTexture(string resourcePath)
        {
            try
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(File.ReadAllBytes(resourcePath));
                return texture;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load texture: {ex}");
                return null;
            }
        }

        public static void LoadBackground()
        {
            if (MainModState.Instance.backgroundLoaded) return;

            string path = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "GoonerSquad", "HelloDiddy.png");
            if (File.Exists(path))
            {
                var texture = LoadTexture(path);
                if (texture != null)
                {
                    MainModState.Instance.backgroundTexture = texture;
                    MainModState.Instance.backgroundLoaded = true;
                    MelonLogger.Msg("Loaded background successfully.");
                }
            }
            else
            {
                MelonLogger.Warning($"Background not found at: {path}");
            }
        }

        public static void CreateBackgroundCanvas()
        {
            if (MainModState.Instance.canvasObject != null) return;
            MainModState.Instance.canvasObject = new GameObject("ModBackgroundCanvas");
            var canvas = MainModState.Instance.canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            var scaler = MainModState.Instance.canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            MainModState.Instance.canvasObject.AddComponent<GraphicRaycaster>();
            MainModState.Instance.canvasGroup = MainModState.Instance.canvasObject.AddComponent<CanvasGroup>();
            MainModState.Instance.canvasGroup.blocksRaycasts = false;
            MainModState.Instance.canvasGroup.interactable = false;
            MainModState.Instance.canvasGroup.alpha = 0f;
            var imageObj = new GameObject("BackgroundImage");
            imageObj.transform.SetParent(MainModState.Instance.canvasObject.transform, false);
            MainModState.Instance.backgroundImage = imageObj.AddComponent<Image>();

            if (MainModState.Instance.backgroundTexture != null)
            {
                var sprite = Sprite.Create(
                    MainModState.Instance.backgroundTexture,
                    new Rect(0, 0, MainModState.Instance.backgroundTexture.width, MainModState.Instance.backgroundTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
                MainModState.Instance.backgroundImage.sprite = sprite;
            }
            MainModState.Instance.backgroundImage.raycastTarget = false;

            var rectTransform = MainModState.Instance.backgroundImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Settings.windowRect.width, Settings.windowRect.height);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }


        #endregion

        #region Color Picker

        public static void DrawColorPicker(string label, ref Color color)
        {
            GUILayout.Label(label, SubHeaderStyle);
            color.r = SliderWithLabel("R", color.r);
            color.g = SliderWithLabel("G", color.g);
            color.b = SliderWithLabel("B", color.b);
        }

        private static float SliderWithLabel(string channel, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(channel, GUILayout.Width(20));
            value = Slider(value, 0f, 1f);
            GUILayout.Label(Mathf.RoundToInt(value * 255).ToString(), GUILayout.Width(40));
            GUILayout.EndHorizontal();
            return value;
        }

        #endregion

        #region Config-Aware Toggles & Sliders
        private static float lastSliderChangeTime = 0f;
        private static float saveDelay = 0.5f;
        private static bool needsSave = false;

        public static void CreateNotifyToggle(string label, Func<bool> getter, Action<bool> setter, string on_msg = "Enabled", string off_msg = "Disabled")
        {
            bool current = getter();
            bool toggled = Toggle(current, label);

            if (toggled != current)
            {
                setter(toggled);
                Config.Config.Save();

                Sprite icon = NotificationUtils.LoadNotificationSprite();
                NotificationUtils.CreateNotification(label, toggled ? on_msg : off_msg, icon, 5f, true);
            }
        }

		public static void CreateConfigToggle(string label, Func<bool> getter, Action<bool> setter)
		{
			bool current = getter();
			bool toggled = Toggle(current, label); 

			if (toggled != current)
			{
				Config.Config.UpdateAndSave(current, toggled, setter, label); // This saves & applies WIP
				Sprite icon = NotificationUtils.LoadNotificationSprite();
				NotificationUtils.CreateNotification(label, toggled ? "Enabled" : "Disabled", icon, 5f, true);
			}
		}

		public static void NotifySlider(string label, int min, int max, Func<int> getter, Action<int> setter)
        {
            int current = getter();
            int newValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(current, min, max));
            GUILayout.Label($"{label}: {newValue}");

            if (newValue != current)
            {
                setter(newValue);
                lastSliderChangeTime = Time.realtimeSinceStartup;
                needsSave = true;
            }

            if (needsSave && Time.realtimeSinceStartup - lastSliderChangeTime > saveDelay)
            {
                Config.Config.Save();
                needsSave = false;
            }
        }

        public static void NotifySlider(string label, float min, float max, Func<float> getter, Action<float> setter)
        {
            float current = getter();
            float newValue = GUILayout.HorizontalSlider(current, min, max);
            GUILayout.Label($"{label}: {newValue:F1}");

            if (Mathf.Abs(newValue - current) > 0.01f)
            {
                setter(newValue);
                lastSliderChangeTime = Time.realtimeSinceStartup;
                needsSave = true;
            }

            if (needsSave && Time.realtimeSinceStartup - lastSliderChangeTime > saveDelay)
            {
                Config.Config.Save();
                needsSave = false;
            }
        }

        #endregion
    }
}
