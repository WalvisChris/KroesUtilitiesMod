using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class MiniTransporterPatch
    {
        [HarmonyPatch(typeof(MiniTransportRework), "Update")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == -1f &&
                    codes[i + 1].opcode == OpCodes.Mul)
                {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i + 1].opcode = OpCodes.Nop;
                    break;
                }
            }
            return codes;
        }

        public static int boxesToAdd = 2;
        public static float _maxForwardSpeed = 8f; // decompiled code says 5f, unity projects settings differ
        public static float _maxBackwardSpeed = 5f; // decompiled code says 2f, unity projects settings differ
        public static float _accelerationRate = 6f; // decompiled code says 5f, unity projects settings differ
        public static float _decelerationRate = 5f; // decompiled code says 2f, unity projects settings differ
        public static float _maxRotationSpeed = 75f; // decompiled code says 150f, unity projects settings differ
        private static readonly HashSet<int> ProcessedVehicles = new HashSet<int>();

        [HarmonyPatch(typeof(MiniTransportRework), "OnStartClient")]
        [HarmonyPrefix]
        private static void Prefix(MiniTransportRework __instance)
        {
            if (boxesToAdd < 1) return;

            int instanceId = __instance.GetInstanceID();
            if (ProcessedVehicles.Contains(instanceId)) return;

            ProcessedVehicles.Add(instanceId);
            UpdateVehicle(__instance);
        }

        private static void UpdateVehicle(MiniTransportRework vehicleRework)
        {
            GameObject vehicleOBJ = vehicleRework.gameObject;
            Transform boxContainer = vehicleOBJ.transform.Find("BoxContainer");
            Transform highlights = vehicleOBJ.transform.Find("Highlights");

            if (boxContainer == null || highlights == null)
            {
                Plugin.mls.LogInfo("Mini Transporter: failed to find BoxContainer or Highlights!");
                return;
            }

            // Set variables from config
            vehicleRework.maxForwardSpeed = _maxForwardSpeed;
            vehicleRework.maxBackwardSpeed = _maxBackwardSpeed;
            vehicleRework.accelerationRate = _accelerationRate;
            vehicleRework.decelerationRate = _decelerationRate;
            vehicleRework.maxRotationSpeed = _maxRotationSpeed;

            // define values
            float yOffset = 1.95f;
            int count = Math.Min(boxesToAdd, 6); // max 6

            // 1. Create new box subcontainers
            AddBoxSubContainers(boxContainer, yOffset, count);

            // 2. Create new highlights
            AddHighlights(highlights, yOffset, count);

            // 2. Add UI icons for Cmd.BoxSpawner()
            AddUIContainerSlots(vehicleRework, count);
            AddCanvasSignSlots(vehicleOBJ.transform, count);

            // 3. Add new slots via server RPC
            if (vehicleRework.isServer)
            {
                AddNewSlotsToProductInfoArray(vehicleRework, count);

                int startIndex = vehicleRework.productInfoArray.Length / 2 - count;
                for (int i = 0; i < count; i++)
                {
                    int slotIndex = startIndex + i;
                    UpdateSlotViaReflection(vehicleRework, slotIndex, -1, -1);
                }

                Plugin.mls.LogInfo($"Added {count} new slots to product info on server.");
            }
        }

        private static int AddBoxSubContainers(Transform parent, float yOffset, int count)
        {
            // 1. Get existing Subcontainers
            List<Transform> existingSubcontainers = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child.name.Contains("BoxSubcontainer")) existingSubcontainers.Add(child);
            }

            // 2. Create new colliders
            List<Collider> newColliders = new List<Collider>();
            for (int i = 0; i < count; i++)
            {
                Transform original = existingSubcontainers[i];
                GameObject duplicated = UnityEngine.Object.Instantiate(original.gameObject, parent);

                // Local data
                Vector3 localPosition = original.localPosition;
                localPosition.y += yOffset;
                duplicated.transform.localPosition = localPosition;
                duplicated.transform.localRotation = original.localRotation;
                
                // Match layers
                duplicated.layer = original.gameObject.layer;
                foreach (Transform child in duplicated.transform)
                {
                    child.gameObject.layer = original.gameObject.layer;
                }


                // Add colliders
                Collider[] cols = duplicated.GetComponentsInChildren<Collider>();
                if (cols != null && cols.Length > 0) newColliders.AddRange(cols);
            }

            Plugin.mls.LogInfo($"Added {newColliders.Count} new BoxSubcontainer colliders.");
            return newColliders.Count;
        }

        private static void AddHighlights(Transform parent, float yOffset, int count)
        {
            // 1. Get existing Highlights
            List<Transform> existingHighlights = new List<Transform>();
            foreach (Transform child in parent)
            {
                existingHighlights.Add(child);
            }

            // 2. Calculate name
            int nextIndex = existingHighlights.Count;

            // 3. Create new highlights
            for (int i = 0; i < count; i++)
            {
                Transform original = existingHighlights[i];
                GameObject duplicated = UnityEngine.Object.Instantiate(original.gameObject, parent);

                Vector3 localPosition = original.localPosition;
                localPosition.y += yOffset;
                duplicated.transform.localPosition = localPosition;
                duplicated.transform.localRotation = original.localRotation;

                duplicated.name = nextIndex++.ToString();
            }

            Plugin.mls.LogInfo("Highlights updated.");
        }

        private static void AddUIContainerSlots(MiniTransportRework vehicleRework, int count)
        {
            GameObject container = vehicleRework.UIboxContainerParent.gameObject;

            float scaleFactor = 1f - (count / 11f); // (count / 6f) for 0-100%
            container.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            if (container.transform.childCount > 0)
            {
                Transform template = container.transform.GetChild(0);
                Image templateImg = template.GetComponent<Image>();

                for (int i = 0; i < count; i++)
                {
                    GameObject duplicatedOBJ = UnityEngine.Object.Instantiate(template.gameObject, container.transform);
                    Image duplicatedIMG = duplicatedOBJ.GetComponent<Image>();
                }
            }

            Plugin.mls.LogInfo($"Added {count} UI containers.");
        }

        private static void AddCanvasSignSlots(Transform vehicleTransform, int count)
        {
            Transform canvasSigns = vehicleTransform.Find("CanvasSigns");
            if (canvasSigns == null || canvasSigns.childCount == 0) return;

            Transform template = canvasSigns.GetChild(0);
            for (int i = 0; i < count; i++) UnityEngine.Object.Instantiate(template.gameObject, canvasSigns);

            Plugin.mls.LogInfo($"Added {count} slots in CanvasSigns.");
        }

        private static void AddNewSlotsToProductInfoArray(MiniTransportRework vehicleRework, int count)
        {
            int oldLength = vehicleRework.productInfoArray.Length;
            int newLength = oldLength + count * 2; // ?
            int[] array = new int[newLength];

            for (int i = 0; i < oldLength; i++)
            {
                array[i] = vehicleRework.productInfoArray[i];
            }
            for (int j = oldLength; j < newLength; j++)
            {
                array[j] = -1;
            }
            vehicleRework.productInfoArray = array;

            Plugin.mls.LogInfo($"Added {count} slots to productInfoArray.");
        }

        private static void UpdateSlotViaReflection(MiniTransportRework vehicleRework, int slotIndex, int PID, int PNUMBER)
        {
            int[] productInfoArray = vehicleRework.productInfoArray;
            productInfoArray[slotIndex * 2] = PID;
            productInfoArray[slotIndex * 2 + 1] = PNUMBER;
            vehicleRework.productInfoArray = productInfoArray;

            MethodInfo method = typeof(MiniTransportRework).GetMethod("UserCode_RpcUpdateArrayValuesStorage__Int32__Int32__Int32", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (method == null)
            {
                Plugin.mls.LogInfo("Failed to find method: RpcUpdateArrayValueStorage");
                return;
            }

            method.Invoke(vehicleRework, new object[]
            {
                slotIndex,
                PID,
                PNUMBER
            });
            // INDEX?!
            Plugin.mls.LogInfo($"Slot {slotIndex} updated via RPC.");
        }
    }
}