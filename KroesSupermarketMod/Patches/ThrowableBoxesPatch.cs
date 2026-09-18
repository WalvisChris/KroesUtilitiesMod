using HarmonyLib;
using KroesSupermarketMod.CustomScripts;
using Mirror;
using UnityEngine;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ThrowableBoxesPatch
    {
        private static float throwForce = 10f;
        private static float spinStrength = 5f;

        // SERVER: spawn box 1f in front instead of 3.5f
        [HarmonyPatch(typeof(ManagerBlackboard), "CmdSpawnBoxFromPlayer")]
        [HarmonyPrefix]
        public static void CmdSpawnBoxFromPlayer_Prefix(ref Vector3 spawnpoint)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                spawnpoint = camTransform.position + camTransform.forward * 1f;
            }
        }

        // SERVER: add impulse
        [HarmonyPatch(typeof(ManagerBlackboard), "UserCode_CmdSpawnBoxFromPlayer__Vector3__Int32__Int32__Single")]
        [HarmonyPostfix]
        public static void UserCode_CmdSpawnBoxFromPlayer_Postfix(Vector3 spawnpoint, int productID, int numberOfProductsInBox, float YRotation)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnpoint, 0.3f);
            foreach (var col in colliders)
            {
                BoxData box = col.GetComponentInParent<BoxData>();
                if (box != null)
                {
                    Rigidbody rb = box.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Plugin.mls.LogInfo("throwing box server-side"); // <-- just for multiplayer testing

                        Vector3 forwardDirection = Quaternion.Euler(0f, YRotation, 0f) * Vector3.forward;
                        Vector3 throwVector = (forwardDirection * 1.0f) + (Vector3.up * 0.3f);
                        rb.velocity = throwVector * throwForce;
                        rb.angularVelocity = new Vector3(
                            UnityEngine.Random.Range(-spinStrength, spinStrength),
                            UnityEngine.Random.Range(-spinStrength, spinStrength),
                            UnityEngine.Random.Range(-spinStrength, spinStrength)
                        );
                    }
                }
            }
        }

        // CLIENT: add impulse
        [HarmonyPatch(typeof(ManagerBlackboard), "UserCode_RpcParentBoxOnClient__GameObject")]
        [HarmonyPostfix]
        public static void UserCode_RpcParentBoxOnClient_Postfix(GameObject boxOBJ)
        {
            if (boxOBJ == null) return;
            
            Rigidbody rb = boxOBJ.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Plugin.mls.LogInfo("throwing box client-side"); // <-- just for multiplayer testing

                Vector3 forwardDirection = -boxOBJ.transform.right; // compensate for YRotation + 90f
                Vector3 throwVector = (forwardDirection * 1.0f) + (Vector3.up * 0.3f);
                rb.velocity = throwVector * throwForce;
                rb.angularVelocity = new Vector3(
                    UnityEngine.Random.Range(-spinStrength, spinStrength),
                    UnityEngine.Random.Range(-spinStrength, spinStrength),
                    UnityEngine.Random.Range(-spinStrength, spinStrength)
                );
            }
        }

        // CLIENT: attach custom script
        [HarmonyPatch(typeof(BoxData), "OnStartClient")]
        [HarmonyPostfix]
        public static void OnStartClient_Postfix(BoxData __instance)
        {
            if (__instance.gameObject.GetComponent<BoxHitDetector>() == null) __instance.gameObject.AddComponent<BoxHitDetector>();
        }
    }
}
