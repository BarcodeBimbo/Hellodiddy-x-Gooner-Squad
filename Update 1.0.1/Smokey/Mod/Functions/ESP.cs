using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using System.Collections.Generic;
using UnityEngine;

namespace Smokey.Mod.Functions
{
    public class ESP
    {
        private readonly Dictionary<NPC, float> npcFade = new Dictionary<NPC, float>();
        private const float FadeSpeed = 2f;

        public void DrawESPPlayers(Il2CppSystem.Collections.Generic.List<Player> players)
        {
            Camera mainCam = Camera.main;
            if (players == null || players.Count == 0 || mainCam == null)
                return;

            foreach (Player player in players)
            {
                if (player == null || player.transform == null || player.IsLocalPlayer)
                    continue;

                float distance = Vector3.Distance(mainCam.transform.position, player.transform.position);
                if (distance > Features.Features.MaxESPDistance)
                    continue;

                // Project head and feet onto screen
                Vector3 headPos = player.transform.position + Vector3.up * 1.8f;
                Vector3 feetPos = player.transform.position;
                Vector3 screenHead = mainCam.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCam.WorldToScreenPoint(feetPos);

                // Only render if in front of camera
                if (screenHead.z <= 0 || screenFeet.z <= 0)
                    continue;

                // Box color is user-customizable; name/distances remain white
                Color boxColor = Features.Features.PlayerBoxColor;
                if (boxColor.a == 0f)
                    boxColor = Color.red; // default

                // Draw the box (2D or 3D)
                if (Features.Features.Use3DBoxes)
                    Draw3DBox(headPos, feetPos, boxColor);
                else
                    Draw2DBox(screenHead, screenFeet, boxColor);

                // Calculate top of box in screen coords
                Vector2 top2D = new Vector2(screenHead.x, Screen.height - screenHead.y);
                Vector2 bottom2D = new Vector2(screenFeet.x, Screen.height - screenFeet.y);
                float boxHeight = Mathf.Abs(top2D.y - bottom2D.y);
                float boxWidth = boxHeight / 2f;
                float centerX = (top2D.x + bottom2D.x) / 2f;

                // Offsets (above the box top)
                float healthBarAboveBox = 4f;  // Health bar is 4px above top of box
                float nameAboveBox = 8f;       // Name is 8px above top of box
                float barHeight = 4f;

                // 1) Health bar:
                //    Bottom edge of bar is "healthBarAboveBox" px above box top
                //    so bar's top edge is (bottom - barHeight)
                float barBottomY = top2D.y - healthBarAboveBox;   // bottom of bar
                float barTopY = barBottomY - barHeight;           // top of bar
                Vector2 barPos = new Vector2(centerX - (boxWidth / 2f), barTopY);

                // 2) Name label:
                //    Its bottom edge is exactly "nameAboveBox" above box top
                //    We'll measure the label to place it correctly.
                GUIStyle nameStyle = MakeLabelStyle(Color.white);
                Vector2 nameSize = nameStyle.CalcSize(new GUIContent(player.name));
                float nameBottomY = top2D.y - nameAboveBox;      // bottom of name label
                float nameTopY = nameBottomY - nameSize.y;       // top of name label
                float nameLeftX = centerX - (nameSize.x / 2f);

                // Draw health bar if we have player health
                if (player.Health != null)
                {
                    float currentHP = player.Health.CurrentHealth;
                    float hpPercent = currentHP / 100f; // assume 100 max
                    GUI.color = Color.red;
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth, barHeight), Texture2D.whiteTexture);
                    GUI.color = Color.green;
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth * hpPercent, barHeight), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                // Draw name label
                if (Features.Features.ShowNames)
                {
                    Rect nameRect = new Rect(nameLeftX, nameTopY, nameSize.x, nameSize.y);
                    GUI.Label(nameRect, player.name, nameStyle);
                }

