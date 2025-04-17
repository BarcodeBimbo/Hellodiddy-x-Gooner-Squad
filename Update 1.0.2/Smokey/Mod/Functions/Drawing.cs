using UnityEngine;

namespace HelloDiddy.Mod.Functions
{
    public static class Drawing
    {
        private static Texture2D lineTex;

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            if (lineTex == null)
            {
                lineTex = new Texture2D(1, 1);
                lineTex.SetPixel(0, 0, Color.white);
                lineTex.Apply();
            }

            Matrix4x4 savedMatrix = GUI.matrix;
            Color savedColor = GUI.color;

            float angle = Vector3.Angle(pointB - pointA, Vector2.right);
            if (pointA.y > pointB.y) angle = -angle;
            float length = Vector2.Distance(pointA, pointB);

            GUI.color = color;
            GUIUtility.ScaleAroundPivot(new Vector2(length, width), pointA);
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }
    }
}
