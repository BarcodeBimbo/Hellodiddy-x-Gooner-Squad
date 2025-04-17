using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using UnityEngine;
using Smokey;
using MelonLoader;
using System;

namespace Smokey.Mod.Patches
{
   public class Initialize
    {
        public static void CustomPatches(HarmonyLib.Harmony harmony)
        {
            try
            {
                harmony.PatchAll();
                MelonLogger.Msg("Debug Mode Patched!");
                MelonLogger.Msg("Stack Limit Patched!");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Patch Error: " + ex);
            }
        }
    }

    [HarmonyPatch]
    public static class DebugPatch
    {
        [HarmonyPatch(typeof(Debug), "get_isDebugBuild")]
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    [HarmonyPatch]
    public class StackLimitPatch
    {
        [HarmonyPatch(typeof(ItemInstance), "StackLimit", MethodType.Getter)]
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            if (MainMod.Instance != null)
            {
                __result = MainMod.Instance.ModdedStackSize;
            }
        }
    }
}
