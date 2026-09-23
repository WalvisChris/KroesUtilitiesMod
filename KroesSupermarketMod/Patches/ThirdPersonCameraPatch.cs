using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ThirdPersonCameraPatch
    {
        private static bool isCameraInThirdPerson = false;
        public static float thirdPersonCameraDistance = 4f;

        [HarmonyPatch(typeof(CustomCameraController), "LateUpdate")]
        [HarmonyPostfix]
        private static void Postfix(CustomCameraController __instance)
        {
            if (__instance.isInCameraEvent || __instance.inEmoteEvent || __instance.inVehicle) return;
            Transform builderTransform = GameCanvas.Instance?.transform.Find("Builder");
            if (builderTransform != null && builderTransform.gameObject.activeSelf) return;

            float scroll = Input.mouseScrollDelta.y;

            if (scroll < 0f && !isCameraInThirdPerson) // wheel down & not 3rd person
            {
                UpdateCamera(true);
            }
            else if (scroll > 0f && isCameraInThirdPerson) // wheel up & 3rd person
            {
                UpdateCamera(false);
            }
        }

        private static void UpdateCamera(bool doThirdPerson)
        {
            // Increased interation range?

            CustomCameraController controller = Camera.main?.GetComponent<CustomCameraController>();
            if (controller != null)
            {
                object isInOptions = AccessTools.Field(typeof(CustomCameraController), "IsInOptions")?.GetValue(controller);
                if (isInOptions is bool inOptions && inOptions) return;

                isCameraInThirdPerson = doThirdPerson;

                object thirdPersonFollow = AccessTools.Field(typeof(CustomCameraController), "thirdPersonFollow")?.GetValue(controller);

                if (thirdPersonFollow != null)
                {
                    // Camera Distance
                    float targetDistance = doThirdPerson ? thirdPersonCameraDistance : 0f;
                    AccessTools.Field(thirdPersonFollow.GetType(), "CameraDistance")?.SetValue(thirdPersonFollow, targetDistance);

                    // Camera Mask
                    string maskFieldName = doThirdPerson ? "thirdPersonDefaultLayerMask" : "thirdPersonNullLayerMask";
                    object layerMask = AccessTools.Field(typeof(CustomCameraController), maskFieldName)?.GetValue(controller);

                    if (layerMask != null) AccessTools.Field(thirdPersonFollow.GetType(), "CameraCollisionFilter")?.SetValue(thirdPersonFollow, layerMask);

                    AccessTools.Method(typeof(CustomCameraController), "ShowCharacter", new[] { typeof(bool) })?.Invoke(controller, new object[] { doThirdPerson });
                }
            }
        }
    }
}