                // Draw distance label at bottom center of box
                if (Features.Features.ShowDistance)
                {
                    GUIStyle distStyle = MakeLabelStyle(Color.white);
                    string distText = $"{distance:F1}m";
                    Vector2 distSize = distStyle.CalcSize(new GUIContent(distText));
                    float distLeftX = centerX - (distSize.x / 2f);
                    float distTopY = (Screen.height - screenFeet.y); // same as bottom2D.y
                    Rect distRect = new Rect(distLeftX, distTopY, distSize.x, distSize.y);
                    GUI.Label(distRect, distText, distStyle);
                }
            }
        }

        public void DrawESPNPC(Il2CppSystem.Collections.Generic.List<NPC> npcs)
        {
            Camera mainCam = Camera.main;
            if (npcs == null || npcs.Count == 0 || mainCam == null)
                return;

            // remove stale npc references
            List<NPC> keysToRemove = new List<NPC>();
            foreach (var kv in npcFade)
                if (!npcs.Contains(kv.Key))
                    keysToRemove.Add(kv.Key);
            foreach (var gone in keysToRemove)
                npcFade.Remove(gone);

            foreach (NPC npc in npcs)
            {
                if (npc == null || npc.transform == null || npc.isInBuilding)
                    continue;

                // fade logic
                bool isValid = npc.isActiveAndEnabled && npc.Health != null && npc.IsConscious;
                if (!npcFade.ContainsKey(npc))
                    npcFade.Add(npc, isValid ? 1f : 0f);
                npcFade[npc] = Mathf.MoveTowards(npcFade[npc], isValid ? 1f : 0f, Time.deltaTime * FadeSpeed);
                float alpha = npcFade[npc];
                if (alpha <= 0.01f)
                    continue;

                float distance = Vector3.Distance(mainCam.transform.position, npc.transform.position);
                if (distance > Features.Features.MaxESPDistance)
                    continue;

                Vector3 headPos = npc.transform.position + Vector3.up * 1.8f;
                Vector3 screenHead = mainCam.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCam.WorldToScreenPoint(npc.transform.position);
                if (screenHead.z <= 0 || screenFeet.z <= 0)
                    continue;

                // Only the box color is user-customizable
                Color boxColor = npc.name.Contains("Officer") ? Features.Features.CopBoxColor : Features.Features.NPCBoxColor;
                if (boxColor.a == 0f)
                    boxColor = npc.name.Contains("Officer") ? Color.blue : Color.yellow;
                boxColor.a = alpha;

                // Draw 2D or 3D box
                if (Features.Features.Use3DBoxes)
                    Draw3DBox(npc.transform.position + Vector3.up * 1.8f, npc.transform.position, boxColor);
                else
                    Draw2DBox(screenHead, screenFeet, boxColor);

                // 2D box boundaries
                Vector2 top2D = new Vector2(screenHead.x, Screen.height - screenHead.y);
                Vector2 bottom2D = new Vector2(screenFeet.x, Screen.height - screenFeet.y);
                float boxHeight = Mathf.Abs(top2D.y - bottom2D.y);
                float boxWidth = boxHeight / 2f;
                float centerX = (top2D.x + bottom2D.x) / 2f;

                float healthBarAboveBox = 4f;
                float barHeight = 4f;
                float nameAboveBox = 8f;

                // bottom & top of health bar
                float barBottomY = top2D.y - healthBarAboveBox;
                float barTopY = barBottomY - barHeight;
                Vector2 barPos = new Vector2(centerX - (boxWidth / 2f), barTopY);

                // name label: bottom is 8px above box
                // measure label to place it properly
                GUIStyle nameStyle = MakeLabelStyle(Color.white);
                Vector2 nameSize = nameStyle.CalcSize(new GUIContent(npc.name));
                float nameBottomY = top2D.y - nameAboveBox;
                float nameTopY = nameBottomY - nameSize.y;
                float nameLeftX = centerX - (nameSize.x / 2f);

                // NPC name label
                if (Features.Features.ShowNames)
                {
                    Rect nameRect = new Rect(nameLeftX, nameTopY, nameSize.x, nameSize.y);
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                    GUI.Label(nameRect, npc.name, nameStyle);
                    GUI.color = Color.white;
                }

                // NPC Health bar
                if (npc.Health != null)
                {
                    float maxHp = npc.Health.MaxHealth;
                    float curHp = npc.Health.Health;
                    float hpPerc = curHp / maxHp;
                    GUI.color = new Color(1f, 0f, 0f, alpha);
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth, barHeight), Texture2D.whiteTexture);
                    GUI.color = new Color(0f, 1f, 0f, alpha);
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth * hpPerc, barHeight), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                // Distance label
                if (Features.Features.ShowDistance)
                {
                    string distText = $"{distance:F1}m";
                    GUIStyle distStyle = MakeLabelStyle(Color.white);
                    Vector2 distSize = distStyle.CalcSize(new GUIContent(distText));
                    float distLeftX = centerX - (distSize.x / 2f);
                    float distTopY = bottom2D.y;
                    Rect distRect = new Rect(distLeftX, distTopY, distSize.x, distSize.y);
                    GUI.Label(distRect, distText, distStyle);
                }
            }
        }

        /// <summary>
        /// Draws a simple 2D bounding box from head to feet.
        /// </summary>
        private void Draw2DBox(Vector3 head, Vector3 feet, Color color)
        {
            Vector2 top = new Vector2(head.x, Screen.height - head.y);
            Vector2 bottom = new Vector2(feet.x, Screen.height - feet.y);
            float height = Mathf.Abs(top.y - bottom.y);
            float width = height / 2;
            Vector2 center = new Vector2(bottom.x, (top.y + bottom.y) / 2);

            GUI.color = color;
            Texture2D tex = Texture2D.whiteTexture;

            // top horizontal
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, width, 1), tex);
            // bottom horizontal
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y + height / 2, width, 1), tex);
            // left vertical
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, 1, height), tex);
            // right vertical
            GUI.DrawTexture(new Rect(center.x + width / 2, center.y - height / 2, 1, height), tex);
        }

        /// <summary>
        /// Draws a simple 3D bounding box (cube) around the entity.
        /// </summary>
        private void Draw3DBox(Vector3 head, Vector3 feet, Color color)
        {
            Vector3 center = (head + feet) / 2f;
            float height = Vector3.Distance(head, feet);
            float width = height / 4f;
            float depth = width / 2f;

            Vector3[] c = new Vector3[8];
            c[0] = center + new Vector3(-width, height / 2, -depth);
            c[1] = center + new Vector3(width, height / 2, -depth);
            c[2] = center + new Vector3(width, height / 2, depth);
            c[3] = center + new Vector3(-width, height / 2, depth);
            c[4] = center + new Vector3(-width, -height / 2, -depth);
            c[5] = center + new Vector3(width, -height / 2, -depth);
            c[6] = center + new Vector3(width, -height / 2, depth);
            c[7] = center + new Vector3(-width, -height / 2, depth);

            DrawLine(c[0], c[1], color);
            DrawLine(c[1], c[2], color);
            DrawLine(c[2], c[3], color);
            DrawLine(c[3], c[0], color);
            DrawLine(c[4], c[5], color);
            DrawLine(c[5], c[6], color);
            DrawLine(c[6], c[7], color);
            DrawLine(c[7], c[4], color);
            DrawLine(c[0], c[4], color);
            DrawLine(c[1], c[5], color);
            DrawLine(c[2], c[6], color);
            DrawLine(c[3], c[7], color);
        }

        /// <summary>
        /// Basic line drawing for 3D boxes, converting to screen coords.
        /// </summary>
        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Camera mainCam = Camera.main;
            if (!mainCam) return;

            Vector3 startS = mainCam.WorldToScreenPoint(start);
            Vector3 endS = mainCam.WorldToScreenPoint(end);
            if (startS.z <= 0 || endS.z <= 0) return;

            startS.y = Screen.height - startS.y;
            endS.y = Screen.height - endS.y;
            Drawing.DrawLine(new Vector2(startS.x, startS.y), new Vector2(endS.x, endS.y), color, 1f);
        }

        /// <summary>
        /// Constructs a GUIStyle for labels with the specified color, always 10pt & centered.
        /// </summary>
        private GUIStyle MakeLabelStyle(Color color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10
            };
            style.normal.textColor = color;
            return style;
        }
    }
}
