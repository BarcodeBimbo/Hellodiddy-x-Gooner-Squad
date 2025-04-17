using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;


namespace HelloDiddy.Mod.Patches
{
    [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.SetStamina))]
    public static class PlayerMovementPatch
    {
        [HarmonyPrefix]
        public static bool PreventStaminaDrain() => !(MainModState.Instance?.InfiniteStamina ?? false);
        
    }
}
