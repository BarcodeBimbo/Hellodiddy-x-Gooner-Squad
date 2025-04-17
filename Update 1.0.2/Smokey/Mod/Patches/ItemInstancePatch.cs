using HarmonyLib;
using Il2CppScheduleOne.ItemFramework;

namespace HelloDiddy.Mod.Patches
{
    [HarmonyPatch(typeof(ItemInstance), nameof(ItemInstance.SetQuantity))]
    public static class ItemInstanceSetQuantityPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ItemInstance __instance, ref int quantity)
        {
            if (MainModState.Instance?.DupPatchEnabled == true)
            {
                int currentQuantity = __instance.Quantity;
                if (quantity > currentQuantity)
                {
                    int diff = quantity - currentQuantity;
                    quantity = currentQuantity + diff * MainModState.Instance.DuplicationMultiplier;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ItemInstance), nameof(ItemInstance.ChangeQuantity))]
    public static class ItemInstanceChangeQuantityPatch 
    {
        [HarmonyPrefix]
        public static void Prefix(ref int change)
        {
            if (MainModState.Instance?.DupPatchEnabled == true && change > 0)
            {
                change *= MainModState.Instance.DuplicationMultiplier;
            }
        }
    }

 
    [HarmonyPatch(typeof(ItemInstance), "StackLimit", MethodType.Getter)]
    public static class ItemInstanceStackLimitPatch 
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            if (MainModState.Instance?.StackPatchEnabled == true)
            {
                __result = MainModState.Instance.ModdedStackSize;
            }
        }
    }
}
