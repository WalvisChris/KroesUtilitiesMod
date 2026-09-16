using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using KroesSupermarketMod.Patches;

namespace KroesSupermarketMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.chris.kroes.supermarket";
        private const string NAME = "Kroes Supermarket";
        private const string VERSION = "1.0.7";

        internal static ManualLogSource mls;
        internal static ConfigManager config;
        internal static Harmony harmony = new Harmony(GUID);
        internal static Plugin instance;

        private void Awake()
        {
            if (instance == null) instance = this;

            // logging
            mls = Logger;

            // config
            config = new ConfigManager(Config);
            if (config.customNpcHitNotifications) harmony.PatchAll(typeof(NpcNotificationPatch));
            if (config.doublePriceGun) harmony.PatchAll(typeof(PriceGunPatch));
            if (config.franchiseProgressBar) harmony.PatchAll(typeof(FranchiseBarPatch));
            if (config.chatCommands) harmony.PatchAll(typeof(ChatCommandsPatch));
            if (config.euroSymbol) harmony.PatchAll(typeof(EuroSymbolPatch));
            if (config.throwBoxes) harmony.PatchAll(typeof(ThrowableBoxesPatch));

            // finish loading
            mls.LogInfo("Loaded succesfully");
        }
    }
}
