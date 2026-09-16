using HarmonyLib;
using System.Globalization;
using TMPro;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class PriceGunPatch
    {
        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPostfix]
        public static void Postfix(ref float ___pPrice, TextMeshProUGUI ___marketPriceTMP, TextMeshProUGUI ___yourPriceTMP)
        {
            if (___marketPriceTMP == null || string.IsNullOrEmpty(___marketPriceTMP.text)) return;
            if (float.TryParse(___marketPriceTMP.text.Substring(1).Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float marketPrice))
            {
                string currencySign = "$";
                var culture = Plugin.config.euroSymbol ? new CultureInfo("nl-NL") : CultureInfo.InvariantCulture;

                if (Plugin.config.euroSymbol)
                {
                    currencySign = "€";
                    // this.marketPriceTMP.text = "$" + num8.ToString();   <-- original game code reference
                    ___marketPriceTMP.text = currencySign + marketPrice.ToString("F2", culture);
                }

                ___pPrice = marketPrice * 2f;
                // this.yourPriceTMP.text = "$" + num9.ToString();   <-- original game code reference
                if (___yourPriceTMP != null) ___yourPriceTMP.text = currencySign + ___pPrice.ToString("F2", culture);
            }
        }
    }
}
