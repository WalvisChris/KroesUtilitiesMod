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


        //// new values
        //private static readonly Vector2 xLimits = new Vector2(-999f, 999f);
        //private static readonly Vector2 zLimits = new Vector2(-999f, 999f);
        //private static readonly float yLimit = 999f;

        //// game references
        //private static readonly AccessTools.FieldRef<Builder_Decoration, float> Builder_Decoration_float_maxY = AccessTools.FieldRefAccess<Builder_Decoration, float>("maxY");
        //private static readonly AccessTools.FieldRef<Builder_Main, float> Builder_Main_float_maxY = AccessTools.FieldRefAccess<Builder_Main, float>("maxY");

        //// patches

        //[HarmonyPatch(typeof(Builder_Decoration), "InCorrectBounds")]
        //[HarmonyPrefix]
        //private static void BuilderDecorations_Prefix(Builder_Decoration __instance)
        //{
        //    // return position.x > -15f && position.x < 37f && position.z > -8.5f && position.z < 48f && position.y > this.minY && position.y < this.maxY;
        //    Builder_Decoration_float_maxY(__instance) = yLimit;
        //}

        //[HarmonyPatch(typeof(Builder_Main), "InCorrectBounds")]
        //[HarmonyPrefix]
        //private static void BuilderMain_Prefix(Builder_Main __instance)
        //{
        //    // return position.x > this.buildXLimits.x && position.x < this.buildXLimits.y && position.z > this.buildZLimits.x && position.z < this.buildZLimits.y && position.y > this.minY && position.y < this.maxY;
        //    Builder_Main_float_maxY(__instance) = yLimit;
        //    __instance.buildXLimits = xLimits;
        //    __instance.buildZLimits = zLimits;
        //}
    }
}
