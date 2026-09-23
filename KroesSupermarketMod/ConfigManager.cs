using BepInEx.Configuration;
using Rewired.Integration.PlayMaker;
using System;
namespace KroesSupermarketMod
{
    internal class ConfigManager
    {
        // Config Entries
        private ConfigEntry<bool> customNpcHitNotificationCfg;
        private ConfigEntry<bool> doublePriceGunCfg;
        private ConfigEntry<bool> franchiseProgressBarCfg;
        private ConfigEntry<bool> chatCommandsCfg;
        private ConfigEntry<bool> euroSymbolCfg;
        private ConfigEntry<bool> throwBoxesCfg;
        private ConfigEntry<bool> thirdPersonCameraCfg;
        private ConfigEntry<float> thirdPersonCameraFollowDistanceCfg;
        private ConfigEntry<bool> betterMiniTransporterCfg;
        private ConfigEntry<int> MiniTransporterExtraBoxesCfg;
        private ConfigEntry<float> maxForwardSpeedCfg;
        private ConfigEntry<float> maxBackwardSpeedCfg;
        private ConfigEntry<float> accelerationRateCfg;
        private ConfigEntry<float> decelerationRateCfg;
        private ConfigEntry<float> maxRotationSpeedCfg;

        // Project references
        internal bool customNpcHitNotifications => customNpcHitNotificationCfg.Value;
        internal bool doublePriceGun => doublePriceGunCfg.Value;
        internal bool franchiseProgressBar => franchiseProgressBarCfg.Value;
        internal bool chatCommands => chatCommandsCfg.Value;
        internal bool euroSymbol => euroSymbolCfg.Value;
        internal bool throwBoxes => throwBoxesCfg.Value;
        internal bool thirdPersonCamera => thirdPersonCameraCfg.Value;
        internal float thirdPersonCameraFollowDistance => thirdPersonCameraFollowDistanceCfg.Value;
        internal bool betterMiniTransporter => betterMiniTransporterCfg.Value;
        internal int MiniTransporterExtraBoxes => MiniTransporterExtraBoxesCfg.Value;
        internal float maxForwardSpeed => maxForwardSpeedCfg.Value;
        internal float maxBackwardSpeed => maxBackwardSpeedCfg.Value;
        internal float accelerationRate => accelerationRateCfg.Value;
        internal float decelerationRate => decelerationRateCfg.Value;
        internal float maxRotationSpeed => maxRotationSpeedCfg.Value;

        // Config Manager
        public ConfigManager(ConfigFile config)
        {
            // General
            doublePriceGunCfg = config.Bind("General", "DoublePriceGun", true, "Enable double price gun feature.");
            franchiseProgressBarCfg = config.Bind("General", "FranchiseProgressBar", true, "Enable the franchise progress bar feature.");
            euroSymbolCfg = config.Bind("General", "EuroSymbol", false, "Replace dollar sign with euro.");
            
            // Third Person Camera
            thirdPersonCameraCfg = config.Bind("Third Person Camera", "Enable Third Person Camera", true, "Allow camera switching using the mouse scroll wheel.");
            thirdPersonCameraFollowDistanceCfg = config.Bind("Third Person Camera", "Camera Follow Distance", 4f, "(float) distance at which the camera follows you.");

            // Mini Transport
            betterMiniTransporterCfg = config.Bind("Mini Transporter", "BetterMiniTransporter", false, "more box capacity and driving modifications.");
            MiniTransporterExtraBoxesCfg = config.Bind("Mini Transporter", "Extra Boxes Amount", 2, "(int) amount of extra boxes to add. (max 6)");
            maxForwardSpeedCfg = config.Bind("Mini Transporter", "Max Forward Speed", 8f, "(float) max speed going forwards. (default 8)");
            maxBackwardSpeedCfg = config.Bind("Mini Transporter", "Max Backward Speed", 5f, "(float) max speed going backwards. (default 5)");
            accelerationRateCfg = config.Bind("Mini Transporter", "Acceleration Rate", 6f, "(float) how fast the transporter accelerates. (default 6)");
            decelerationRateCfg = config.Bind("Mini Transporter", "Deceleration Rate", 5f, "(float) how fast the transporter decelerates. (default 5)");
            maxRotationSpeedCfg = config.Bind("Mini Transporter", "Max Rotation Speed", 75f, "(float) max steering speed. (default 75)");

            // Experimental
            customNpcHitNotificationCfg = config.Bind("Experimental", "CustomNpcHitNotifications", false, "Enable custom voice lines (inside jokes).");
            throwBoxesCfg = config.Bind("Experimental", "ThrowBoxes", false, "Throw boxes instead of dropping.");
            chatCommandsCfg = config.Bind("Experimental", "ChatCommands", false, "Enable chat commands feature.");
        }

    }
}
