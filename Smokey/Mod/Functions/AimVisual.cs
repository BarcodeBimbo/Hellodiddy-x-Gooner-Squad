using UnityEngine;

namespace Smokey.Mod.Functions
{
    public static class AimbotVisual
    {
        private static bool hasTarget = false;
        private static Material fovMaterial;
        public static float FovLineThickness = 0.5f;

        static AimbotVisual()
        {
            Shader shader = Shader.Find("UI/Default");
            if (shader == null)
            {
                Debug.LogError("UI/Default shader not found. FOV circle will not be drawn.");
                return;
            }
            fovMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            fovMaterial.SetInt("_Cull", 0);
            fovMaterial.SetInt("_ZWrite", 0);
            fovMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fovMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        }

        public static void SetLockState(bool locked)
        {
            hasTarget = locked;
        }

        public static void DrawFOV()
        {
            if (!Aimbot.DrawFovEnabled || !MainMod.IsMainScene || fovMaterial == null)
                return;

            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            float radius = Aimbot.FOV;
            Color fovColor = hasTarget ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.4f);
            DrawCircle(center, radius, fovColor);
        }

        private static void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            int segments = 128;
            for (float offset = -FovLineThickness / 2f; offset <= FovLineThickness / 2f; offset += 1f)
            {
                float currentRadius = radius + offset;
                GL.PushMatrix();
                fovMaterial.SetPass(0);
                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);
                for (int i = 0; i <= segments; i++)
                {
                    float angle = (2 * Mathf.PI * i) / segments;
                    float x = center.x + Mathf.Cos(angle) * currentRadius;
                    float y = center.y + Mathf.Sin(angle) * currentRadius;
                    GL.Vertex3(x, y, 0);
                }
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}
