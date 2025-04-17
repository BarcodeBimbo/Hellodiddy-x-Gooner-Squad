using HelloDiddy.Mod.Features;
using HelloDiddy.Mod.UI;
using Il2CppScheduleOne;
using UnityEngine;
using System.Linq;
using HelloDiddy.Mod;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using HelloDiddy.Mod.UI.Config;

namespace HelloDiddy
{
    public static class TabSystem
    {
        public static void DrawPlayerTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.playerTabScroll = GUILayout.BeginScrollView(MainModState.Instance.playerTabScroll, "box");
            GUILayout.Space(10);
            UI.Label("Player List", UI.HeaderStyle);
            GUILayout.Space(10);

            var players = Features.GetPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player != null && player.transform != null)
                {
                    if (GUILayout.Button(player.name, GUILayout.Height(25)))
                        MainModState.Instance.SelectedPlayerIndex = i;
                }
            }

            if (MainModState.Instance.SelectedPlayerIndex >= 0 && MainModState.Instance.SelectedPlayerIndex < players.Count)
            {
                var selectedPlayer = players[MainModState.Instance.SelectedPlayerIndex];
                if (selectedPlayer != null && selectedPlayer.transform != null)
                {
                    GUILayout.Space(10);
                    UI.Label("Player Info", UI.HeaderStyle);
                    UI.Label($"Name: {selectedPlayer.name}");
                    UI.Label($"Position: {selectedPlayer.transform.position}");

                    if (selectedPlayer.Health != null)
                        UI.Label($"Health: {selectedPlayer.Health.CurrentHealth} HP");

                    GUILayout.Space(5);
                    if (GUILayout.Button("Teleport to Player", GUILayout.Height(25)) && MainModState.LocalPlayer != null)
                        MainModState.LocalPlayer.transform.position = selectedPlayer.transform.position;

                    if (GUILayout.Button($"Kill {selectedPlayer.name}", GUILayout.Height(25)) && selectedPlayer.Health != null)
                        selectedPlayer.Health.SetHealth(0f);

                }
            }

            GUILayout.EndScrollView();
        }

        public static void DrawSelfTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.selfTabScroll = GUILayout.BeginScrollView(MainModState.Instance.selfTabScroll, "box");
            GUILayout.Space(10);
            UI.Label("Self Options", UI.HeaderStyle);

            UI.CreateConfigToggle("God Mode", () => MainModState.Instance.GodModeEnabled, v => MainModState.Instance.GodModeEnabled = v);
            UI.CreateConfigToggle("Unlimited Ammo", () => MainModState.Instance.UnlimitedAmmoEnabled, v => MainModState.Instance.UnlimitedAmmoEnabled = v);
            UI.CreateConfigToggle("Infinite Stamina", () => MainModState.Instance.InfiniteStamina, v => MainModState.Instance.InfiniteStamina = v);
            UI.CreateConfigToggle("Infinite Energy", () => MainModState.Instance.InfiniteEnergy, v => MainModState.Instance.InfiniteEnergy = v);
            UI.CreateConfigToggle("Always Unwanted", () => MainModState.Instance.AlwaysUnwanted, v => MainModState.Instance.AlwaysUnwanted = v);

            GUILayout.Space(10);
            UI.Label("Combat Tweaks", UI.HeaderStyle);

            UI.CreateConfigToggle("No Recoil", () => MainModState.Instance.NoRecoilEnabled, v => MainModState.Instance.NoRecoilEnabled = v);
            UI.CreateConfigToggle("No Spread", () => MainModState.Instance.NoSpreadEnabled, v => MainModState.Instance.NoSpreadEnabled = v);
            UI.CreateConfigToggle("Rapid Fire", () => MainModState.Instance.RapidFireEnabled, v => MainModState.Instance.RapidFireEnabled = v);
            UI.CreateConfigToggle("Super Impact", () => MainModState.Instance.SuperImpactEnabled, v => MainModState.Instance.SuperImpactEnabled = v);
            UI.CreateConfigToggle("One Shot Kill", () => MainModState.Instance.OneShotKillEnabled, v => MainModState.Instance.OneShotKillEnabled = v);
            UI.CreateConfigToggle("Explosive Bullet", () => MainModState.ExplosiveBulletsEnabled, v => MainModState.ExplosiveBulletsEnabled = v);

            GUILayout.Space(10);
            UI.Label("Item Options", UI.HeaderStyle);
            GUILayout.Space(10);
            UI.CreateConfigToggle("Auto Grow", () => MainModState.AutoGrowEnabled, v => MainModState.AutoGrowEnabled = v);
            if (MainModState.AutoGrowEnabled)
            {
                UI.Label("Select Seed for Auto Grow:");
                MainModState.SelectedGrowSeedIndex = GUILayout.SelectionGrid((MainModState.SelectedGrowSeedIndex <= 0 ? 1 : MainModState.SelectedGrowSeedIndex) - 1, MainModState.AutoGrowSeeds.Skip(1).ToArray(), 1) + 1;
            }
            else
            {
                MainModState.SelectedGrowSeedIndex = 0;
            }

            UI.CreateConfigToggle("Trash Picker++", () => MainModState.Instance.TrashPlusPlusPatch, v => MainModState.Instance.TrashPlusPlusPatch = v);
            UI.CreateConfigToggle("Trash Deleter", () => MainModState.Instance.TrashPatchEnabled, v => MainModState.Instance.TrashPatchEnabled = v);
          
            MainModState.Instance.DupPatchEnabled = UI.Toggle(MainModState.Instance.DupPatchEnabled, "Item Duplication");
            GUILayout.Space(5);
            UI.NotifySlider("Duplication Multiplier", 2, 10, () => MainModState.Instance.DuplicationMultiplier, v => MainModState.Instance.DuplicationMultiplier = v);
            GUILayout.Space(5);
           
            MainModState.Instance.StackPatchEnabled = UI.Toggle(MainModState.Instance.StackPatchEnabled, "Custom Stack Size");
            GUILayout.Space(5);
            UI.NotifySlider("Stack Size", 20, 9999, () => MainModState.Instance.ModdedStackSize, v => MainModState.Instance.ModdedStackSize = v);
            GUILayout.Space(10);
           
            UI.Label("Fun Options", UI.HeaderStyle);
            UI.CreateConfigToggle("Super Punch", () => MainModState.SuperPunchEnabled, v => MainModState.SuperPunchEnabled = v);
            GUILayout.Space(5);
            UI.NotifySlider("Punch Range", 1f, 1000f, () => MainModState.SuperPunchDist, v => MainModState.SuperPunchDist = v);

            GUILayout.Space(5);
            UI.NotifySlider("Jump Multiplier", 1f, 10f, () => MainModState.Instance.JumpMultiplier, v =>
            {
                MainModState.Instance.JumpMultiplier = v;
                if (PlayerMovement.Instance != null)
                    PlayerMovement.JumpMultiplier = v;
            });
            GUILayout.Space(5);
            UI.NotifySlider("Speed Multiplier", 1f, 10f, () => MainModState.Instance.SpeedMultiplier, v =>
            {
                MainModState.Instance.SpeedMultiplier = v;
                if (PlayerMovement.Instance != null)
                    PlayerMovement.Instance.MoveSpeedMultiplier = v;
            });
            GUILayout.Space(5);
            UI.NotifySlider("Money Amount", 0, 1000000, () => MainModState.Instance.MoneyAmount, v => MainModState.Instance.MoneyAmount = v);
            if (UI.Button("Give Money"))
                Features.GiveMoney(MainModState.Instance.MoneyAmount);

            if (UI.Button("Sleep"))
                Features.ForceSleep();

            GUILayout.EndScrollView();
        }

        public static void DrawAimbotEspTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.aimbotEspScrollPos = GUILayout.BeginScrollView(MainModState.Instance.aimbotEspScrollPos, "box");
            UI.Label("ESP Settings", UI.HeaderStyle);

            UI.CreateConfigToggle("Player ESP", () => MainModState.Instance.PlayerEspEnabled, v => MainModState.Instance.PlayerEspEnabled = v);
            UI.CreateConfigToggle("NPC ESP", () => MainModState.Instance.NpcEspEnabled, v => MainModState.Instance.NpcEspEnabled = v);
            UI.CreateConfigToggle("Show Names", () => MainModState.ShowNames, v => MainModState.ShowNames = v);
            UI.CreateConfigToggle("Show Distance", () => MainModState.ShowDistance, v => MainModState.ShowDistance = v);

            UI.Label("Box Type", UI.SubHeaderStyle);
            MainModState.Instance.BoxModeIndex = GUILayout.SelectionGrid(MainModState.Instance.BoxModeIndex, new string[] { "2D Box", "3D Box" }, 2);
            MainModState.Use3DBoxes = MainModState.Instance.BoxModeIndex == 1;

            UI.NotifySlider("Max ESP Distance", 10f, 300f, () => MainModState.MaxESPDistance, v => MainModState.MaxESPDistance = v);

            UI.DrawColorPicker("Player Box Color", ref MainModState.PlayerBoxColor);
            UI.DrawColorPicker("NPC Box Color", ref MainModState.NPCBoxColor);
            UI.DrawColorPicker("Cop Box Color", ref MainModState.CopBoxColor);

            GUILayout.Space(15);
            UI.Label("Aimbot Settings", UI.HeaderStyle);

            UI.CreateConfigToggle("Legit Aimbot", () => MainModState.AimbotEnabled, v => MainModState.AimbotEnabled = v);
            

            UI.CreateConfigToggle("Unfair Aimbot (Shoot Through Walls)", () => MainModState.UnfairAimbotEnabled, v => MainModState.UnfairAimbotEnabled = v);
            UI.CreateConfigToggle("TriggerBot (Auto Fire)", () => MainModState.TriggerBotEnabled, v => MainModState.TriggerBotEnabled = v);

            GUILayout.Space(5);
            UI.NotifySlider("Unfair Aim Distance", 1f, 300f, () => MainModState.UnfairAimbotDistance, v => MainModState.UnfairAimbotDistance = v);
            GUILayout.Space(5);
            UI.Label("Current Priority: " + Aimbot.CurrentPriority.ToString());
            if (UI.Button("Cycle Priority"))
                Aimbot.CyclePriority();

            GUILayout.Space(15);

            UI.Label("FOV Settings", UI.HeaderStyle);
            GUILayout.Space(5);
            UI.CreateConfigToggle("Draw FOV Circle", () => MainModState.DrawFovEnabled, v => MainModState.DrawFovEnabled = v);
            GUILayout.Space(5);
            UI.NotifySlider("FOV Angle", 10f, 500f, () => MainModState.FOV, v => MainModState.FOV = v);
            GUILayout.Space(5);
            UI.CreateConfigToggle("Bullet Prediction", () => MainModState.EnablePrediction, v => MainModState.EnablePrediction = v);
            GUILayout.Space(5);
            UI.NotifySlider("Prediction Factor", 0f, 2f, () => MainModState.PredictionFactor, v => MainModState.PredictionFactor = v);
            GUILayout.Space(5);

            UI.CreateConfigToggle("Enable Smoothing", () => MainModState.AimSmoothingEnabled, v => MainModState.AimSmoothingEnabled = v);
            GUILayout.Space(5);
            UI.NotifySlider("Smoothness (Players)", 0f, 100f, () => MainModState.SmoothFactor, v => MainModState.SmoothFactor = v);
            GUILayout.Space(5);
            UI.NotifySlider("Smoothness (NPCs)", 0f, 100f, () => MainModState.NPCSmoothFactor, v => MainModState.NPCSmoothFactor = v);
            GUILayout.Space(5);
            UI.NotifySlider("Aim Inaccuracy (Spread)", 0f, 5f, () => MainModState.AimInaccuracy, v => MainModState.AimInaccuracy = v);
            GUILayout.Space(5);
            UI.NotifySlider("Bullet Speed (Prediction)", 10f, 150f, () => MainModState.BulletSpeed, v => MainModState.BulletSpeed = v);

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }

        public static void DrawSpawnerTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.spawnerTabScroll = GUILayout.BeginScrollView(MainModState.Instance.spawnerTabScroll, "box");
            GUILayout.Space(10);
            UI.Label("Spawner Options", UI.HeaderStyle);

            if (GUILayout.Button($"Category: {MainModState.Instance.SelectedCategory}", GUILayout.Height(30)))
                MainModState.Instance.categoryDropdownExpanded = !MainModState.Instance.categoryDropdownExpanded;

            if (MainModState.Instance.categoryDropdownExpanded)
            {
                GUILayout.BeginVertical("box");
                foreach (var category in MainModState.Instance.SpawnerItems.Keys)
                {
                    if (GUILayout.Button(category, GUILayout.Height(25)))
                    {
                        MainModState.Instance.SelectedCategory = category;
                        MainModState.Instance.categoryDropdownExpanded = false;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            UI.Label($"Items in: {MainModState.Instance.SelectedCategory}");

            if (MainModState.Instance.SpawnerItems.ContainsKey(MainModState.Instance.SelectedCategory))
            {
                MainModState.Instance.spawnerItemScrollPos = GUILayout.BeginScrollView(MainModState.Instance.spawnerItemScrollPos, GUILayout.Height(200));
                int columns = 2, count = 0;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                foreach (var item in MainModState.Instance.SpawnerItems[MainModState.Instance.SelectedCategory])
                {
                    if (GUILayout.Button(item, GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        var command = new Console.AddItemToInventoryCommand();
                        var args = new Il2CppSystem.Collections.Generic.List<string>();
                        args.Add(item);
                        args.Add(MainModState.Instance.SpawnerItemAmount.ToString());
                        command.Execute(args);
                    }

                    count++;
                    if (count % columns == 0)
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.Space(10);
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }

            UI.NotifySlider("Item Amount", 1, MainModState.Instance.ModdedStackSize, () => MainModState.Instance.SpawnerItemAmount, v => MainModState.Instance.SpawnerItemAmount = v);
            GUILayout.EndScrollView();
        }


        public static void DrawWorldTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.worldTabScroll = GUILayout.BeginScrollView(MainModState.Instance.worldTabScroll, "box");
            GUILayout.Space(10);
            UI.Label("World Control", UI.HeaderStyle);

            UI.NotifySlider("Selected Time", 0f, 24f,
                () => MainModState.SelectedTime,
                v => MainModState.SelectedTime = v
            );

            if (UI.Button("Set Time"))
                Features.SetTime(Mathf.RoundToInt(MainModState.SelectedTime));

            GUILayout.EndScrollView();
        }

        public static void DrawMiscTab()
        {
            if (!MainModState.IsMainScene) return;

            MainModState.Instance.miscTabScroll = GUILayout.BeginScrollView(MainModState.Instance.miscTabScroll, "box");
            GUILayout.Space(10);
            UI.Label("Miscellaneous Options", UI.HeaderStyle);

            GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

            // Menu Key
            GUILayout.Label($"Current Menu Key: {MainModState.MenuKeyPreference.Value}", style);

            if (!MainModState.isSelectingMenuKey)
            {
                if (GUILayout.Button("Change Menu Key"))
                    MainModState.isSelectingMenuKey = true;
            }
            else
            {
                GUILayout.Label("Press any key now...");
                var e = Event.current;
                if (e.isKey && e.type == EventType.KeyDown)
                {
                    MainModState.MenuKeyPreference.Value = e.keyCode;
                    MainModState.MenuKeyPreference.Save();
                    Config.Save();
                    MelonLogger.Msg($"Menu Key changed to {e.keyCode}!");
                    MainModState.isSelectingMenuKey = false;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label($"Noclip Keybind: {MainModState.NoclipKeyPrimary.Value} + {MainModState.NoclipKeySecondary.Value}", style);

            if (!MainModState.isSelectingNoclipPrimary && !MainModState.isSelectingNoclipSecondary)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Change Noclip Primary"))
                    MainModState.isSelectingNoclipPrimary = true;

                if (GUILayout.Button("Change Noclip Secondary"))
                    MainModState.isSelectingNoclipSecondary = true;

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Press any key now...");
                var e = Event.current;
                if (e.isKey && e.type == EventType.KeyDown)
                {
                    if (MainModState.isSelectingNoclipPrimary)
                    {
                        MainModState.NoclipKeyPrimary.Value = e.keyCode;
                        MainModState.NoclipKeyPrimary.Save();
                        MelonLogger.Msg($"Noclip Primary Key changed to {e.keyCode}");
                        MainModState.isSelectingNoclipPrimary = false;
                    }
                    else if (MainModState.isSelectingNoclipSecondary)
                    {
                        MainModState.NoclipKeySecondary.Value = e.keyCode;
                        MainModState.NoclipKeySecondary.Save();
                        MelonLogger.Msg($"Noclip Secondary Key changed to {e.keyCode}");
                        MainModState.isSelectingNoclipSecondary = false;
                    }

                    Config.Save();
                }
            }

            /*GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (UI.Button("Save Config"))
            {
                Config.Save();
                MelonLogger.Msg("Configuration saved!");
            }

            GUILayout.Space(10);

            if (UI.Button("Load Config"))
            {
                Config.Init();
                MelonLogger.Msg("Configuration loaded!");
            }

            GUILayout.Space(10);
            GUILayout.EndHorizontal();*/

            GUILayout.Space(10);

            if (UI.Button("Force Quit Game"))
                Application.Quit();

            GUILayout.Space(5);

            if (UI.Button("Unlock All Achievements"))
            {
                if (AchievementManager.Instance != null)
                {
                    foreach (var achievement in System.Enum.GetValues(typeof(AchievementManager.EAchievement)))
                        AchievementManager.Instance.UnlockAchievement((AchievementManager.EAchievement)achievement);
                }
            }

            GUILayout.Space(10);
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical("box");
            UI.Label("Credits", UI.SubHeaderStyle);
            UI.Label("Support: Goonernator", UI.SubHeaderStyle);
            UI.Label("Developer: GoonerToons", UI.SubHeaderStyle);
            UI.Label("NexusMods: GoonerToons & Goonernator", UI.SubHeaderStyle);
            UI.Label("Discords: ikickfatbitches & goonernator5000", UI.SubHeaderStyle);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }
    }
}
