using HarmonyLib;
using KroesSupermarketMod.CustomScripts;
using System.Collections.Generic;
using UnityEngine;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class WorldBordersPatch
    {
        //private static List<GameObject> visualizers = new List<GameObject>();

        [HarmonyPatch(typeof(GameCanvas), "Awake")]
        [HarmonyPostfix]
        private static void Prefix()
        {
            Collider[] colliders = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None);
            foreach(Collider col in colliders)
            {
                GameObject obj = col.gameObject;
                string name = obj.name;

                if (col is BoxCollider && name.StartsWith("Base_Collider"))
                {
                    obj.AddComponent<BoxColliderVisualizer>();
                    Plugin.mls.LogInfo($"Assigned BoxColliderVisualizer to '{name}'");
                }
                else if (col is MeshCollider && name.StartsWith("WeatherPlane_"))
                {
                    obj.AddComponent<MeshColliderVisualizer>();
                    Plugin.mls.LogInfo($"Assigned MeshColliderVisualizer to '{name}'");
                }
            }

            //Collider[] colliders = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None);
            //foreach (Collider col in colliders)
            //{
            //    GameObject obj = col.gameObject;

            //    if (visualizers.Contains(obj)) continue;

            //    if (obj.name.StartsWith("Base_Collider"))
            //    {
            //        obj.AddComponent<BoxColliderVisualizer>();
            //        visualizers.Add(obj);
            //        Plugin.mls.LogInfo($"Added BoxColliderVisualizer to: {obj.name}");
            //    }
            //    else if (obj.name.StartsWith("WeatherPlane_"))
            //    {
            //        obj.AddComponent<MeshColliderVisualizer>();
            //        visualizers.Add(obj);
            //        Plugin.mls.LogInfo($"Added MeshColliderVisualizer to: {obj.name}");
            //    }
            //}
        }
    }
}
