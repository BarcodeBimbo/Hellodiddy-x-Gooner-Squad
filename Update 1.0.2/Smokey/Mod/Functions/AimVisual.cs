using UnityEngine;

namespace HelloDiddy.Mod.Functions
{
    public static class AimbotVisual
    {
        static AimbotVisual()
        {
            Shader shader = Shader.Find("UI/Default");
            if (shader == null)
            {
                Debug.LogError("UI/Default shader not found. FOV circle will not be drawn.");
                return;
            }

            MainModState.fovMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            MainModState.fovMaterial.SetInt("_Cull", 0);
            MainModState.fovMaterial.SetInt("_ZWrite", 0);
            MainModState.fovMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            MainModState.fovMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        }

        public static void SetLockState(bool locked)
        {
            MainModState.hasTarget = locked;
        }

        public static void DrawFOV()
        {
            if (!MainModState.DrawFovEnabled || !MainModState.IsMainScene || MainModState.fovMaterial == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            float radius = MainModState.FOV;

            Color fovColor = MainModState.hasTarget
                ? new Color(0f, 1f, 0f, 0.5f)
                : new Color(1f, 1f, 1f, 0.4f);

            DrawCircle(center, radius, fovColor);
            DrawCrosshair(center, MainModState.hasTarget ? Color.green : Color.white);
        }

        private static void DrawCircle(Vector2 center, float radius, Color color)
        {
            int segments = 128;
            float lineThickness = MainModState.FovLineThickness;

            for (float offset = -lineThickness / 2f; offset <= lineThickness / 2f; offset += 1f)
            {
                float currentRadius = radius + offset;

                GL.PushMatrix();
                MainModState.fovMaterial.SetPass(0);
                GL.LoadPixelMatrix();
                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);

                for (int i = 0; i <= segments; i++)
                {
                    float angle = (2 * Mathf.PI * i) / segments;
                    float x = center.x + Mathf.Cos(angle) * currentRadius;
                    float y = Screen.height - (center.y + Mathf.Sin(angle) * currentRadius);
                    GL.Vertex3(x, y, 0);
                }

                GL.End();
                GL.PopMatrix();
            }
        }

        private static void DrawCrosshair(Vector2 center, Color color)
        {
            float size = 6f;

            GL.PushMatrix();
            MainModState.fovMaterial.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(center.x - size, Screen.height - center.y, 0);
            GL.Vertex3(center.x + size, Screen.height - center.y, 0);
            GL.Vertex3(center.x, Screen.height - (center.y - size), 0);
            GL.Vertex3(center.x, Screen.height - (center.y + size), 0);

            GL.End();
            GL.PopMatrix();
        }
    }
}
