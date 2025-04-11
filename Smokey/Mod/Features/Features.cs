using Il2CppScheduleOne.Audio;
using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using Smokey.Mod.Functions;

namespace Smokey.Mod.Features
{
    public static class Features
    {
        public static bool undetectedbool = false;
        public static Player LocalPlayer => Player.Local;

        private static float lastHealth = -1f;
        private const float GodModeHealthValue = float.MaxValue;

        public static bool Use3DBoxes = true;
        public static float MaxESPDistance = 75f;
        public static bool ShowNames = true;
        public static bool ShowDistance = true;

        public static Color PlayerBoxColor = new Color(0.6f, 0.4f, 1.0f);
        public static Color CopBoxColor = new Color(0.3f, 0.6f, 1.0f);
        public static Color NPCBoxColor = new Color(1.0f, 0.8f, 0.2f);

        public static bool SuperPunchEnabled = false;
        public static bool OnePunchEnabled = false;

        public static MusicTrack musicTrack = new MusicTrack();
        public static AudioSourceController audioSource => audioSource;
        public static CombatManager combatManager => combatManager;
        public static void HandleGodMode()
        {
            if (LocalPlayer == null || LocalPlayer.Health == null)
                return;

            float currentHealth = LocalPlayer.Health.CurrentHealth;
            if (lastHealth == -1f)
                lastHealth = currentHealth;
            if (currentHealth < lastHealth)
                SetGodMode(GodModeHealthValue);
            lastHealth = LocalPlayer.Health.CurrentHealth;
        }

        public static void HandleNoclip()
        {
            UniversalNoclip.Update();
        }

        public static void ToggleNoclip()
        {
            UniversalNoclip.IsFlying = !UniversalNoclip.IsFlying;
            MelonLogger.Msg(UniversalNoclip.IsFlying ? "Noclip Enabled" : "Noclip Disabled");
        }

        public static void HandleSuperPunch()
        {
            if (!OnePunchEnabled || LocalPlayer == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 playerPos = LocalPlayer.transform.position;
                foreach (var player in GetPlayers())
                {
                    if (player == null || player == LocalPlayer || player.transform == null)
                        continue;

                    float distance = Vector3.Distance(playerPos, player.transform.position);
                    if (distance < 3f && player.Health != null)
                        player.Health.SetHealth(0f);
                }
                foreach (var npc in GetNPCS())
                {
                    if (npc == null || npc.transform == null || npc.isInBuilding)
                        continue;

                    float distance = Vector3.Distance(playerPos, npc.transform.position);
                    if (distance < 3f && npc.Health != null)
                        npc.Health.KnockOut();
                }
            }
        }

        public static void SetGodMode(float healthValue)
        {
            if (LocalPlayer != null && LocalPlayer.Health != null)
                LocalPlayer.Health.SetHealth(healthValue);
        }

        public static void ForceSleep()
        {
            TimeManager.instance.StartSleep();
        }
        public static void ChangeStamina()
        {
            PlayerMovement.instance.ChangeStamina(100);
        }

        public static void NeverWanted()
        {
            LocalPlayer.CrimeData.SetPursuitLevel(PlayerCrimeData.EPursuitLevel.None);
        }

        public static void ChangeEnergy()
        {
            Player.Local.Energy.RestoreEnergy();
        }

        public static void GiveMoney(float moneyValue)
        {
            MoneyManager.Instance?.ChangeCashBalance(moneyValue);
        }

        public static void GiveXP(int xpValue)
        {
            DailySummary.Instance?.AddXP(xpValue);
        }

        public static void SetTime(float time)
        {
            TimeManager.Instance?.SetTime((int)(time * 100));
        }

        public static Il2CppSystem.Collections.Generic.List<Player> GetPlayers()
        {
            return Player.PlayerList;
        }

        public static Il2CppSystem.Collections.Generic.List<NPC> GetNPCS()
        {
            return NPCManager.NPCRegistry;
        }

    }
}
