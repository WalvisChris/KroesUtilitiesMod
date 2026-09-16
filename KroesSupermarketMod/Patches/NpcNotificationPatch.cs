using HarmonyLib;
using TMPro;
using UnityEngine;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class NpcNotificationPatch
    {
        private static readonly string[] CustomNotifications = new string[]
        {
            "Kanker!",                                  // NPCmessagehit0
            "Het is de echter gangster, Klaas!",        // NPCmessagehit1
            "Ik ben vergeten in m'n bed te pissen...",  // NPCmessagehit2
            "What stays in Kos stays in Kos.",          // NPCmessagehit3
            "Waar bemoei jij je mee?!",                 // NPCmessagehit4
            "Krebsratte!",                              // NPCmessagehit5
            "Josh zitten!",                             // NPCmessagehit6
            "Ik mag thuis niet lachen.",                // NPCmessagehit7
            "Bruinjoekel!"                              // NPCmessagehit8
        };

        [HarmonyPatch(typeof(NPC_Info), "UserCode_RPCNotificationAboveHead__String__String")]
        [HarmonyPrefix]
        public static bool Prefix(NPC_Info __instance, string message1, GameObject ___messagePrefab)
        {
            if (message1 != null && message1.StartsWith("NPCmessagehit"))
            {
                if (___messagePrefab == null) return false;

                string indexString = message1.Substring("NPCmessagehit".Length);
                int index = 0;

                if (int.TryParse(indexString, out int parsedindex)) index = parsedindex % CustomNotifications.Length;
                
                string customText = CustomNotifications[index];

                GameObject gameObject = UnityEngine.Object.Instantiate(
                    ___messagePrefab,
                    __instance.transform.position + Vector3.up * 1.8f,
                    Quaternion.identity
                );

                gameObject.GetComponent<TextMeshPro>().text = customText;
                gameObject.SetActive(true);

                return false;
            }
            return true;
        }
    }
}
