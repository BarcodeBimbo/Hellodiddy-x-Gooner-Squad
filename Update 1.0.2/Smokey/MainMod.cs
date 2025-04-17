using HelloDiddy.Mod;
using HelloDiddy.Mod.Features;
using HelloDiddy.Mod.Functions;
using HelloDiddy.Mod.Patches;
using HelloDiddy.Mod.UI;
using HelloDiddy.Mod.UI.Config;
using HelloDiddy.Mod.Utils;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;

namespace HelloDiddy
{
    public class MainMod : MelonMod
    {
        [Obsolete]
        public override void OnApplicationStart()
        {
            new MainModState();
            Config.Init();
            MelonLogger.Msg("Initializing HelloDiddy x GoonerSquad");
            UI.LoadBackground();
            Initialize.CustomPatches(new HarmonyLib.Harmony("A51"));
        }

        public override void OnInitializeMelon()
        {
            Settings.windowRect = new Rect(20, 20, 450, 500);

            MainModState.Instance.SpawnerItems = Utils.LoadItemsFromJson();
            if (MainModState.Instance.SpawnerItems.Count > 0)
            {
                MainModState.Instance.SelectedCategory = MainModState.Instance.SpawnerItems.Keys.First();
                MelonLogger.Msg($"Spawner items loaded: {MainModState.Instance.SpawnerItems.Count} categories.");
            }
            else
            {
                MelonLogger.Error("No spawner items loaded.");
            }
        }

        public override void OnApplicationQuit()
        {
            Config.Save();
            base.OnApplicationQuit();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MainModState.IsMainScene = Utils.CheckMainScene(sceneName, false);
            if (MainModState.Instance.canvasObject == null)
                UI.CreateBackgroundCanvas();
            else
                MainModState.Instance.canvasObject.SetActive(true);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MainModState.IsMainScene = Utils.CheckMainScene(sceneName, true);
            if (MainModState.Instance.canvasObject != null)
                MainModState.Instance.canvasObject.SetActive(false);
        }

        public override void OnGUI()
        {
            if (MainModState.Instance.backgroundImage != null)
                MainModState.Instance.backgroundImage.enabled = Settings.showUI;

            if (Settings.showUI)
                DrawUI();
            else if (MainModState.Instance.backgroundImage != null)
                MainModState.Instance.backgroundImage.enabled = false;

            if (MainModState.IsMainScene)
            {
                DrawESP();
                AimbotVisual.DrawFOV();
                DrawBonesESP();
            }
        }

        private void DrawUI()
        {
            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.85f);
            Settings.windowRect = GUI.Window(Settings.windowID, Settings.windowRect, (GUI.WindowFunction)DrawWindow, "HelloDiddy x GoonerSquad");

            if (MainModState.Instance.backgroundImage != null)
            {
                var rectTransform = MainModState.Instance.backgroundImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(Settings.windowRect.width, Settings.windowRect.height);
                rectTransform.position = new Vector2(Settings.windowRect.x + Settings.windowRect.width / 2, Screen.height - Settings.windowRect.y - Settings.windowRect.height / 2);
            }
        }

        private void DrawESP()
        {
            if (MainModState.Instance.PlayerEspEnabled)
                MainModState.Instance.Esp.DrawESPPlayers(Features.GetPlayers());
            if (MainModState.Instance.NpcEspEnabled)
                MainModState.Instance.Esp.DrawESPNPC(Features.GetNPCS());
        }

        private void DrawBonesESP()
        {
            if (!MainModState.BonesESPEnabled) return;

            var players = Features.GetPlayers();
            var npcs = Features.GetNPCS();

            foreach (var player in players)
            {
                if (player?.Avatar?.BodyContainer != null)
                    ESP.DrawBonesESP(player.Avatar.BodyContainer, MainModState.BonesESPColor);
                else if (player?.transform != null)
                    ESP.DrawBonesESP(player.transform, MainModState.BonesESPColor);
            }

            foreach (var npc in npcs)
            {
                if (npc?.Avatar?.BodyContainer != null)
                    ESP.DrawBonesESP(npc.Avatar.BodyContainer, MainModState.BonesESPColor);
                else if (npc?.transform != null)
                    ESP.DrawBonesESP(npc.transform, MainModState.BonesESPColor);
            }
        }

