using BepInEx.Configuration;
using Rewired.Integration.PlayMaker;
using System;
namespace KroesSupermarketMod
{
    internal class ConfigManager
    {
        private ConfigEntry<bool> customNpcHitNotificationCfg;
        private ConfigEntry<bool> doublePriceGunCfg;
        private ConfigEntry<bool> franchiseProgressBarCfg;
        private ConfigEntry<bool> chatCommandsCfg;
        private ConfigEntry<bool> euroSymbolCfg;
        private ConfigEntry<bool> throwBoxesCfg;
        private ConfigEntry<bool> thirdPersonCameraCfg;
        internal bool customNpcHitNotifications => customNpcHitNotificationCfg.Value;
        internal bool doublePriceGun => doublePriceGunCfg.Value;
        internal bool franchiseProgressBar => franchiseProgressBarCfg.Value;
        internal bool chatCommands => chatCommandsCfg.Value;
        internal bool euroSymbol => euroSymbolCfg.Value;
        internal bool throwBoxes => throwBoxesCfg.Value;
        internal bool thirdPersonCamera => thirdPersonCameraCfg.Value;
        public ConfigManager(ConfigFile config)
        {
            customNpcHitNotificationCfg = config.Bind("General", "CustomNpcHitNotifications", false, "Enable custom voice lines (inside jokes).");
            doublePriceGunCfg = config.Bind("General", "DoublePriceGun", true, "Enable double price gun feature.");
            franchiseProgressBarCfg = config.Bind("General", "FranchiseProgressBar", true, "Enable the franchise progress bar feature.");
            chatCommandsCfg = config.Bind("General", "ChatCommands", true, "Enable chat commands feature.");
            euroSymbolCfg = config.Bind("General", "EuroSymbol", false, "Replace dollar sign with euro.");
            throwBoxesCfg = config.Bind("General", "ThrowBoxes", true, "Throw boxes instead of dropping.");
            thirdPersonCameraCfg = config.Bind("General", "ThirdPersonCamera", true, "Allow camera switching with mouse scroll wheel.");
        }

    }
}
