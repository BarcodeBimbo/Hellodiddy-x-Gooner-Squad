using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using System.Collections.Generic;
using UnityEngine;

namespace HelloDiddy.Mod.Functions
{
    public class ESP
    {
        private readonly Dictionary<NPC, float> npcFade = new Dictionary<NPC, float>();
        private const float FadeSpeed = 2f;

        private static readonly string[] BonePaths =
{
            "Armature/mixamorig:Hips",
            "Armature/mixamorig:Hips/mixamorig:Spine",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head",
            "Armature/mixamorig:Hips/mixamorig:LeftUpLeg",
            "Armature/mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg",
            "Armature/mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot",
            "Armature/mixamorig:Hips/mixamorig:RightUpLeg",
            "Armature/mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg",
            "Armature/mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm",
            "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"
        };

        public static void DrawBonesESP(Transform bodyContainer, Color color)
        {
            if (bodyContainer == null || Camera.main == null)
                return;

            Camera cam = Camera.main;
            Dictionary<string, Vector2> boneScreenPositions = new Dictionary<string, Vector2>();

            foreach (string path in BonePaths)
            {
                Transform bone = bodyContainer.Find(path);
                if (bone == null) continue;

                Vector3 screenPos = cam.WorldToScreenPoint(bone.position);
                if (screenPos.z > 0)
                {
                    boneScreenPositions[path] = new Vector2(screenPos.x, Screen.height - screenPos.y);
                }
            }
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips", "Armature/mixamorig:Hips/mixamorig:Spine");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine", "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1", "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2", "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck", "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips", "Armature/mixamorig:LeftUpLeg");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:LeftUpLeg", "Armature/mixamorig:LeftLeg");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:LeftLeg", "Armature/mixamorig:LeftFoot");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips", "Armature/mixamorig:RightUpLeg");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:RightUpLeg", "Armature/mixamorig:RightLeg");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:RightLeg", "Armature/mixamorig:RightFoot");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck", "Armature/mixamorig:LeftShoulder");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:LeftShoulder", "Armature/mixamorig:LeftArm");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:LeftArm", "Armature/mixamorig:LeftForeArm");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:LeftForeArm", "Armature/mixamorig:LeftHand");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck", "Armature/mixamorig:RightShoulder");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:RightShoulder", "Armature/mixamorig:RightArm");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:RightArm", "Armature/mixamorig:RightForeArm");
            TryDrawBoneLine(boneScreenPositions, "Armature/mixamorig:RightForeArm", "Armature/mixamorig:RightHand");
        }

        private static void TryDrawBoneLine(Dictionary<string, Vector2> bones, string boneA, string boneB)
        {
            if (bones.TryGetValue(boneA, out Vector2 a) && bones.TryGetValue(boneB, out Vector2 b))
            {
                Drawing.DrawLine(a, b, Color.red, 1f);
            }
        }

        public static void DrawDistance(Vector3 position, float distance, Color color)
        {
            if (Camera.main == null) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
            if (screenPos.z <= 0) return;

            Vector2 pos2D = new Vector2(screenPos.x, Screen.height - screenPos.y);
            GUIStyle style = new GUIStyle()
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 10,
                normal = { textColor = color }
            };
            string distText = $"{distance:F1}m";
            Vector2 size = style.CalcSize(new GUIContent(distText));
            GUI.Label(new Rect(pos2D.x - size.x / 2, pos2D.y, size.x, size.y), distText, style);
        }   

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
                if (distance > MainModState.MaxESPDistance)
                    continue;
                Vector3 headPos = player.transform.position + Vector3.up * 1.8f;
                Vector3 feetPos = player.transform.position;
                Vector3 screenHead = mainCam.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCam.WorldToScreenPoint(feetPos);
                if (screenHead.z <= 0 || screenFeet.z <= 0)
                    continue;
                Color boxColor = MainModState.PlayerBoxColor;
                if (boxColor.a == 0f)
                    boxColor = Color.red;
                if (MainModState.Use3DBoxes)
                    Draw3DBox(headPos, feetPos, boxColor);
                else
                    Draw2DBox(screenHead, screenFeet, boxColor);
                Vector2 top2D = new Vector2(screenHead.x, Screen.height - screenHead.y);
                Vector2 bottom2D = new Vector2(screenFeet.x, Screen.height - screenFeet.y);
                float boxHeight = Mathf.Abs(top2D.y - bottom2D.y);
                float boxWidth = boxHeight / 2f;
                float centerX = (top2D.x + bottom2D.x) / 2f;
                float healthBarAboveBox = 4f;
                float nameAboveBox = 8f;
                float barHeight = 4f;
                float barBottomY = top2D.y - healthBarAboveBox;
                float barTopY = barBottomY - barHeight;
                Vector2 barPos = new Vector2(centerX - (boxWidth / 2f), barTopY);
                GUIStyle nameStyle = MakeLabelStyle(Color.white);
                Vector2 nameSize = nameStyle.CalcSize(new GUIContent(player.name));
                float nameBottomY = top2D.y - nameAboveBox;
                float nameTopY = nameBottomY - nameSize.y;
                float nameLeftX = centerX - (nameSize.x / 2f);
                if (player.Health != null)
                {
                    float currentHP = player.Health.CurrentHealth;
                    float hpPercent = currentHP / 100f;
                    GUI.color = Color.red;
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth, barHeight), Texture2D.whiteTexture);
                    GUI.color = Color.green;
                    GUI.DrawTexture(new Rect(barPos.x, barPos.y, boxWidth * hpPercent, barHeight), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }
                if (MainModState.ShowNames)
                {
                    Rect nameRect = new Rect(nameLeftX, nameTopY, nameSize.x, nameSize.y);
                    GUI.Label(nameRect, player.name, nameStyle);
                }
                if (MainModState.ShowDistance)
                {
                    GUIStyle distStyle = MakeLabelStyle(Color.white);
                    string distText = $"{distance:F1}m";
                    Vector2 distSize = distStyle.CalcSize(new GUIContent(distText));
                    float distLeftX = centerX - (distSize.x / 2f);
                    float distTopY = (Screen.height - screenFeet.y);
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
                bool isValid = npc.isActiveAndEnabled && npc.Health != null && npc.IsConscious;
                if (!npcFade.ContainsKey(npc))
                    npcFade.Add(npc, isValid ? 1f : 0f);
                npcFade[npc] = Mathf.MoveTowards(npcFade[npc], isValid ? 1f : 0f, Time.deltaTime * FadeSpeed);
                float alpha = npcFade[npc];
                if (alpha <= 0.01f)
                    continue;

                float distance = Vector3.Distance(mainCam.transform.position, npc.transform.position);
                if (distance > MainModState.MaxESPDistance)
                    continue;

                Vector3 headPos = npc.transform.position + Vector3.up * 1.8f;
                Vector3 screenHead = mainCam.WorldToScreenPoint(headPos);
                Vector3 screenFeet = mainCam.WorldToScreenPoint(npc.transform.position);
                if (screenHead.z <= 0 || screenFeet.z <= 0)
                    continue;
                Color boxColor = npc.name.Contains("Officer") ? MainModState.CopBoxColor : MainModState.NPCBoxColor;
                if (boxColor.a == 0f)
                    boxColor = npc.name.Contains("Officer") ? Color.blue : Color.yellow;
                boxColor.a = alpha;
                if (MainModState.Use3DBoxes)
                    Draw3DBox(npc.transform.position + Vector3.up * 1.8f, npc.transform.position, boxColor);
                else
                    Draw2DBox(screenHead, screenFeet, boxColor);
                Vector2 top2D = new Vector2(screenHead.x, Screen.height - screenHead.y);
                Vector2 bottom2D = new Vector2(screenFeet.x, Screen.height - screenFeet.y);
                float boxHeight = Mathf.Abs(top2D.y - bottom2D.y);
                float boxWidth = boxHeight / 2f;
                float centerX = (top2D.x + bottom2D.x) / 2f;

                float healthBarAboveBox = 4f;
                float barHeight = 4f;
                float nameAboveBox = 8f;
                float barBottomY = top2D.y - healthBarAboveBox;
                float barTopY = barBottomY - barHeight;
                Vector2 barPos = new Vector2(centerX - (boxWidth / 2f), barTopY);
                GUIStyle nameStyle = MakeLabelStyle(Color.white);
                Vector2 nameSize = nameStyle.CalcSize(new GUIContent(npc.name));
                float nameBottomY = top2D.y - nameAboveBox;
                float nameTopY = nameBottomY - nameSize.y;
                float nameLeftX = centerX - (nameSize.x / 2f);
                if (MainModState.ShowNames)
                {
                    Rect nameRect = new Rect(nameLeftX, nameTopY, nameSize.x, nameSize.y);
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                    GUI.Label(nameRect, npc.name, nameStyle);
                    GUI.color = Color.white;
                }
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
                if (MainModState.ShowDistance)
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
        private void Draw2DBox(Vector3 head, Vector3 feet, Color color)
        {
            Vector2 top = new Vector2(head.x, Screen.height - head.y);
            Vector2 bottom = new Vector2(feet.x, Screen.height - feet.y);
            float height = Mathf.Abs(top.y - bottom.y);
            float width = height / 2;
            Vector2 center = new Vector2(bottom.x, (top.y + bottom.y) / 2);

            GUI.color = color;
            Texture2D tex = Texture2D.whiteTexture;
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, width, 1), tex);
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y + height / 2, width, 1), tex);
            GUI.DrawTexture(new Rect(center.x - width / 2, center.y - height / 2, 1, height), tex);
            GUI.DrawTexture(new Rect(center.x + width / 2, center.y - height / 2, 1, height), tex);
        }
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
