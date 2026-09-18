using Mirror;
using Rewired.Integration.PlayMaker;
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
        private BoxData boxData = null;
        private GameObject recycle1Obj = null;
        private GameObject recycle2Obj = null;
        private GameObject trashObj = null;
        public bool hasAlreadyHit = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            boxData = GetComponent<BoxData>();
        }

        private void Awake()
        {
            if (Utilities.recycleObj1 != null) recycle1Obj = Utilities.recycleObj1;
            if (Utilities.recycleObj2 != null) recycle2Obj = Utilities.recycleObj2;
            if (Utilities.trashObj != null) trashObj = Utilities.trashObj;
        }

        private void Update()
        {
            if (boxData != null && boxData.numberOfProducts > 0) return;

            if (NetworkServer.active)
            {
                float distance1 = (recycle1Obj != null) ? Vector3.Distance(transform.position, recycle1Obj.transform.position) : float.MaxValue;
                float distance2 = (recycle2Obj != null) ? Vector3.Distance(transform.position, recycle2Obj.transform.position) : float.MaxValue;
                float distance3 = (trashObj != null) ? Vector3.Distance(transform.position, trashObj.transform.position) : float.MaxValue;

                if (distance1 < 2.5f || distance2 < 2.5f)
                {
                    RecycleBox();
                }
                else if (distance3 < 3f) // this one seems to be a little bigger for some reason
                {
                    if (NPC_Manager.Instance.closestRecyclePerk)
                    {
                        RecycleBox();
                    }
                    else
                    {
                        Plugin.mls.LogInfo("trash thrown box...");
                        // make use the box is gone for all clients
                        NetworkServer.Destroy(gameObject);
                    }
                }
            }
        }

        private void RecycleBox()
        {
            Plugin.mls.LogInfo("recycle thrown box...");

            // literal copy from NPC recycling: NPC_Manager.EmpolyeeNPCControl
            float fundsToAdd = 1.5f * (float)GameData.Instance.GetComponent<UpgradesManager>().boxRecycleFactor;
            AchievementsManager.Instance.CmdAddAchievementPoint(2, 1);
            StatisticsManager.Instance.totalBalesRecycled++;
            GameData.Instance.CmdAlterFunds(fundsToAdd);

            // make use the box is gone for all clients
            NetworkServer.Destroy(gameObject);
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
            if (!NetworkServer.active) return;
            if (hasAlreadyHit || rb == null || rb.velocity.magnitude < 1f) return;

            // NPC hit logic
            if (otherOBJ.name == "HitTrigger" && otherOBJ.transform.parent && otherOBJ.transform.parent.GetComponent<NPC_Info>())
            {
                hasAlreadyHit = true;
                NPC_Info npc = otherOBJ.transform.parent.GetComponent<NPC_Info>();

                npc.CmdAnimationPlay(0);
                int randomHitIndex = UnityEngine.Random.Range(0, 9);
                npc.RPCNotificationAboveHead($"NPCmessagehit{randomHitIndex}", "");

                // stumble
                PlayMakerFSM fsm = otherOBJ.GetComponent<PlayMakerFSM>();
                if (fsm != null)
                {
                    fsm.SendEvent("Send_Data_2");
                }

                // slow down box
                rb.velocity *= 0.1f;
                return;
            }

            // Player hit logic
            if (otherOBJ.name == "HitTrigger" &&
                otherOBJ.transform.parent &&
                otherOBJ.transform.parent.transform.parent &&
                otherOBJ.transform.parent.transform.parent.GetComponent<PlayerNetwork>())
            {
                hasAlreadyHit = true;
                PlayerNetwork player = otherOBJ.transform.parent.transform.parent.GetComponent<PlayerNetwork>();

                // stumble
                PlayMakerFSM fsm = otherOBJ.GetComponent<PlayMakerFSM>();
                if (fsm != null)
                {
                    fsm.SendEvent("Send_Data_2");
                }

                // subtle push for player
                Vector3 pushDirection = (otherOBJ.transform.position - transform.position).normalized * 0.1f;
                player.PushPlayer(pushDirection);

                // slow down box
                rb.velocity *= 0.1f;
            }
        }
    }
}
