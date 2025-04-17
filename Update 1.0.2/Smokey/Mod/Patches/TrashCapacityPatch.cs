using HarmonyLib;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.Trash;

namespace HelloDiddy.Mod.Patches
{
    [HarmonyPatch]
    public static class TrashCapacityPatch
    {
        [HarmonyPatch(typeof(Il2CppScheduleOne.Trash.TrashContent), "GetTotalSize")]
        [HarmonyPrefix]
        public static bool GetTotalSizePatch(ref int __result)
        {
            if (MainModState.Instance != null && MainModState.Instance.TrashPatchEnabled)
            {
                __result = 0;
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(Equippable_TrashGrabber), "GetCapacity")]
        [HarmonyPrefix]
        public static bool GetCapacityPatch(ref int __result)
        {
            if (MainModState.Instance.TrashPlusPlusPatch == true)
            {
                __result = int.MaxValue;
                return false;
            }
            return true;
        }
        
        [HarmonyPatch(typeof(Equippable_TrashGrabber), "RefreshVisuals")]
        [HarmonyPostfix]
        public static void RefreshVisualsPatch(Equippable_TrashGrabber __instance)
        {
            if (MainModState.Instance.TrashPlusPlusPatch != true) return;

            if (__instance.TrashContent == null || __instance.TrashContent_Min == null) return;

            __instance.TrashContent.localScale = __instance.TrashContent_Min.localScale;
        }
    }
}
