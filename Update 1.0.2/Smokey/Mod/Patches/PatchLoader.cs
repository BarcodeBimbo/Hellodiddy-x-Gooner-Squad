using MelonLoader;
using System;

namespace HelloDiddy.Mod.Patches
{
    public static class Initialize
    {
        public static void CustomPatches(HarmonyLib.Harmony harmony)
        {
            try
            {
                var patchTypes = new[] {
                    typeof (DebugPatch),
                    typeof (PlayerMovementPatch),
                    typeof (TrashCapacityPatch),
                    typeof (ItemInstanceStackLimitPatch),
                    typeof (ItemInstanceSetQuantityPatch),
                    typeof (ItemInstanceChangeQuantityPatch),
                    typeof (UltimateAimbotPatches),

                };

                foreach (var type in patchTypes)
                {
                    harmony.PatchAll(type);
                    MelonLogger.Msg($"Patched: {type.Name} successfully");
                }            
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Patch Error: " + ex);
            }
        }
    }
}