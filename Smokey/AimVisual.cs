using UnityEngine;

namespace Smokey
{
    public static class AimbotVisual
    {
        private static bool hasTarget = false;

        public static void SetLockState(bool locked)
        {
            hasTarget = locked;
        }

        public static void DrawFOV()
        {
            if (!Aimbot.DrawFovEnabled || !MainMod.IsMainScene)
                return;

            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            float radius = Aimbot.FOV; // Adjust FOV circle size

            Color fovColor = hasTarget ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.4f); // Green locked, White idle

            DrawCircle(center, radius, fovColor);
        }

        private static void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            int segments = 64;
            float angleStep = 360f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float nextAngle = (i + 1) * angleStep * Mathf.Deg2Rad;

                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Vector2 nextPoint = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;

                GL.Vertex3(point.x, point.y, 0);
                GL.Vertex3(nextPoint.x, nextPoint.y, 0);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
