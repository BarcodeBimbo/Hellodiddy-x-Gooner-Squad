using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using Smokey.Mod.UI;
using Smokey.Mod.Features;
using Smokey.Mod.Functions;
using Smokey.Mod.Utils;
using Smokey.Mod.Patches;

namespace Smokey
{
    public class MainMod : MelonMod
    {
        public static MainMod Instance { get; private set; }
        public static bool IsMainScene { get; set; } = false;

        // Configuration properties.
        public bool GodModeEnabled { get => godModeEnabled; set => godModeEnabled = value; }
        public bool AlwaysUnwanted { get => alwaysUnwanted; set => alwaysUnwanted = value; }
        public bool Infinite_Stamina { get => InfiniteStamina; set => InfiniteStamina = value; }
        public bool Infinite_Energy { get => InfiniteEnergy; set => InfiniteEnergy = value; }
        public float JumpMultiplier { get => jumpMultiplier; set => jumpMultiplier = value; }
        public float SpeedMultiplier { get => speedMultiplier; set => speedMultiplier = value; }
        public int ModdedStackSize { get => _moddedStackSize; set => _moddedStackSize = value; }
        public bool PlayerEspEnabled { get => playerEspEnabled; set => playerEspEnabled = value; }
        public bool NpcEspEnabled { get => npcEspEnabled; set => npcEspEnabled = value; }
        public string[] TabNames { get => tabNames; set => tabNames = value; }
        public static float SelectedTime { get => selectedTime; set => selectedTime = value; }
        public int CurrentTab { get => currentTab; set => currentTab = value; }
        public int BoxModeIndex { get => boxModeIndex; set => boxModeIndex = value; }
        public Dictionary<string, List<string>> SpawnerItems { get => spawnerItems; set => spawnerItems = value; }
        public Vector2 SpawnerCatScrollPos { get => spawnerCatScrollPos; set => spawnerCatScrollPos = value; }
        public string SelectedCategory { get => selectedCategory; set => selectedCategory = value; }
        public int SpawnerItemAmount { get => spawnerItemAmount; set => spawnerItemAmount = value; }
        public ESP Esp { get => esp; set => esp = value; }
        public float MoneyAmount { get => moneyAmount; set => moneyAmount = value; }
        public int XpAmount { get => xpAmount; set => xpAmount = value; }
        public int SelectedPlayerIndex { get => selectedPlayerIndex; set => selectedPlayerIndex = value; }
        public static string ItemsJson => itemsJson;

