using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ThirdPersonCameraPatch
    {
        // TODO:
        // - cancel scroll wheel in DLC builder menu
        // - longer interact range in 3rd person

        private static bool isCameraInThirdPerson = false;
        public static float thirdPersonCameraDistance = 4f;

        [HarmonyPatch(typeof(CustomCameraController), "ThirdPersonEmoteVisualize")]
        [HarmonyPostfix]
        private static void ThirdPersonEmoteVisualize_Postfix()
        {
            isCameraInThirdPerson = false;
        }

        private static bool ReturnInstances(CustomCameraController controller)
        {
            // cancel third person toggle when in DLC painting menu
            //Builder_Paintables DLCmenu = UnityEngine.Object.FindAnyObjectByType<Builder_Paintables>(FindObjectsInactive.Include);
            //if (DLCmenu != null)
            //{
            //    object isInDLCMenu = AccessTools.Field(typeof(Builder_Paintables), "inColorMenu")?.GetValue(DLCmenu);
            //    if (isInDLCMenu is bool inDLCMenu && inDLCMenu) return true;
            //}

            // cancel third person toggle when in settings menu
            object isInOptions = AccessTools.Field(typeof(CustomCameraController), "IsInOptions")?.GetValue(controller);
            if (isInOptions is bool inOptions && inOptions) return true;
            
            if (GameCanvas.Instance?.transform.Find("Builder")?.gameObject.activeSelf == true) return true; // cancel third person toggle when in building menu
            if (controller.isInCameraEvent) return true; // cancel third person toggle when in UI
            if (controller.inEmoteEvent) return true; // cancel third person toggle when emoting
            if (controller.inVehicle) return true; // cancel third person toggle when in vehicle
            return false;
        }

        [HarmonyPatch(typeof(CustomCameraController), "LateUpdate")]
        [HarmonyPostfix]
        private static void LateUpdate_Postfix(CustomCameraController __instance)
        {
            float scroll = Input.mouseScrollDelta.y;

            if (scroll < 0f && !isCameraInThirdPerson) // wheel down & not 3rd person
            {
                UpdateCamera(__instance, true);
            }
            else if (scroll > 0f && isCameraInThirdPerson) // wheel up & 3rd person
            {
                UpdateCamera(__instance, false);
            }
        }

        private static void UpdateCamera(CustomCameraController controller, bool doThirdPerson)
        {
            if (ReturnInstances(controller)) return; // All the exceptions

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
