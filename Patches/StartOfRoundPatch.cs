using HarmonyLib;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void OnStartOfRoundStart(StartOfRound __instance)
        {
            __instance.gameObject.AddComponent<BatteryManager>();
        }
    }
}