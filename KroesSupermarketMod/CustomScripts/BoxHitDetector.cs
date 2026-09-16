using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KroesSupermarketMod.CustomScripts
{
    public class BoxHitDetector : MonoBehaviour
    {
        private Rigidbody rb;
        public bool hasAlreadyHit = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CheckHit(collision.gameObject);
        }

        private void CheckHit(GameObject otherOBJ)
        {
            // 1. VOORKOM MULTIPLAYER SPAM: Voer de logica ALLEEN uit op de Server/Host!
            if (!NetworkServer.active) return;

            // 2. Negeer als er al geraakt is of als de doos te langzaam vliegt
            if (hasAlreadyHit || rb == null || rb.velocity.magnitude < 1f)
            {
                return;
            }

            // 3. NPC Hit Logica
            if (otherOBJ.name == "HitTrigger" && otherOBJ.transform.parent && otherOBJ.transform.parent.GetComponent<NPC_Info>())
            {
                hasAlreadyHit = true; // Vergrendel direct op de server
                NPC_Info npc = otherOBJ.transform.parent.GetComponent<NPC_Info>();

                // Animaties en notificatie (wordt automatisch door Mirror gesynchroniseerd)
                npc.CmdAnimationPlay(0);
                int randomHitIndex = UnityEngine.Random.Range(0, 9);
                npc.RPCNotificationAboveHead($"NPCmessagehit{randomHitIndex}", "");

                // Stagger/Stumble via FSM (Handelt de fysieke reactie van de NPC af)
                PlayMakerFSM fsm = otherOBJ.GetComponent<PlayMakerFSM>();
                if (fsm != null)
                {
                    fsm.SendEvent("Send_Data_2");
                }

                // Rem de doos af na de klap
                rb.velocity *= 0.1f;
                return;
            }

            // 4. Speler Hit Logica
            if (otherOBJ.name == "HitTrigger" &&
                otherOBJ.transform.parent &&
                otherOBJ.transform.parent.transform.parent &&
                otherOBJ.transform.parent.transform.parent.GetComponent<PlayerNetwork>())
            {
                hasAlreadyHit = true;
                PlayerNetwork player = otherOBJ.transform.parent.transform.parent.GetComponent<PlayerNetwork>();

                PlayMakerFSM fsm = otherOBJ.GetComponent<PlayMakerFSM>();
                if (fsm != null)
                {
                    fsm.SendEvent("Send_Data_2");
                }

                // Heel subtiele push-vector (0.1f) voor de speler
                Vector3 pushDirection = (otherOBJ.transform.position - transform.position).normalized * 0.1f;
                player.PushPlayer(pushDirection);

                rb.velocity *= 0.1f;
            }
        }
    }
}
