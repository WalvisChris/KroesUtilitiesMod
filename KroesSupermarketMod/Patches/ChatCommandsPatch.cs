using HarmonyLib;
using System;
using UnityEngine;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ChatCommandsPatch
    {
        private static readonly string[] weatherStrings = new string[]
        {
            "normal",   // 0
            "normal+",  // 1
            "normal++", // 2
            "rain",     // 3
            "rain+",    // 4
            "snow"      // 5
        };

        private static readonly string[] poseStrings = new string[]
        {
            "idle1",
            "CrouchIdle",
            "run",
            "walk",
            "CrouchWalking",
            "Trip",
            "Standing Melee Attack Horizontal",
            "1_HipHopDance",
            "2_StupidDance",
            "3_Salute",
            "4_TheHurricane",
            "5_GroovyDance",
            "6_OogieBoogie",
            "7_SalsaDance",
            "8_SwingDance",
            "9_Swimming",
            "10_HipHopDance2",
            "11_RobotDance",
            "12_HouseDancing",
            "13_Singing",
            "14_Flair",
            "15_UprockDance",
            "16_GuitarPlay",
            "17_Falling",
            "18_ClimbInPlace",
            "19_Hanging",
            "20_QuietPose_1",
            "21_QuietPose_2",
            "22_QuietPose_3",
            "23_QuietPose_4",
            "24_BreakDanceFreeze",
            "25_WaveDance",
            "26_SillyDance",
            "27_Cheering",
            "28_Clapping",
            "29_Victory",
            "30_SleepOnFloor",
            "31_Cartwheel",
            "32_Praying",
            "33_Scared",
            "34_Capoeira",
            "35_Statue"
        };

        [HarmonyPatch(typeof(PlayerObjectController), "SendChatMsg")]
        private static bool SendChatMsg_Prefix(PlayerObjectController __instance, ref string message)
        {
            if (!string.IsNullOrEmpty(message) && message.StartsWith("/"))
            {
                string[] args = message.Substring(1).Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 0) return false;

                string command = args[0].ToLower();

                switch (command)
                {
                    case "npc":
                        NPC_Info[] allNPCs = UnityEngine.Object.FindObjectsByType<NPC_Info>(FindObjectsSortMode.None);

                        if (args.Length == 1)
                        {
                            foreach (NPC_Info npc in allNPCs)
                            {
                                Utilities.ResetNpcAnimator(npc.gameObject);
                            }

                            Utilities.CreateCanvasNotification("Reset NPC animators.");
                            return false;
                        }

                        string subArg = args[1].Trim().ToLower();

                        if (subArg == "random")
                        {
                            int randomIndex = UnityEngine.Random.Range(0, poseStrings.Length);
                            string randomPose = poseStrings[randomIndex];

                            foreach (NPC_Info npc in allNPCs)
                            {
                                Utilities.PlayEmoteOnNpc(npc.gameObject, randomPose);
                            }

                            Utilities.CreateCanvasNotification($"Random pose {randomIndex}: {randomPose}");
                            return false;
                        }

                        if (int.TryParse(subArg, out int animationIndex))
                        {
                            if (animationIndex >= 0 && animationIndex < poseStrings.Length)
                            {
                                string animationString = poseStrings[animationIndex];

                                foreach (NPC_Info npc in allNPCs)
                                {
                                    Utilities.PlayEmoteOnNpc(npc.gameObject, animationString);
                                }

                                Utilities.CreateCanvasNotification($"Performing pose {animationIndex}: {animationString}");
                            }
                            else
                            {
                                Utilities.CreateCanvasNotification($"Ongeldige index! Kies tussen 0 en {poseStrings.Length - 1}");
                            }
                        }
                        return false;

                    case "weather":
                        if (args.Length > 1 && int.TryParse(args[1], out int weatherType) && weatherType < 6)
                        {
                            var weatherManager = UnityEngine.Object.FindFirstObjectByType<GameData>();
                            if (weatherManager != null)
                            {
                                weatherManager.NetworkweatherIndex = weatherType;

                                if (weatherType >= 4)
                                {
                                    NPC_Manager.Instance.badWeatherDay = true;

                                    var upgradeMgr = weatherManager.GetComponent<UpgradesManager>();
                                    if (upgradeMgr != null && upgradeMgr.extraUpgrades.Length > 43 && upgradeMgr.extraUpgrades[43])
                                    {
                                        OrderPackaging.Instance.extraOrdersDueToWeather = UnityEngine.Random.Range(3, 6);
                                    }
                                }
                                else
                                {
                                    NPC_Manager.Instance.badWeatherDay = false;
                                    OrderPackaging.Instance.extraOrdersDueToWeather = 0;
                                }

                                string currentTempData = weatherManager.NetworktodayTemperatureData;

                                var rpcMethod = HarmonyLib.AccessTools.Method(typeof(GameData), "RpcSetWeather", new Type[] { typeof(int), typeof(string) });
                                if (rpcMethod != null)
                                {
                                    rpcMethod.Invoke(weatherManager, new object[] { weatherType, currentTempData });
                                }
                            }
                            Utilities.CreateCanvasNotification($"Weather set to {weatherType}: {weatherStrings[weatherType]}");
                        }
                        return false;

                    case "notif1":
                        if (args.Length > 1) Utilities.CreateCanvasNotification(args[1]);
                        return false;

                    case "notif2":
                        if (args.Length > 1) Utilities.CreateImportantNotification(args[1]);
                        return false;

                    default:
                        return true;
                }
            }
            return true;
        }
    }
}
