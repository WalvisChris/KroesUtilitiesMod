using HarmonyLib;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class BuildingLimitsPatch
    {
        [HarmonyPatch(typeof(Builder_Main), "InCorrectBounds")]
        [HarmonyPostfix]
        private static void Builder_Main_Postfix(ref bool __result)
        {
            __result = true;
        }

        [HarmonyPatch(typeof(Builder_Decoration), "InCorrectBounds")]
        [HarmonyPostfix]
        private static void Builder_Decoration_Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
