using HarmonyLib;
using KroesSupermarketMod.CustomScripts;
using UnityEngine;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ThrowableBoxesPatch
    {
        private static Vector3 pendingVelocity;
        private static Vector3 pendingAngularVelocity;
        private static bool shouldApplyPhysics = false;

        [HarmonyPatch(typeof(ManagerBlackboard), "UserCode_CmdSpawnBoxFromPlayer__Vector3__Int32__Int32__Single")]
        [HarmonyPrefix]
        public static void Prefix(ref Vector3 spawnpoint, float YRotation)
        {
            Vector3 forwardDirection = Quaternion.Euler(0f, YRotation, 0f) * Vector3.forward;
            spawnpoint -= forwardDirection * 2.0f;

            Vector3 throwVector = (forwardDirection * 1.0f) + (Vector3.up * 0.3f);
            float throwForce = 10f;
            pendingVelocity = throwVector * throwForce;

            float spinStrength = 5f;
            pendingAngularVelocity = new Vector3(
                UnityEngine.Random.Range(-spinStrength, spinStrength),
                UnityEngine.Random.Range(-spinStrength, spinStrength),
                UnityEngine.Random.Range(-spinStrength, spinStrength)
            );

            shouldApplyPhysics = true;
        }

        [HarmonyPatch(typeof(ManagerBlackboard), "UserCode_CmdSpawnBoxFromPlayer__Vector3__Int32__Int32__Single")]
        [HarmonyPostfix]
        public static void Postfix(Vector3 spawnpoint)
        {
            if (!shouldApplyPhysics) return;
            shouldApplyPhysics = false;

            Collider[] colliders = Physics.OverlapSphere(spawnpoint, 0.6f);

            foreach (var col in colliders)
            {
                if (col.GetComponent<BoxData>() != null)
                {
                    GameObject boxObj = col.gameObject;

                    BoxHitDetector detector = boxObj.GetComponent<BoxHitDetector>();
                    if (detector == null) detector = boxObj.AddComponent<BoxHitDetector>();
                    detector.hasAlreadyHit = false;

                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = pendingVelocity;
                        rb.angularVelocity = pendingAngularVelocity;
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BoxData), "OnStartClient")]
        [HarmonyPostfix]
        public static void OnStartClient_Postfix(BoxData __instance)
        {
            if (__instance.gameObject.GetComponent<BoxHitDetector>() == null) __instance.gameObject.AddComponent<BoxHitDetector>();
        }
    }
}
