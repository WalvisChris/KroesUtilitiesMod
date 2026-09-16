using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class FranchiseBarPatch
    {
        private static int lastKnownExp = -1;
        private static Slider franchiseProgressBar;
        
        [HarmonyPatch(typeof(GameData), "Update")]
        [HarmonyPostfix]
        public static void Update_Postfix(GameData __instance, int ___gameFranchiseExperience)
        {
            if (lastKnownExp == -1)
            {
                lastKnownExp = ___gameFranchiseExperience;
                return;
            }

            if (___gameFranchiseExperience != lastKnownExp)
            {
                int oldExp = lastKnownExp;
                lastKnownExp = ___gameFranchiseExperience;
                AccessTools.Method(typeof(GameData), "UpdateFranchisePoints")?.Invoke(__instance, new object[] { oldExp, ___gameFranchiseExperience });
            }
        }

        [HarmonyPatch(typeof(GameData), "UpdateFranchisePoints")]
        [HarmonyPostfix]
        public static void UpdateFranchisePoints_Postfix(GameData __instance, TextMeshProUGUI ___UIFranchisePointsOBJ, int ___gameFranchiseExperience)
        {
            if (___UIFranchisePointsOBJ == null) return;

            Utilities.EnsureProgressBarCreated(___UIFranchisePointsOBJ, ref franchiseProgressBar);

            float progressNormalized = Utilities.GetLevelProgress(___gameFranchiseExperience);

            if (franchiseProgressBar != null) franchiseProgressBar.value = Mathf.Clamp01(progressNormalized);
        }
    }
}
