using HarmonyLib;
using KroesSupermarketMod.Patches;
using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace KroesSupermarketMod
{
    internal class Utilities
    {
        // HELPER METHODS
        private static void CreateCanvasNotification(string message)
        {
            var manager = UnityEngine.Object.FindFirstObjectByType<GameCanvas>();
            if (manager == null) return;

            bool inCooldown = (bool)AccessTools.Field(manager.GetType(), "inCooldown").GetValue(manager);
            if (inCooldown) return;

            GameObject prefab = (GameObject)AccessTools.Field(manager.GetType(), "notificationPrefab").GetValue(manager);
            Transform parent = (Transform)AccessTools.Field(manager.GetType(), "notificationParentTransform").GetValue(manager);

            if (prefab != null && parent != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(prefab, parent);

                var tmpText = gameObject.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = message;
                }

                gameObject.SetActive(true);
                manager.StartCoroutine("NotificationCooldown");
            }
        }
        private static void CreateImportantNotification(string message)
        {
            var manager = UnityEngine.Object.FindFirstObjectByType<GameCanvas>();
            if (manager == null) return;

            bool inCooldown = (bool)AccessTools.Field(manager.GetType(), "inCooldown").GetValue(manager);
            if (inCooldown) return;

            GameObject prefab = (GameObject)AccessTools.Field(manager.GetType(), "importantNotificationPrefab").GetValue(manager);
            Transform parent = (Transform)AccessTools.Field(manager.GetType(), "importantNotificationParentTransform").GetValue(manager);

            if (prefab != null && parent != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(prefab, parent);

                var tmpText = gameObject.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = message;
                }

                gameObject.SetActive(true);
                manager.StartCoroutine("NotificationCooldown");
            }
        }
        private static void PlayEmoteOnNpc(GameObject npcOBJ, string pose)
        {
            if (npcOBJ == null) return;

            Animator animator = npcOBJ.GetComponentInChildren<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null) return;

            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Equals(pose, System.StringComparison.OrdinalIgnoreCase))
                {
                    AnimationPlayableUtilities.PlayClip(animator, clip, out PlayableGraph graph);
                    return;
                }
            }

            Plugin.mls.LogInfo($"Clip '{pose}' niet gevonden op NPC!");
        }
        private static void ResetNpcAnimator(GameObject npcOBJ)
        {
            if (npcOBJ == null) return;

            Animator animator = npcOBJ.GetComponentInChildren<Animator>();
            if (animator == null) return;

            if (animator.hasBoundPlayables)
            {
                PlayableGraph graph = animator.playableGraph;
                if (graph.IsValid())
                {
                    graph.Destroy();
                }
            }

            animator.Rebind();
            animator.Update(0f);
        }
        public static void EnsureProgressBarCreated(TextMeshProUGUI targetText, ref Slider progressBar)
        {
            if (progressBar != null) return;

            // Create container object parented to the TextMeshPro object
            GameObject barObj = new GameObject("FranchiseProgressBar", typeof(RectTransform), typeof(Slider));
            barObj.transform.SetParent(targetText.transform, false);

            RectTransform barRect = barObj.GetComponent<RectTransform>();

            // Position above the text object
            barRect.anchorMin = new Vector2(0.5f, 1f);
            barRect.anchorMax = new Vector2(0.5f, 1f);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.anchoredPosition = new Vector2(0, 20f);
            barRect.sizeDelta = new Vector2(240f, 15f);

            // Background Image (white)
            GameObject background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(barObj.transform, false);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            background.GetComponent<Image>().color = Color.white;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(barObj.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            // Fill Image
            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;

            Image fillImage = fill.GetComponent<Image>();
            fillImage.color = new Color(0.58f, 0.318f, 0.973f);

            // Setup Slider Component
            progressBar = barObj.GetComponent<Slider>();
            progressBar.fillRect = fillRect;
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.interactable = false;
        }
        public static float GetLevelProgress(int currentExp)
        {
            int num = 0;
            int num2 = 1;

            while ((float)num2 < float.PositiveInfinity)
            {
                num += num2 * 100;
                if (num > currentExp)
                {
                    float currentLevelExp = (float)(currentExp - (num - num2 * 100));
                    float requiredLevelExp = (float)(num2 * 100);
                    return currentLevelExp / requiredLevelExp;
                }
                num2++;
            }

            return 0f;
        }
        
        // GAME REFERENCES
        public static GameObject recycleObj1 = null;
        public static GameObject recycleObj2 = null;
        public static GameObject trashObj = null;
        
        // CHAT COMMANDS
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
        private static readonly Dictionary<string, int> commonProps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "ladder", 8 }
        };
        private static readonly Dictionary<string, int> commonDecorations = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "tv", 3 },
            { "train", 68 },
            { "wagon", 275 }
        };
        public static bool TryParseCommand(string rawChatMessage, out string command, out string[] args)
        {
            command = string.Empty;
            args = Array.Empty<string>();

            if (string.IsNullOrWhiteSpace(rawChatMessage) || !rawChatMessage.StartsWith("/")) return false;

            string[] parts = rawChatMessage.Substring(1).Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return false;

            command = parts[0].ToLower();

            if (parts.Length > 1) args = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return true;
        }
        public static void ChatCommand_Npc(string[] args)
        {
            NPC_Info[] allNPCs = UnityEngine.Object.FindObjectsByType<NPC_Info>(FindObjectsSortMode.None);

            if (args.Length < 1)
            {
                foreach (NPC_Info npc in allNPCs)
                {
                    ResetNpcAnimator(npc.gameObject);
                }

                CreateCanvasNotification("Reset NPC animators.");
                return;
            }

            if (args[0].Trim().ToLower() == "random")
            {
                int randomIndex = UnityEngine.Random.Range(0, poseStrings.Length);
                string randomPose = poseStrings[randomIndex];

                foreach (NPC_Info npc in allNPCs)
                {
                    PlayEmoteOnNpc(npc.gameObject, randomPose);
                }

                CreateCanvasNotification($"Random pose {randomIndex}: {randomPose}");
                return;
            }

            if (int.TryParse(args[0], out int animationIndex))
            {
                if (animationIndex >= 0 && animationIndex < poseStrings.Length)
                {
                    string animationString = poseStrings[animationIndex];

                    foreach (NPC_Info npc in allNPCs)
                    {
                        PlayEmoteOnNpc(npc.gameObject, animationString);
                    }

                    CreateCanvasNotification($"Performing pose {animationIndex}: {animationString}");
                }
                else
                {
                    CreateCanvasNotification($"Ongeldige index! Kies tussen 0 en {poseStrings.Length - 1}");
                }
            }
            return;
        }
        public static void ChatCommand_Weather(string[] args)
        {
            if (args.Length > 0 && int.TryParse(args[0], out int weatherType) && weatherType < 6)
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
                CreateCanvasNotification($"Weather set to {weatherType}: {weatherStrings[weatherType]}");
            }
            return;
        }
        public static void ChatCommand_Notify(string[] args)
        {
            if (args.Length > 1 && int.TryParse(args[0], out int notificationType) && notificationType < 2)
            {
                if (notificationType == 0)
                {
                    CreateCanvasNotification(args[1]);
                }
                else if (notificationType == 1)
                {
                    CreateImportantNotification(args[1]);
                }
            }
        }
        public static void ChatCommand_Spawn(string[] args)
        {
            if (args.Length < 1) return;

            if (Camera.main != null)
            {
                string search = args[0].Trim().ToLower();

                // props
                if (commonProps.TryGetValue(search, out int result))
                {
                    Transform camTransform = Camera.main.transform;
                    Vector3 vector = camTransform.position + camTransform.forward * 3.5f;

                    GameData.Instance.GetComponent<NetworkSpawner>().CmdSpawnProp(result, vector, Vector3.zero);
                    CreateCanvasNotification($" Spawned prop {search}.");

                    return;
                }

                // decorations
                if (commonDecorations.TryGetValue(search, out int index))
                {
                    Transform camTransform = Camera.main.transform;
                    Vector3 vector = camTransform.position + camTransform.forward * 3.5f;

                    GameData.Instance.GetComponent<NetworkSpawner>().CmdSpawnDecoration(index, vector, Vector3.zero);
                    CreateCanvasNotification($" Spawned decoration {search}.");

                    return;
                }
            }
        }
    }
}
