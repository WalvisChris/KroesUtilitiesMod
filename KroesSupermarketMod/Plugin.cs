using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using KroesSupermarketMod.CustomScripts;
using KroesSupermarketMod.Patches;
using Mirror;
using System.IO;
using UnityEngine;

namespace KroesSupermarketMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.chris.kroes.supermarket";
        private const string NAME = "Kroes Supermarket";
        private const string VERSION = "1.2.0";

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

            // patches
            HarmonyPatches();

            // finish loading
            mls.LogInfo("Loaded succesfully");
        }

        private void HarmonyPatches()
        {
            // General
            if (config.doublePriceGun)
            {
                harmony.PatchAll(typeof(PriceGunPatch));
                mls.LogInfo("Patched: double price gun");
            }
            if (config.franchiseProgressBar)
            {
                harmony.PatchAll(typeof(FranchiseBarPatch));
                mls.LogInfo("Patched: franchise progress bar");
            }
            if (config.euroSymbol)
            {
                harmony.PatchAll(typeof(EuroSymbolPatch));
                mls.LogInfo("Patched: euro symbol");
            }
            if (config.disableBuildingLimits)
            {
                harmony.PatchAll(typeof(BuildingLimitsPatch));
                mls.LogInfo("Patched: disable building limits");
            }
            if (config.customerShoppingLists)
            {
                harmony.PatchAll(typeof(CustomerShoppingListPatch));
                mls.LogInfo("Patched: customer shopping lists");
            }

            // Third Person Camera
            if (config.thirdPersonCamera)
            {
                harmony.PatchAll(typeof(ThirdPersonCameraPatch));
                ThirdPersonCameraPatch.thirdPersonCameraDistance = config.thirdPersonCameraFollowDistance;
                mls.LogInfo("Patched: third person camera");
            }

            // Mini Transporter
            if (config.betterMiniTransporter)
            {
                harmony.PatchAll(typeof(MiniTransporterPatch));
                MiniTransporterPatch.boxesToAdd = config.MiniTransporterExtraBoxes;
                mls.LogInfo("Patched: better mini transporter");

                MiniTransporterPatch._maxForwardSpeed = config.maxForwardSpeed;
                mls.LogInfo("* max forward speed: " + config.maxForwardSpeed);

                MiniTransporterPatch._maxBackwardSpeed = config.maxBackwardSpeed;
                mls.LogInfo("* max backward speed: " + config.maxBackwardSpeed);

                MiniTransporterPatch._accelerationRate = config.accelerationRate;
                mls.LogInfo("* acceleration rate: " + config.accelerationRate);

                MiniTransporterPatch._decelerationRate = config.decelerationRate;
                mls.LogInfo("* deceleration rate: " + config.decelerationRate);

                MiniTransporterPatch._maxRotationSpeed = config.maxRotationSpeed;
                mls.LogInfo("* max rotation speed: " + config.maxRotationSpeed);
            }

            // Experimental
            if (config.customNpcHitNotifications)
            {
                harmony.PatchAll(typeof(NpcNotificationPatch));
                mls.LogInfo("Patched: custom npc voice lines (EXPERIMENTAL)");
            }
            if (config.throwBoxes)
            {
                harmony.PatchAll(typeof(LayoutManagerPatch)); // to get recycler game object
                harmony.PatchAll(typeof(ThrowableBoxesPatch));
                mls.LogInfo("Patched: throwable boxes (EXPERIMENTAL)");
            }
            if (config.chatCommands)
            {
                harmony.PatchAll(typeof(ChatCommandsPatch));
                mls.LogInfo("Patched: chat commands (EXPERIMENTAL)");
            }
            if (config.showWorldBorders)
            {
                harmony.PatchAll(typeof(WorldBordersPatch));
                mls.LogInfo("Patched: show world borders (EXPERIMENTAL)");
            }
        }
    }
}
