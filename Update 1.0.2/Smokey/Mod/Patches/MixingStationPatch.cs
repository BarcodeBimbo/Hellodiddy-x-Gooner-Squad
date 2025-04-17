namespace HelloDiddy.Mod.Patches
{
  
//TODO: WIP
 /*[HarmonyPatch(typeof(MixingStation), "StartButtonClicked")]
    public static class NoMixTimePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(MixingStation __instance, RaycastHit hit)
        {
            if (MainModState.Instance.isDoneMixing && MainModState.IsMainScene)
            {

                MelonLogger.Msg("InstantMixingButtonPatch triggered: Forcing instant mix completion.");
                __instance.MixingStart();
                __instance.MixingDone_Networked();

                return false;
            }
            return true;
          
        }      
    }*/
}
