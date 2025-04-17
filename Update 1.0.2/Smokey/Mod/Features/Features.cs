using HelloDiddy.Mod.Functions;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using System;
using UnityEngine;

namespace HelloDiddy.Mod.Features
{
    public static class Features
    {
        #region Player Modifiers
        public static void HandleGodMode()
        {
            if (Player.Local == null || Player.Local.Health == null)
                return;

            float currentHealth = Player.Local.Health.CurrentHealth;
            if (MainModState.lastHealth < 0f)
                MainModState.lastHealth = currentHealth;
            if (currentHealth < MainModState.lastHealth)
                SetGodMode(MainModState.GodModeHealthValue);
            MainModState.lastHealth = Player.Local.Health.CurrentHealth;
        }

        public static void SetGodMode(float healthValue)
        {
            if (Player.Local != null && Player.Local.Health != null)
                Player.Local.Health.SetHealth(healthValue);
        }

        public static void HandleSuperPunch()
        {
            if (!MainModState.SuperPunchEnabled || Player.Local == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 playerPos = Player.Local.transform.position;
                foreach (var player in GetPlayers())
                {
                    if (player == null || player == Player.Local || player.transform == null)
                        continue;
                    float distance = Vector3.Distance(playerPos, player.transform.position);
                    if (distance < MainModState.SuperPunchDist && player.Health != null)
                        player.Health.SetHealth(0f);
                }
                foreach (var npc in GetNPCS())
                {
                    if (npc == null || npc.transform == null || npc.isInBuilding)
                        continue;
                    float distance = Vector3.Distance(playerPos, npc.transform.position);
                    if (distance < MainModState.SuperPunchDist && npc.Health != null)
                        npc.Health.KnockOut();
                }
            }
        }
        #endregion

        #region Movement & Energy/Stamina
        public static void HandleInfiniteEnergy()
        {
            if (Player.Local == null || Player.Local.Energy == null)
                return;

            Player.Local.Energy.RestoreEnergy();
        }

        public static void HandleInfiniteStamina()
        {
            if (Player.Local == null || PlayerMovement.instance == null)
                return;
           
            PlayerMovement.instance.SetStamina(100);
        }

        public static void NeverWanted()
        {
            if (Player.Local == null || PlayerMovement.instance == null)
                return;
            
            Player.Local.CrimeData.SetPursuitLevel(PlayerCrimeData.EPursuitLevel.None);
        }
        #endregion

        #region Noclip and Aimbot
  
        public static void HandleUnfairAimbot()
        {
            if (!MainModState.UnfairAimbotEnabled || Player.Local == null)
                return;

            if (!Input.GetMouseButtonDown(0))
                return;

            Camera cam = Camera.main;
            if (cam == null)
                return;

            Vector3 localPlayerPos = Player.Local.transform.position;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            void TryApplyAction(Transform targetTransform, Vector3 targetPos, Action applyAction)
            {
                if (targetTransform == null)
                    return;

                Vector3 headPos = targetPos + Vector3.up * 1.6f;
                Vector3 screenPos = cam.WorldToScreenPoint(headPos);
                if (screenPos.z <= 0)
                    return;

                Vector2 guiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
                if (Vector2.Distance(guiPos, screenCenter) > MainModState.FOV)
                    return;

                if (Vector3.Distance(localPlayerPos, targetPos) > MainModState.UnfairAimbotDistance)
                    return;

                applyAction();
            }

            foreach (var player in Player.PlayerList)
            {
                if (player == null || player == Player.Local || player.transform == null)
                    continue;

                TryApplyAction(
                    player.transform,
                    player.transform.position,
                    () => player.Health.CurrentHealth = 0f
                );
            }

            foreach (var npc in NPCManager.NPCRegistry)
            {
                if (npc == null || npc.transform == null || !Player.Local.IsHost)
                    continue;

                TryApplyAction(
                    npc.transform,
                    npc.transform.position,
                    () => npc.Health.KnockOut()
                );
            }
        }

        #endregion

        #region Auto Grow
        public static void AutoGrowAfterPlant()
        {
            try
            {
                if (!MainModState.AutoGrowEnabled)
                {
                    MainModState.SelectedGrowSeedIndex = 0;
                    return;
                }
                string seed = MainModState.AutoGrowSeeds[MainModState.SelectedGrowSeedIndex];
                if (string.IsNullOrWhiteSpace(seed))
                    return;

                Pot[] pots = UnityEngine.Object.FindObjectsOfType<Pot>();
                if (pots == null || pots.Length == 0)
                    return;

                foreach (Pot pot in pots)
                {
                    if (pot == null)
                        continue;

                    string harvestReason;
                    if (pot.IsReadyForHarvest(out harvestReason))
                    {
                        if (!MainModState.readyHarvestNotified.ContainsKey(pot))
                            MainModState.readyHarvestNotified[pot] = true;
                        continue;
                    }
                    else if (MainModState.readyHarvestNotified.ContainsKey(pot))
                    {
                        MainModState.readyHarvestNotified.Remove(pot);
                    }
                    pot.SoilCapacity = 20f;
                    if (pot.SoilLevel < 20f)
                    {
                        pot.SoilLevel = 20f;
                        pot.PushSoilDataToServer();
                    }

                    pot.WaterCapacity = 5f;
                    if (pot.WaterLevel < 4.9f)
                    {
                        pot.WaterLevel = 4.9f;
                        pot.PushWaterDataToServer();
                    }

                    string canAcceptReason;
                    if (pot.CanAcceptSeed(out canAcceptReason))
                    {
                        pot.SendPlantSeed(seed, 100f, 100f, 100f);
                        pot.GrowSpeedMultiplier = 999f;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("AutoGrowAfterPlant error: " + ex);
            }
        }
        #endregion

        #region Misc Game Functions
        public static void ForceSleep() => TimeManager.instance.StartSleep();
    
        public static void GiveMoney(float moneyValue) => MoneyManager.Instance?.ChangeCashBalance(moneyValue);

        public static void GiveXP(int xpValue) => DailySummary.Instance?.AddXP(xpValue);

        public static void SetTime(float time) => TimeManager.Instance?.SetTime((int)(time * 100));

        #endregion

        #region Player and NPC Lists
        public static Il2CppSystem.Collections.Generic.List<Player> GetPlayers() => Player.PlayerList;
    
        public static Il2CppSystem.Collections.Generic.List<NPC> GetNPCS() => NPCManager.NPCRegistry;
    
        #endregion
    }
}