        // Private fields.
        private static bool isMainScene = false;
        private bool godModeEnabled = false;
        private bool alwaysUnwanted = false;
        private bool InfiniteStamina = false;
        private bool InfiniteEnergy = false;
        private float jumpMultiplier = 1f;
        private float speedMultiplier = 1f;
        private int _moddedStackSize = 20;
        private float lobbyCapacity = 4f;
        private bool playerEspEnabled = false;
        private bool npcEspEnabled = false;
        private string[] tabNames = { "Online", "Self", "Aimbot & ESP", "Spawner", "World", "Misc" };
        private static float selectedTime = 1f;
        private int currentTab = 0;
        private int boxModeIndex = 1;
        private Dictionary<string, List<string>> spawnerItems = new Dictionary<string, List<string>>();
        private Vector2 spawnerCatScrollPos = Vector2.zero;
        private string selectedCategory = "";
        private int spawnerItemAmount = 1;
        private ESP esp = new ESP();
        private float moneyAmount = 100f;
        private int xpAmount = 100;
        private int selectedPlayerIndex = -1;
        // Field for dropdown state.
        private bool categoryDropdownExpanded = false;
        // Persistent scroll position for the spawner items.
        private Vector2 spawnerItemScrollPos = Vector2.zero;
        // Field for aimbot & ESP scroll view.
        private Vector2 aimbotEspScrollPos = Vector2.zero;
        private const string itemsJson = @"{ ""categories"": [ { ""category"": ""Product"", ""items"": [ ""granddaddypurple"", ""greencrack"", ""ogkush"", ""sourdiesel"", ""cocaine"", ""meth"" ] }, { ""category"": ""Packaging"", ""items"": [ ""baggie"", ""brick"", ""jar"" ] }, { ""category"": ""Growing"", ""items"": [ ""extralonglifesoil"", ""fertilizer"", ""longlifesoil"", ""pgr"", ""soil"", ""speedgrow"", ""cocaleaf"", ""granddaddypurpleseed"", ""greencrackseed"", ""ogkushseed"", ""sourdieselseed"", ""cocaseed"" ] }, { ""category"": ""Tools"", ""items"": [ ""electrictrimmers"", ""flashlight"", ""managementclipboard"", ""trashbag"", ""trashgrabber"", ""trimmers"", ""wateringcan"", ""bigsprinkler"", ""soilpourer"", ""potsprinkler"", ""baseballbat"", ""fryingpan"", ""m1911"", ""m1911mag"", ""machete"", ""revolvercylinder"", ""revolver"" ] }, { ""category"": ""Furniture"", ""items"": [ ""coffeetable"", ""dumpster"", ""suspensionrack"", ""metalsquaretable"", ""trashcan"", ""bed"", ""smalltrashcan"", ""TV"", ""woodsquaretable"" ] }, { ""category"": ""Lightning"", ""items"": [ ""fullspectrumgrowlight"", ""halogengrowlight"", ""ledgrowlight"" ] }, { ""category"": ""Consumable"", ""items"": [ ""banana"", ""cuke"", ""donut"", ""energydrink"", ""flumedicine"", ""chili"" ] }, { ""category"": ""Equipment"", ""items"": [ ""airpot"", ""plasticpot"", ""growtent"", ""moisturepreservingpot"", ""largestoragerack"", ""mediumstoragerack"", ""smallstoragerack"", ""brickpress"", ""cauldron"", ""chemistrystation"", ""dryingrack"", ""laboven"", ""launderingstation"", ""mixingstation"", ""mixingstationmk"", ""packagingstation"", ""packagingstationmk"" ] }, { ""category"": ""Ingredient"", ""items"": [ ""acid"", ""addy"", ""battery"", ""gasoline"", ""horsesemen"", ""motoroil"", ""mouthwash"", ""paracetamol"", ""viagra"" ] }, { ""category"": ""Decoration"", ""items"": [ ""floorlamp"", ""displaycabinet"", ""filingcabinet"" ] }, { ""category"": ""Clothing"", ""items"": [ ""cargopants"", ""jeans"", ""jorts"", ""longskirt"", ""overalls"", ""skirt"", ""legendsunglasses"", ""rectangleframeglasses"", ""smallroundglasses"", ""speeddealershades"", ""combatboots"", ""dressshoes"", ""flats"", ""sandals"", ""sneakers"", ""fingerlessgloves"", ""gloves"", ""buckethat"", ""cap"", ""chefhat"", ""cowboyhat"", ""flatcap"", ""porkpiehat"", ""saucepan"", ""apron"", ""blazer"", ""collarjacket"", ""tacticalvest"", ""vest"", ""buttonup"", ""rolledbuttonup"", ""flannelshirt"", ""tshirt"", ""vneck"", ""belt"" ] }, { ""category"": ""Vehicle"", ""items"": [ ""cheapskateboard"", ""cruiser"", ""goldenskateboard"", ""lightweightskateboard"", ""skateboard"" ] }, { ""category"": ""Special"", ""items"": [ ""chateaulapeepee"", ""goldbar"", ""oldmanjimmys"", ""cocainebase"", ""iodine"", ""liquidbabyblue"", ""liquidbikercrank"", ""liquidglass"", ""liquidmeth"", ""megabean"", ""phosphorus"", ""babyblue"", ""bikercrank"", ""glass"", ""testweed"" ] } ] }";

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Initializing Patches...");
            var harmony = new HarmonyLib.Harmony("A51");
            Initialize.CustomPatches(harmony);
        }

