using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace KroesSupermarketMod
{
    internal class Utilities
    {
        public static void CreateCanvasNotification(string message)
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

        public static void CreateImportantNotification(string message)
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

        public static void PlayEmoteOnNpc(GameObject npcOBJ, string pose)
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

        public static void ResetNpcAnimator(GameObject npcOBJ)
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
    }
}
