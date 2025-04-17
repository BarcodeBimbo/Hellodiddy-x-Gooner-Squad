using MelonLoader;
using System;
using UnityEngine;

namespace HelloDiddy.Mod.UI.Config
{
    public static class Config
    {
        private static MelonPreferences_Category category;

        public static void Init()
        {
            category = MelonPreferences.CreateCategory("HelloDiddy", "HelloDiddy Settings");

            // Keybinds
            MainModState.MenuKeyPreference = category.CreateEntry("MenuKey", KeyCode.Insert, "Menu Key", "Which key toggles the menu?");
            MainModState.NoclipKeyPrimary = category.CreateEntry("NoclipKeyPrimary", KeyCode.LeftShift, "Noclip Modifier Key");
            MainModState.NoclipKeySecondary = category.CreateEntry("NoclipKeySecondary", KeyCode.F, "Noclip Toggle Key");

            // Aimbot + ESP Toggles
            MainModState.AimbotEnabled = category.CreateEntry("AimbotEnabled", false, "Enable Aimbot").Value;
            MainModState.AimSmoothingEnabled = category.CreateEntry("AimSmoothingEnabled", false, "Enable Smoothing").Value;
            MainModState.UnfairAimbotEnabled = category.CreateEntry("UnfairAimbotEnabled", false, "Unfair Aimbot").Value;
            MainModState.TriggerBotEnabled = category.CreateEntry("TriggerBotEnabled", false, "Trigger Bot").Value;
            MainModState.DrawFovEnabled = category.CreateEntry("DrawFovEnabled", true, "Draw FOV Circle").Value;
            MainModState.EnablePrediction = category.CreateEntry("EnablePrediction", false, "Enable Prediction").Value;
            MainModState.ExplosiveBulletsEnabled = category.CreateEntry("ExplosiveBulletsEnabled", false, "Explosive Bullets").Value;

            // Aimbot Sliders
            MainModState.FOV = category.CreateEntry("FOV", 30f, "Aimbot Field of View").Value;
            MainModState.BulletSpeed = category.CreateEntry("BulletSpeed", 100f, "Bullet Speed").Value;
            MainModState.PredictionFactor = category.CreateEntry("PredictionFactor", 1f, "Prediction Factor").Value;
            MainModState.SmoothFactor = category.CreateEntry("SmoothFactor", 10f, "Smooth Factor").Value;
            MainModState.NPCSmoothFactor = category.CreateEntry("NPCSmoothFactor", 10f, "NPC Smooth Factor").Value;
            MainModState.AimInaccuracy = category.CreateEntry("AimInaccuracy", 0f, "Aim Inaccuracy").Value;
            MainModState.UnfairAimbotDistance = category.CreateEntry("UnfairAimbotDistance", 50f, "Unfair Aimbot Distance").Value;

            // ESP Toggles
            MainModState.Instance.PlayerEspEnabled = category.CreateEntry("PlayerEspEnabled", true, "Player ESP").Value;
            MainModState.Instance.NpcEspEnabled = category.CreateEntry("NpcEspEnabled", true, "NPC ESP").Value;
            MainModState.ShowNames = category.CreateEntry("ShowNames", true, "Show Names").Value;
            MainModState.ShowDistance = category.CreateEntry("ShowDistance", true, "Show Distance").Value;
            MainModState.MaxESPDistance = category.CreateEntry("MaxESPDistance", 75f, "Max ESP Distance").Value;

            // ESP Visuals
            MainModState.PlayerBoxColor = category.CreateEntry("PlayerBoxColor", Color.green, "Player ESP Color").Value;
            MainModState.NPCBoxColor = category.CreateEntry("NPCBoxColor", Color.yellow, "NPC ESP Color").Value;
            MainModState.CopBoxColor = category.CreateEntry("CopBoxColor", Color.blue, "Cop ESP Color").Value;
            MainModState.Instance.BoxModeIndex = category.CreateEntry("BoxModeIndex", 0, "ESP Box Type").Value;
            MainModState.Use3DBoxes = MainModState.Instance.BoxModeIndex == 1;

            // Self Options / Player Boosts
            MainModState.Instance.GodModeEnabled = category.CreateEntry("GodModeEnabled", false, "God Mode").Value;
            MainModState.Instance.UnlimitedAmmoEnabled = category.CreateEntry("UnlimitedAmmoEnabled", false, "Unlimited Ammo").Value;
            MainModState.Instance.InfiniteEnergy = category.CreateEntry("InfiniteEnergy", false, "Infinite Energy").Value;
            MainModState.Instance.InfiniteStamina = category.CreateEntry("InfiniteStamina", false, "Infinite Stamina").Value;
            MainModState.Instance.AlwaysUnwanted = category.CreateEntry("AlwaysUnwanted", false, "Always Unwanted").Value;

            MainModState.Instance.NoRecoilEnabled = category.CreateEntry("NoRecoilEnabled", false, "No Recoil").Value;
            MainModState.Instance.NoSpreadEnabled = category.CreateEntry("NoSpreadEnabled", false, "No Spread").Value;
            MainModState.Instance.RapidFireEnabled = category.CreateEntry("RapidFireEnabled", false, "Rapid Fire").Value;
            MainModState.Instance.SuperImpactEnabled = category.CreateEntry("SuperImpactEnabled", false, "Super Impact").Value;
            MainModState.Instance.OneShotKillEnabled = category.CreateEntry("OneShotKillEnabled", false, "One Shot Kill").Value;

            // Items / Stack Tweaks
            MainModState.AutoGrowEnabled = category.CreateEntry("AutoGrowEnabled", false, "Auto Grow").Value;
            MainModState.Instance.TrashPlusPlusPatch = category.CreateEntry("TrashPlusPlusPatch", false, "Trash Picker++").Value;
            MainModState.Instance.TrashPatchEnabled = category.CreateEntry("TrashPatchEnabled", false, "Unlimited Trash").Value;
            MainModState.Instance.DupPatchEnabled = category.CreateEntry("DupPatchEnabled", false, "Dup Patch").Value;
            MainModState.Instance.DuplicationMultiplier = category.CreateEntry("DuplicationMultiplier", 2, "Duplication Multiplier").Value;
            MainModState.Instance.StackPatchEnabled = category.CreateEntry("StackPatchEnabled", false, "Stack Patch").Value;
            MainModState.Instance.ModdedStackSize = category.CreateEntry("ModdedStackSize", 20, "Custom Stack Size").Value;

            // Movement & Fun
            MainModState.Instance.JumpMultiplier = category.CreateEntry("JumpMultiplier", 1f, "Jump Multiplier").Value;
            MainModState.Instance.SpeedMultiplier = category.CreateEntry("SpeedMultiplier", 1f, "Speed Multiplier").Value;
            MainModState.SuperPunchEnabled = category.CreateEntry("SuperPunchEnabled", false, "Super Punch").Value;
            MainModState.SuperPunchDist = category.CreateEntry("SuperPunchDist", 10f, "Super Punch Distance").Value;
            MainModState.Instance.MoneyAmount = category.CreateEntry("MoneyAmount", 1000f, "Money Amount").Value;
        }

        public static void Save()
        {
            category?.SaveToFile();
        }

        public static Color UpdateAndSave(Color currentValue, Color newValue, Action<Color> apply, string label = null)
        {
            if (!ColorsEqual(currentValue, newValue))
            {
                apply(newValue);
                Save();
                if (!string.IsNullOrEmpty(label))
                    MelonLogger.Msg($"[Config Updated] {label}: {ColorUtility.ToHtmlStringRGBA(newValue)}");
            }

            return newValue;
        }

        private static bool ColorsEqual(Color a, Color b)
        {
            return Mathf.Approximately(a.r, b.r) &&
                   Mathf.Approximately(a.g, b.g) &&
                   Mathf.Approximately(a.b, b.b) &&
                   Mathf.Approximately(a.a, b.a);
        }

        public static T UpdateAndSave<T>(T currentValue, T newValue, Action<T> apply, string label = null) where T : IEquatable<T>
        {
            if (!currentValue.Equals(newValue))
            {
                apply(newValue);
                Save(); 
                if (!string.IsNullOrEmpty(label))
                    MelonLogger.Msg($"[Config Updated] {label}: {newValue}");
            }

            return newValue;
        }
    }
}