        public override void OnInitializeMelon()
        {
            Instance = this;
            Settings.windowRect = new Rect(20, 20, 450, 500);
            SpawnerItems = LoadItemsFromJson();
            if (SpawnerItems.Count > 0)
            {
                foreach (var key in SpawnerItems.Keys)
                {
                    SelectedCategory = key;
                    break;
                }
                MelonLogger.Msg("Spawner items loaded: " + SpawnerItems.Count + " categories.");
            }
            else
            {
                MelonLogger.Error("No spawner items loaded.");
            }
        }

        public Dictionary<string, List<string>> LoadItemsFromJson()
        {
            Dictionary<string, List<string>> itemTree = new Dictionary<string, List<string>>();
            try
            {
                string[] categories = ItemsJson.Split(new string[] { "\"category\":" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cat in categories)
                {
                    if (!cat.Contains("\"items\":"))
                        continue;
                    string[] parts = cat.Split('"');
                    if (parts.Length < 2)
                    {
                        MelonLogger.Error("Failed to parse category name.");
                        continue;
                    }
                    string categoryName = parts[1];
                    string[] itemParts = cat.Split(new string[] { "\"items\": [" }, StringSplitOptions.None);
                    if (itemParts.Length < 2)
                    {
                        MelonLogger.Error("Failed to find item list for category " + categoryName);
                        continue;
                    }
                    itemParts[1] = itemParts[1].Replace("]", "").Replace("}", "");
                    string itemsPart = itemParts[1];
                    List<string> items = new List<string>();
                    foreach (string item in itemsPart.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedItem = item.Trim();
                        if (trimmedItem == "[" || trimmedItem == "]" || trimmedItem == "{" || trimmedItem == "}")
                            continue;
                        if (!string.IsNullOrWhiteSpace(trimmedItem))
                        {
                            items.Add(trimmedItem);
                        }
                    }
                    itemTree[categoryName] = items;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Error parsing embedded JSON: " + ex);
            }
            return itemTree;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            IsMainScene = Utils.CheckMainScene(sceneName, false);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            IsMainScene = Utils.CheckMainScene(sceneName, true);
        }

        public override void OnGUI()
        {
            if (Settings.showUI)
            {
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.85f);
                Settings.windowRect = GUI.Window(Settings.windowID, Settings.windowRect, (GUI.WindowFunction)DrawWindow, "HelloDiddy x GoonerSquad");
            }
            if (IsMainScene)
            {
                if (PlayerEspEnabled)
                    esp.DrawESPPlayers(Features.GetPlayers());
                if (NpcEspEnabled)
                    esp.DrawESPNPC(Features.GetNPCS());
                // Draw the Aimbot FOV overlay.
                AimbotVisual.DrawFOV();
            }
        }

        private void DrawWindow(int id)
        {
            if (IsMainScene)
            {
                UI.BeginVertical();
                UI.BeginHorizontal();
                for (int i = 0; i < TabNames.Length; i++)
                {
                    if (UI.Button(TabNames[i]))
                    {
                        CurrentTab = i;
                    }
                }
                UI.EndHorizontal();
                switch (CurrentTab)
                {
                    case 0:
                        DrawPlayerTab(); // "Online" tab functionality.
                        break;
                    case 1:
                        DrawSelfTab();
                        break;
                    case 2:
                        DrawAimbotESPTab(); // Combined "Aimbot & ESP" tab.
                        break;
                    case 3:
                        DrawSpawnerTab();
                        break;
                    case 4:
                        DrawWorldTab();
                        break;
                    case 5:
                        DrawMiscTab();
                        break;
                    default:
                        UI.Label("Invalid Tab", UI.HeaderStyle);
                        break;
                }
                UI.EndVertical();
                UI.DragWindow();
            }
            else
            {
                UI.Label("Load into the game to access features.", UI.HeaderStyle);
            }
        }

        private void DrawSelfTab()
        {
            if (IsMainScene)
            {
                UI.Label("Self Options", UI.HeaderStyle);
                GodModeEnabled = UI.Toggle(GodModeEnabled, "GodMode");
                AlwaysUnwanted = UI.Toggle(AlwaysUnwanted, "Always Unwanted");
                UniversalNoclip.IsFlying = UI.Toggle(UniversalNoclip.IsFlying, "Hulk Mode");
                Features.OnePunchEnabled = UI.Toggle(Features.OnePunchEnabled, "Super Punch");
                UI.Label("Jump Multiplier: " + JumpMultiplier.ToString("F1"));
                JumpMultiplier = UI.Slider(JumpMultiplier, 1f, 10f);
                if (PlayerMovement.Instance != null)
                {
                    PlayerMovement.JumpMultiplier = JumpMultiplier;
                }
                else
                {
                    UI.Label("PlayerMovement not found");
                }
                UI.Label("Movement Multiplier: " + SpeedMultiplier.ToString("F1"));
                SpeedMultiplier = UI.Slider(SpeedMultiplier, 1f, 10f);
                if (PlayerMovement.Instance != null)
                {
                    PlayerMovement.Instance.MoveSpeedMultiplier = SpeedMultiplier;
                }
                else
                {
                    UI.Label("PlayerMovement.Instance not found");
                }
                UI.Label("Stack Size: " + ModdedStackSize);
                ModdedStackSize = (int)UI.Slider(ModdedStackSize, 20, 9999);
                if (UI.Button("Sleep"))
                {
                    if (Features.LocalPlayer != null && Features.LocalPlayer.Health != null)
                        Features.ForceSleep();
                }
            }
        }

        private void DrawSpawnerTab()
        {
            if (!IsMainScene)
                return;

            // Use UI.HeaderStyle for a unified look.
            GUIStyle headerStyle = UI.HeaderStyle;

            GUILayout.Space(10);
            GUILayout.Label("Spawner Options", headerStyle);
            GUILayout.Space(5);

            // CATEGORY DROPDOWN
            if (GUILayout.Button("Category: " + SelectedCategory, GUILayout.Height(30)))
            {
                categoryDropdownExpanded = !categoryDropdownExpanded;
            }
            if (categoryDropdownExpanded)
            {
                GUILayout.BeginVertical("box");
                List<string> categoryList = new List<string>(SpawnerItems.Keys);
                foreach (string cat in categoryList)
                {
                    if (GUILayout.Button(cat, GUILayout.Height(25)))
                    {
                        SelectedCategory = cat;
                        categoryDropdownExpanded = false;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);

            // ITEMS LIST
            GUILayout.Label("Items in: " + SelectedCategory, headerStyle);
            GUILayout.Space(5);
            if (SpawnerItems.ContainsKey(SelectedCategory))
            {
                spawnerItemScrollPos = GUILayout.BeginScrollView(spawnerItemScrollPos, GUILayout.Height(200));

                int columns = 2; // Two items per row.
                int counter = 0;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace(); // Left flexible space.
                foreach (string item in SpawnerItems[SelectedCategory])
                {
                    if (GUILayout.Button(item, GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        var command = new Il2CppScheduleOne.Console.AddItemToInventoryCommand();
                        var args = new Il2CppSystem.Collections.Generic.List<string>();
                        args.Add(item);
                        args.Add(SpawnerItemAmount.ToString());
                        command.Execute(args);
                    }
                    counter++;
                    if (counter % columns == 0)
                    {
                        GUILayout.FlexibleSpace(); // Right flexible space.
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.Space(5);
                    }
                }
                GUILayout.FlexibleSpace(); // End last row.
                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();
            }

            // ITEM AMOUNT CONTROL
            GUILayout.Label("Item Amount: " + SpawnerItemAmount, headerStyle);
            SpawnerItemAmount = (int)UI.Slider(SpawnerItemAmount, 1, ModdedStackSize);
        }

        private void DrawAimbotESPTab()
        {
            if (IsMainScene)
            {
                aimbotEspScrollPos = GUILayout.BeginScrollView(aimbotEspScrollPos, GUILayout.Height(400));

                // ESP Settings
                UI.Label("ESP Settings", UI.HeaderStyle);
                PlayerEspEnabled = UI.Toggle(PlayerEspEnabled, "Enable Player ESP");
                NpcEspEnabled = UI.Toggle(NpcEspEnabled, "Enable NPC ESP");
                UI.Label("Box Style:");
                BoxModeIndex = GUILayout.SelectionGrid(BoxModeIndex, new string[] { "2D Box", "3D Box" }, 2);
                Features.Use3DBoxes = BoxModeIndex == 1;
                UI.Label("Max ESP Distance: " + Mathf.RoundToInt(Features.MaxESPDistance) + "m");
                Features.MaxESPDistance = UI.Slider(Features.MaxESPDistance, 10f, 300f);
                Features.ShowNames = UI.Toggle(Features.ShowNames, "Show Names");
                Features.ShowDistance = UI.Toggle(Features.ShowDistance, "Show Distance");
                UI.Label("Cop ESP(RGB):");
                Features.CopBoxColor = UI.ColorField(Features.CopBoxColor);
                UI.Label("NPC ESP(RGB):");
                Features.NPCBoxColor = UI.ColorField(Features.NPCBoxColor);
                UI.Label("Player ESP(RGB):");
                Features.PlayerBoxColor = UI.ColorField(Features.PlayerBoxColor);

                UI.Label(" "); // Separator

                // Aimbot Settings
                UI.Label("Aimbot Options", UI.HeaderStyle);
                Aimbot.AimbotEnabled = UI.Toggle(Aimbot.AimbotEnabled, "Enable Aimbot (Right Click)");
                Aimbot.SilentAimbotEnabled = UI.Toggle(Aimbot.SilentAimbotEnabled, "Enable Silent Aimbot (Right Click)");
                Aimbot.VisibilityCheck = UI.Toggle(Aimbot.VisibilityCheck, "Require Visibility Check");
                Aimbot.DrawFovEnabled = UI.Toggle(Aimbot.DrawFovEnabled, "Draw Aimbot FOV");
                UI.Label("Aimbot FOV: " + Mathf.RoundToInt(Aimbot.FOV));
                Aimbot.FOV = UI.Slider(Aimbot.FOV, 10f, 500f);
                UI.Label("Bullet Speed: " + Aimbot.BulletSpeed.ToString("F2"));
                Aimbot.BulletSpeed = UI.Slider(Aimbot.BulletSpeed, 10f, 150f);
                Aimbot.EnablePrediction = UI.Toggle(Aimbot.EnablePrediction, "Enable Prediction");
                UI.Label("Aim Prediction Factor: " + Aimbot.PredictionFactor.ToString("F2"));
                Aimbot.PredictionFactor = UI.Slider(Aimbot.PredictionFactor, 0f, 2f);
                Aimbot.AimSmoothingEnabled = UI.Toggle(Aimbot.AimSmoothingEnabled, "Enable Aim Smoothing");
                UI.Label("Smooth Factor: " + Aimbot.SmoothFactor.ToString("F2"));
                Aimbot.SmoothFactor = UI.Slider(Aimbot.SmoothFactor, 0f, 100f);
                UI.Label("NPC Aim Smoothness: " + Aimbot.NPCSmoothFactor.ToString("F2"));
                Aimbot.NPCSmoothFactor = UI.Slider(Aimbot.NPCSmoothFactor, 0f, 100f);
                UI.Label("Aim Inaccuracy: " + Aimbot.AimInaccuracy.ToString("F2"));
                Aimbot.AimInaccuracy = UI.Slider(Aimbot.AimInaccuracy, 0f, 5f);
                UI.Label("Priority Mode: " + Aimbot.CurrentPriority, UI.HeaderStyle);
                if (UI.Button("Cycle Priority Mode"))
                    Aimbot.CyclePriority();

                GUILayout.EndScrollView();
            }
        }

        private void DrawWorldTab()
        {
            if (IsMainScene)
            {
                UI.Label("World Control", UI.HeaderStyle);
                UI.Label("Selected Time: " + Mathf.RoundToInt(SelectedTime) + "h");
                SelectedTime = UI.Slider(SelectedTime, 0f, 24f);
                if (UI.Button("Set Time"))
                    Features.SetTime(Mathf.RoundToInt(SelectedTime));
            }
        }

        private void DrawPlayerTab()
        {
            if (IsMainScene)
            {
                UI.Label("Player List", UI.HeaderStyle);
                var players = Features.GetPlayers();
                int maxButtons = Mathf.Min(4, players.Count);
                for (int i = 0; i < maxButtons; i++)
                {
                    var player = players[i];
                    if (player != null && player.transform != null)
                    {
                        if (UI.Button("[" + i + "] " + player.name))
                            SelectedPlayerIndex = i;
                    }
                }
                if (SelectedPlayerIndex >= 0 && SelectedPlayerIndex < players.Count)
                {
                    var selectedPlayer = players[SelectedPlayerIndex];
                    if (selectedPlayer != null && selectedPlayer.transform != null)
                    {
                        UI.Label("Name: " + selectedPlayer.name, UI.HeaderStyle);
                        UI.Label("Position: " + selectedPlayer.transform.position);
                        if (selectedPlayer.Health != null)
                            UI.Label("Health: " + selectedPlayer.Health.CurrentHealth + " HP");
                        if (UI.Button("Teleport to Player"))
                            Features.LocalPlayer.transform.position = selectedPlayer.transform.position;
                    }
                }
            }
        }

        private void DrawMiscTab()
        {
            if (IsMainScene)
            {
                UI.Label("Miscellaneous Options", UI.HeaderStyle);
                if (UI.Button("Force Quit Game"))
                    Application.Quit();
                if (UI.Button("Unlock All Achievements"))
                {
                    foreach (var achievement in Enum.GetValues(typeof(AchievementManager.EAchievement)))
                    {
                        AchievementManager.Instance.UnlockAchievement((AchievementManager.EAchievement)achievement);
                    }
                }
                if (UI.Button("Change Quality"))
                {
                    foreach (var qualityType in Enum.GetValues(typeof(EQuality)))
                    {
                        if (GUILayout.Button(qualityType.ToString()))
                        {
                            Il2CppScheduleOne.Console.SetQuality command = new Il2CppScheduleOne.Console.SetQuality();
                            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
                            args.Add(qualityType.ToString());
                            command.Execute(args);
                        }
                    }
                }
                UI.Label($"Money Amount: {Mathf.RoundToInt(moneyAmount)}", UI.HeaderStyle);
                moneyAmount = UI.Slider(moneyAmount, 0, 1000000);
                if (UI.Button("Give Money"))
                    Features.GiveMoney(moneyAmount);
                UI.Label($"XP Amount: {xpAmount}", UI.HeaderStyle);
                xpAmount = (int)UI.Slider(xpAmount, 0, 100000);
                if (UI.Button("Give XP"))
                    Features.GiveXP(xpAmount);
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(Settings.guiToggleKey))
            {
                Settings.showUI = !Settings.showUI;
                if (IsMainScene)
                {
                    Cursor.visible = Settings.showUI;
                    Cursor.lockState = Settings.showUI ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
            if (IsMainScene)
            {
                if (GodModeEnabled)
                    Features.HandleGodMode();
                if (AlwaysUnwanted)
                    Features.NeverWanted();
                if (Features.OnePunchEnabled)
                    Features.HandleSuperPunch();

                if (Aimbot.AimbotEnabled || Aimbot.SilentAimbotEnabled)
                    Aimbot.HandleAimbot();
                UniversalNoclip.Update();
            }
        }
    }
}
