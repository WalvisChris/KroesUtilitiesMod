using HarmonyLib;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch(typeof(LayoutManager))]
    internal class LayoutManagerPatch
    {
        [HarmonyPatch("AssignReferences")]
        [HarmonyPostfix]
        public static void Postfix(LayoutManager __instance)
        {
            if (__instance.trashRecycle1 != null) Utilities.recycleObj1 = __instance.trashRecycle1.gameObject;
            if (__instance.trashRecycle2 != null) Utilities.recycleObj2 = __instance.trashRecycle2.gameObject;
            if (__instance.trashNormal != null) Utilities.trashObj = __instance.trashNormal.gameObject;
        }
    }
}