        private void DrawWindow(int id)
        {
            MainModState.Instance.scrollPos = GUILayout.BeginScrollView(MainModState.Instance.scrollPos, GUILayout.Width(Settings.windowRect.width - 20), GUILayout.Height(Settings.windowRect.height - 40));
            UI.BeginVertical();
            DrawTabHeaders();
            GUILayout.Space(10);
            DrawSelectedTab();
            UI.EndVertical();
            GUILayout.EndScrollView();
            DrawFooter();
            GUI.DragWindow();
        }

        private void DrawTabHeaders()
        {
            UI.BeginHorizontal();
            for (int i = 0; i < MainModState.Instance.TabNames.Length; i++)
            {
                if (UI.Button(MainModState.Instance.TabNames[i]))
                    MainModState.Instance.CurrentTab = i;
            }
            UI.EndHorizontal();
        }

        private void DrawSelectedTab()
        {
            switch (MainModState.Instance.CurrentTab)
            {
                case 0: TabSystem.DrawPlayerTab(); break;
                case 1: TabSystem.DrawSelfTab(); break;
                case 2: TabSystem.DrawAimbotEspTab(); break;
                case 3: TabSystem.DrawSpawnerTab(); break;
                case 4: TabSystem.DrawWorldTab(); break;
                case 5: TabSystem.DrawMiscTab(); break;
                default: UI.Label("Invalid Tab", UI.HeaderStyle); break;
            }
        }

        private void DrawFooter()
        {
            Rect footerRect = GUILayoutUtility.GetRect(0, 20);
            footerRect.y -= 3;
            GUI.Label(new Rect(footerRect.x + 6, footerRect.y, 200, 20), $"Mod Version: {Info.Version}");
            GUI.Label(new Rect(Settings.windowRect.width - 145, footerRect.y, 150, 20), $"Game Version: {Application.version}");
        }

        public override void OnUpdate()
        {
            HandleKeyPress();
            UpdateCanvasFade();
            RunMainSceneFeatures();
        }

        private void HandleKeyPress()
        {
            if (Input.GetKeyDown(MainModState.MenuKeyPreference.Value))
            {
                Settings.showUI = !Settings.showUI;
                if (MainModState.IsMainScene)
                {
                    Cursor.visible = Settings.showUI;
                    Cursor.lockState = Settings.showUI ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
        }

        private void UpdateCanvasFade()
        {
            if (MainModState.Instance.canvasGroup != null)
            {
                float targetAlpha = Settings.showUI ? 1f : 0f;
                MainModState.Instance.canvasGroup.alpha = Mathf.Lerp(MainModState.Instance.canvasGroup.alpha, targetAlpha, Time.deltaTime * MainModState.Instance.fadeSpeed);
            }
        }

        private void RunMainSceneFeatures()
        {
            if (!MainModState.IsMainScene) return;

            if (MainModState.Instance.GodModeEnabled) Features.HandleGodMode();
            if (MainModState.Instance.AlwaysUnwanted) Features.NeverWanted();
            if (MainModState.SuperPunchEnabled) Features.HandleSuperPunch();
            if (MainModState.UnfairAimbotEnabled) Features.HandleUnfairAimbot();
            if (MainModState.Instance.InfiniteEnergy) Features.HandleInfiniteEnergy();
            if (MainModState.Instance.InfiniteStamina) Features.HandleInfiniteStamina();

            if (!MainModState.Instance.StackPatchEnabled && MainModState.Instance.ModdedStackSize != 20)
                MainModState.Instance.ModdedStackSize = 20;

            if (MainModState.AutoGrowEnabled)
            {
                MainModState.Instance.soilWaterTimer += Time.deltaTime;
                if (MainModState.Instance.soilWaterTimer >= MainModState.SoilWaterInterval)
                {
                    MainModState.Instance.soilWaterTimer = 0f;
                    Features.AutoGrowAfterPlant();
                }
            }

            if (MainModState.AimbotEnabled || MainModState.SilentAimbotEnabled)
                Aimbot.HandleAimbot();

            Noclip.Update();
        }
    }
}
