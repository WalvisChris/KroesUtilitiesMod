using HarmonyLib;
using KroesSupermarketMod.CustomScripts;
using StarterAssets;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Events;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class ChatCommandsPatch
    {
        [HarmonyPatch(typeof(PlayerObjectController), "SendChatMsg")]
        [HarmonyPrefix]
        private static bool SendChatMsg_Prefix(PlayerObjectController __instance, ref string message)
        {
            if (Utilities.TryParseCommand(message, out string command, out string[] args))
            {
                switch (command)
                {
                    case "npc":
                        Plugin.mls.LogInfo($"/npc: {string.Join(", ", args)}");
                        Utilities.ChatCommand_Npc(args);
                        return false;

                    case "weather":
                        Plugin.mls.LogInfo($"/weather: {string.Join(", ", args)}");
                        Utilities.ChatCommand_Weather(args);
                        return false;

                    case "notify":
                        Plugin.mls.LogInfo($"/notify: {string.Join(", ", args)}");
                        Utilities.ChatCommand_Notify(args);
                        return false;

                    case "spawn":
                        Plugin.mls.LogInfo($"/spawn: {string.Join(", ", args)}");
                        Utilities.ChatCommand_Spawn(args);
                        return false;

                    default:
                        return true;
                }
            }
            return true;
        }
    }
}
