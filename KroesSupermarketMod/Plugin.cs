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
        private const string VERSION = "1.1.2";

        internal static ManualLogSource mls;
        internal static ConfigManager config;
        internal static Harmony harmony = new Harmony(GUID);
        internal static Plugin instance;
        public static AssetBundle CustomAssets;

        private void Awake()
        {
            if (instance == null) instance = this;

            // logging
            mls = Logger;

            // config
            config = new ConfigManager(Config);

            // assets
            // LoadCustomAssets();

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
        }

        //private void LoadCustomAssets()
        //{
        //    string bundlePath = Path.Combine(Path.GetDirectoryName(Info.Location), "Assets", "assets1");
        //    if (!File.Exists(bundlePath))
        //    {
        //        mls.LogWarning("Missing asset bundle: assets1");
        //        return;
        //    }

        //    CustomAssets = AssetBundle.LoadFromFile(bundlePath);
        //    if (CustomAssets == null)
        //    {
        //        mls.LogWarning("Failed to load asset bundle");
        //        return;
        //    }

        //    GameObject prefab = CustomAssets.LoadAsset<GameObject>("MyVehiclePrefab");
        //    if (prefab == null)
        //    {
        //        mls.LogWarning("Failed to find MyVehiclePrefab in asset bundle!");
        //        return;
        //    }

        //    // The prefab must already carry a NetworkIdentity from Unity (that is where its
        //    // asset ID comes from). Registration itself is NOT done here: NetworkClient forgets
        //    // registered prefabs when a session ends, so NetworkAddonPatch registers it before
        //    // every host/join instead.
        //    if (prefab.GetComponent<NetworkIdentity>() == null)
        //    {
        //        mls.LogError("MyVehiclePrefab is missing a NetworkIdentity component!");
        //        return;
        //    }

        //    NetworkAddon.ObjectPrefab = prefab;
        //    mls.LogInfo("Loaded MyVehiclePrefab");
        //}
    }
}
