using UnityEngine;

namespace Smokey.Mod.UI
{
    public static class UI
    {
        // A header style for consistent titles.
        public static GUIStyle HeaderStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 14;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                return style;
            }
        }

        public static void Label(string text)
        {
            GUILayout.Label(text);
        }

        public static void Label(string text, GUIStyle style)
        {
            GUILayout.Label(text, style);
        }

        public static bool Button(string text)
        {
            return GUILayout.Button(text);
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, options);
        }

        public static bool Toggle(bool value, string text)
        {
            return GUILayout.Toggle(value, text);
        }

        public static float Slider(float value, float min, float max)
        {
            return GUILayout.HorizontalSlider(value, min, max);
        }

        public static void BeginVertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }

        public static void EndVertical()
        {
            GUILayout.EndVertical();
        }

        public static void BeginHorizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }

        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }

        public static void DragWindow()
        {
            GUI.DragWindow();
        }

        public static Color ColorField(Color color)
        {
            BeginHorizontal();
            color.r = Slider(color.r, 0f, 1f);
            color.g = Slider(color.g, 0f, 1f);
            color.b = Slider(color.b, 0f, 1f);
            EndHorizontal();
            return color;
        }
    }
}
