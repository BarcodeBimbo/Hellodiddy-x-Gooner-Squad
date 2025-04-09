using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Money;

namespace Smokey
{
    public class MainMod : MelonMod
    {
        public static MainMod Instance { get; private set; }
        public static bool IsMainScene = false;

        public bool playerEspEnabled = false;
        public bool npcEspEnabled = false;
        private bool godModeEnabled = false;
        private bool alwaysUnwanted = false;

        private static float selectedTime = 1f;
        private int currentTab = 0;
        private int boxModeIndex = 1;
        private string[] tabNames = { "Self", "ESP", "World", "Player", "Misc" };

        public ESP esp = new ESP();

        private float moneyAmount = 100f;
        private int xpAmount = 100;
        private int selectedPlayerIndex = -1;

        public override void OnInitializeMelon()
        {
            Instance = this;
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
                if (playerEspEnabled)
                    esp.DrawESPPlayers(Features.GetPlayers());

                if (npcEspEnabled)
                    esp.DrawESPNPC(Features.GetNPCS());

                AimbotVisual.DrawFOV();
            }
        }

        private void DrawWindow(int id)
        {
            UI.BeginVertical();

            if (IsMainScene)
            {
                UI.BeginHorizontal();
                for (int i = 0; i < tabNames.Length; i++)
                {
                    if (UI.Button(tabNames[i]))
                        currentTab = i;
                }
                UI.EndHorizontal();
            }
            else
            {
                UI.Label("Load into the game to access features.");
            }

            switch (currentTab)
            {
                case 0: SelfTab(); break;
                case 1: ESPTab(); break;
                case 2: WorldTab(); break;
                case 3: PlayerTab(); break;
                case 4: MiscTab(); break;
            }

            UI.EndVertical();
            GUI.DragWindow();
        }

        private void SelfTab()
        {
            UI.Label("Self Options");

            godModeEnabled = UI.Toggle(godModeEnabled, "Enable GodMode");
            alwaysUnwanted = UI.Toggle(alwaysUnwanted, "Always Unwanted");
            UniversalNoclip.IsFlying = UI.Toggle(UniversalNoclip.IsFlying, "Enable Hulk Mode");
            Features.OnePunchEnabled = UI.Toggle(Features.OnePunchEnabled, "Enable Super Punch");

            UI.Label("Aimbot Options");
            Aimbot.AimbotEnabled = UI.Toggle(Aimbot.AimbotEnabled, "Enable Aimbot (Left Click)");
            Aimbot.SilentAimbotEnabled = UI.Toggle(Aimbot.SilentAimbotEnabled, "Enable Silent Aimbot (Right Click)");
            Aimbot.VisibilityCheck = UI.Toggle(Aimbot.VisibilityCheck, "Require Visibility Check");
            Aimbot.DrawFovEnabled = UI.Toggle(Aimbot.DrawFovEnabled, "Draw Aimbot FOV");

            UI.Label($"Aimbot FOV: {Mathf.RoundToInt(Aimbot.FOV)}");
            Aimbot.FOV = UI.Slider(Aimbot.FOV, 10f, 500f);

            // Priority Mode Selection
            UI.Label("Priority Mode");
            if (UI.Button("Cycle Priority Mode"))
            {
                Aimbot.CyclePriority(); // Toggle through modes
            }

            // Display the current priority mode
            UI.Label($"Current Priority: {Aimbot.CurrentPriority.ToString()}");

        }

        private void ESPTab()
        {
            UI.Label("ESP Settings");

            playerEspEnabled = UI.Toggle(playerEspEnabled, "Enable Player ESP");
            npcEspEnabled = UI.Toggle(npcEspEnabled, "Enable NPC ESP");

            UI.Label("Box Style:");
            boxModeIndex = GUILayout.SelectionGrid(boxModeIndex, new string[] { "2D Box", "3D Box" }, 2);
            Features.Use3DBoxes = boxModeIndex == 1;

            UI.Label($"Max ESP Distance: {Mathf.RoundToInt(Features.MaxESPDistance)}m");
            Features.MaxESPDistance = UI.Slider(Features.MaxESPDistance, 10f, 300f);

            Features.ShowNames = UI.Toggle(Features.ShowNames, "Show Names");
            Features.ShowDistance = UI.Toggle(Features.ShowDistance, "Show Distance");

            UI.Label("Colors:");
            Features.PlayerBoxColor = UI.ColorField(Features.PlayerBoxColor);
            Features.CopBoxColor = UI.ColorField(Features.CopBoxColor);
            Features.NPCBoxColor = UI.ColorField(Features.NPCBoxColor);
        }

        private void WorldTab()
        {
            UI.Label("World Control");

            UI.Label($"Selected Time: {Mathf.RoundToInt(selectedTime)}h");
            selectedTime = UI.Slider(selectedTime, 0f, 24f);

            if (UI.Button("Set Time"))
                Features.SetTime(Mathf.RoundToInt(selectedTime));
        }

        private void PlayerTab()
        {
            UI.Label("Player List");

            var players = Features.GetPlayers();
            int maxButtons = Mathf.Min(4, players.Count);

            for (int i = 0; i < maxButtons; i++)
            {
                var player = players[i];
                if (player != null && player.transform != null)
                {
                    if (UI.Button($"[{i}] {player.name}"))
                        selectedPlayerIndex = i;
                }
            }

            if (selectedPlayerIndex >= 0 && selectedPlayerIndex < players.Count)
            {
                var selectedPlayer = players[selectedPlayerIndex];
                if (selectedPlayer != null && selectedPlayer.transform != null)
                {
                    UI.Label($"Name: {selectedPlayer.name}");
                    UI.Label($"Position: {selectedPlayer.transform.position}");

                    if (selectedPlayer.Health != null)
                        UI.Label($"Health: {selectedPlayer.Health.CurrentHealth} HP");

                    if (UI.Button("Teleport to Player"))
                        Features.LocalPlayer.transform.position = selectedPlayer.transform.position;
                }
            }
        }

        private void MiscTab()
        {
            UI.Label("Miscellaneous");

            if (UI.Button("Force Quit Game"))
                Application.Quit();

            UI.Label($"Money Amount: {Mathf.RoundToInt(moneyAmount)}");
            moneyAmount = UI.Slider(moneyAmount, 0, 1000000);
            if (UI.Button("Give Money"))
                Features.GiveMoney(moneyAmount);

            UI.Label($"XP Amount: {xpAmount}");
            xpAmount = (int)UI.Slider(xpAmount, 0, 100000);
            if (UI.Button("Give XP"))
                Features.GiveXP(xpAmount);
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
                if (godModeEnabled)
                    Features.HandleGodMode();

                if (alwaysUnwanted)
                    Features.SetWantedLvl();

                Features.SuperPunchEnabled = Features.OnePunchEnabled;

                if (Features.SuperPunchEnabled)
                    Features.HandleSuperPunch();

                if (Aimbot.AimbotEnabled || Aimbot.SilentAimbotEnabled)
                    Aimbot.HandleAimbot();

                UniversalNoclip.Update();
            }
        }
    }
}
