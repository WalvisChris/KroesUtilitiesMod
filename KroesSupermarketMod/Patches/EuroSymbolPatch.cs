using HarmonyLib;
using Steamworks;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class EuroSymbolPatch
    {
        [HarmonyPatch(typeof(ProductListing), "ConvertFloatToTextPrice")]
        [HarmonyPrefix]
        public static bool Prefix(float price, ref string __result)
        {
            __result = "€" + price.ToString("F2", new System.Globalization.CultureInfo("nl-NL"));
            return false;
        }
    }
}
