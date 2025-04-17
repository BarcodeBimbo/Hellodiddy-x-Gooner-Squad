using HarmonyLib;
using UnityEngine;

namespace HelloDiddy.Mod.Patches
{
    [HarmonyPatch(typeof(Debug), nameof(Debug.isDebugBuild), MethodType.Getter)]
    public static class DebugPatch
    {
        [HarmonyPostfix]
        public static void AlwaysDebug(ref bool __result) => __result = true;
    }
}
