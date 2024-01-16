using HarmonyLib;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void ObjUpdatePatch(ref GrabbableObject __instance)
        {
            if (__instance.itemProperties.itemId == 5601)
            {
                if (ConfigManager.batteryScrapValue.Value < 0)
                    __instance.SetScrapValue((int)ConfigManager.batteryScrapValue.DefaultValue);
                else
                    __instance.SetScrapValue(ConfigManager.batteryScrapValue.Value);
            }
        }
    }
}