using HarmonyLib;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class ItemChargerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void ChargerUpdatePatch(ref ItemCharger __instance)
        {
            if (__instance.gameObject.activeSelf && ConfigManager.disableChargingCoil.Value)
                __instance.gameObject.SetActive(false);
        }
    }
}
