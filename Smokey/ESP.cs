using System.Collections.Generic;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using UnityEngine;

namespace Smokey
{
    public class ESP
    {
        private readonly Dictionary<NPC, float> npcFade = new Dictionary<NPC, float>();
        private const float FadeSpeed = 2f;

        public void DrawESPPlayers(Il2CppSystem.Collections.Generic.List<Player> players)
        {
            Camera mainCamera = Camera.main;
            if (players == null || players.Count == 0 || mainCamera == null)
                return;

            foreach (Player player in players)
            {
                if (player == null || player.transform == null || player.IsLocalPlayer)
                    continue;

                float distance = Vector3.Distance(mainCamera.transform.position, player.transform.position);
                if (distance > Features.MaxESPDistance)
                    continue;

                Vector3 playerPos = player.transform.position;
                Vector3 headPos = playerPos + Vector3.up * 1.8f;

                Vector3 screenHead = mainCamera.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCamera.WorldToScreenPoint(playerPos);

                if (screenHead.z > 0 && screenFeet.z > 0)
                {
                    if (Features.Use3DBoxes)
                    {
                        Draw3DBox(headPos, playerPos, Features.PlayerBoxColor);
                    }
                    else
                    {
                        Draw2DBox(screenHead, screenFeet, Features.PlayerBoxColor);
                    }

                    float centerX = (screenHead.x + screenFeet.x) / 2f;

                    if (Features.ShowNames)
                        DrawLabel(new Vector2(centerX, Screen.height - screenHead.y - 15), player.name, Color.white);

                    if (Features.ShowDistance)
                        DrawLabel(new Vector2(centerX, Screen.height - screenFeet.y + 5), $"{distance:F1}m", Color.cyan);
                }
            }
        }

        public void DrawESPNPC(Il2CppSystem.Collections.Generic.List<NPC> npcs)
        {
            Camera mainCamera = Camera.main;
            if (npcs == null || npcs.Count == 0 || mainCamera == null)
                return;

            foreach (NPC npc in npcs)
            {
                if (npc == null || npc.transform == null || npc.isInBuilding)
                    continue;

                float targetAlpha = 1f;
                bool isValid = npc.isActiveAndEnabled && npc.Health != null && npc.IsConscious;

                if (!npcFade.ContainsKey(npc))
                    npcFade.Add(npc, isValid ? 1f : 0f);

                npcFade[npc] = Mathf.MoveTowards(npcFade[npc], isValid ? 1f : 0f, Time.deltaTime * FadeSpeed);

                if (npcFade[npc] <= 0.01f)
                    continue;

                float alpha = npcFade[npc];

                float distance = Vector3.Distance(mainCamera.transform.position, npc.transform.position);
                if (distance > Features.MaxESPDistance)
                    continue;

                Vector3 npcPos = npc.transform.position;
                Vector3 headPos = npcPos + Vector3.up * 1.8f;

                Vector3 screenHead = mainCamera.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCamera.WorldToScreenPoint(npcPos);

                Color boxColor = npc.name.Contains("Officer") ? Features.CopBoxColor : Features.NPCBoxColor;
                boxColor.a = alpha;

                if (screenHead.z > 0 && screenFeet.z > 0)
                {
                    if (Features.Use3DBoxes)
                    {
                        Draw3DBox(headPos, npcPos, boxColor);
                    }
                    else
                    {
                        Draw2DBox(screenHead, screenFeet, boxColor);
                    }

                    float centerX = (screenHead.x + screenFeet.x) / 2f;

                    if (Features.ShowNames)
                        DrawLabel(new Vector2(centerX, Screen.height - screenHead.y - 15), npc.name, new Color(1f, 1f, 1f, alpha));

                    if (Features.ShowDistance)
                        DrawLabel(new Vector2(centerX, Screen.height - screenFeet.y + 5), $"{distance:F1}m", new Color(0f, 1f, 1f, alpha));
                }
            }
        }

        private void Draw2DBox(Vector3 head, Vector3 feet, Color color)
        {
            Vector2 top = new Vector2(head.x, Screen.height - head.y);
            Vector2 bottom = new Vector2(feet.x, Screen.height - feet.y);

            float height = Mathf.Abs(top.y - bottom.y);
            float width = height / 2;

            Vector2 center = new Vector2(bottom.x, (top.y + bottom.y) / 2);

            GUI.color = color;
            Texture2D texture = Texture2D.whiteTexture;

            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, width, 1), texture);
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y + height / 2, width, 1), texture);
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, 1, height), texture);
            GUI.DrawTexture(new Rect(center.x + width / 2, center.y - height / 2, 1, height), texture);
        }

        private void Draw3DBox(Vector3 head, Vector3 feet, Color color)
        {
            Vector3 center = (head + feet) / 2f;
            float height = Vector3.Distance(head, feet);
            float width = height / 2f;
            float depth = width / 2f;

            Vector3[] corners = new Vector3[8];
            corners[0] = center + new Vector3(-width, height / 2, -depth);
            corners[1] = center + new Vector3(width, height / 2, -depth);
            corners[2] = center + new Vector3(width, height / 2, depth);
            corners[3] = center + new Vector3(-width, height / 2, depth);
            corners[4] = center + new Vector3(-width, -height / 2, -depth);
            corners[5] = center + new Vector3(width, -height / 2, -depth);
            corners[6] = center + new Vector3(width, -height / 2, depth);
            corners[7] = center + new Vector3(-width, -height / 2, depth);

            DrawLine(corners[0], corners[1], color);
            DrawLine(corners[1], corners[2], color);
            DrawLine(corners[2], corners[3], color);
            DrawLine(corners[3], corners[0], color);

            DrawLine(corners[4], corners[5], color);
            DrawLine(corners[5], corners[6], color);
            DrawLine(corners[6], corners[7], color);
            DrawLine(corners[7], corners[4], color);

            DrawLine(corners[0], corners[4], color);
            DrawLine(corners[1], corners[5], color);
            DrawLine(corners[2], corners[6], color);
            DrawLine(corners[3], corners[7], color);
        }

        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Vector3 startScreen = Camera.main.WorldToScreenPoint(start);
            Vector3 endScreen = Camera.main.WorldToScreenPoint(end);

            if (startScreen.z > 0 && endScreen.z > 0)
            {
                startScreen.y = Screen.height - startScreen.y;
                endScreen.y = Screen.height - endScreen.y;

                Drawing.DrawLine(new Vector2(startScreen.x, startScreen.y), new Vector2(endScreen.x, endScreen.y), color, 1f);
            }
        }

        private void DrawLabel(Vector2 center, string text, Color color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = color;
            style.fontSize = 10;

            Vector2 size = style.CalcSize(new GUIContent(text));
            Rect labelRect = new Rect(center.x - size.x / 2, center.y, size.x, size.y);

            GUI.Label(labelRect, text, style);
        }
    }
}
