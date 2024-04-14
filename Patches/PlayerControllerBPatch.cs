using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        private static bool wasUsingController;
        
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlayerUpdatePatch(ref PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || __instance.currentlyHeldObjectServer == null || __instance.inSpecialInteractAnimation)
                return;
            
            if (ConfigManager.toolTipEnabled.Value && __instance.currentlyHeldObjectServer.itemProperties.requiresBattery)
            {
                if (__instance.currentlyHeldObjectServer.insertedBattery.charge < 1f &&
                    (!BatteryTooltipShown() ||
                     wasUsingController != StartOfRound.Instance.localPlayerUsingController) && BatteryInInventory())
                {
                    var binding =
                        BatteryManager.Instance.ReloadBatteryAction.bindings.FirstOrDefault(x => x.path
                            .ToLowerInvariant().Contains(
                                StartOfRound.Instance.localPlayerUsingController ? "<gamepad>" : "<keyboard>"
                            ));

                    var bindingText = binding == default ? "?" : binding.ToDisplayString();

                    string[] toolTips = __instance.currentlyHeldObjectServer.itemProperties.toolTips;
                    string[] newToolTips = new string[toolTips.Length + 1];
                    for (int i = 0; i < newToolTips.Length; i++)
                    {
                        if (i != newToolTips.Length - 1)
                            newToolTips[i] = toolTips[i];
                        else
                            newToolTips[i] = $"Use Battery : [{bindingText}]";
                    }
                    __instance.currentlyHeldObjectServer.itemProperties.toolTips = newToolTips;

                    HUDManager.Instance.ClearControlTips();
                    __instance.currentlyHeldObjectServer.SetControlTipsForItem();

                    wasUsingController = StartOfRound.Instance.localPlayerUsingController;
                }

                if (BatteryTooltipShown() && (__instance.currentlyHeldObjectServer.insertedBattery.charge >= 1f || !BatteryInInventory()))
                {
                    __instance.currentlyHeldObjectServer.itemProperties.toolTips = __instance.currentlyHeldObjectServer.itemProperties.toolTips.Where(x => !x.StartsWith("Use Battery : [")).ToArray();
                    
                    HUDManager.Instance.ClearControlTips();
                    __instance.currentlyHeldObjectServer.SetControlTipsForItem();
                }
            }
        }

        private static bool BatteryInInventory()
        {
            return StartOfRound.Instance.localPlayerController.ItemSlots.Any(x => x && (x.itemProperties.itemId == 5601 || x.itemProperties.itemId == 5602));
        }
        
        private static bool BatteryTooltipShown()
        {
            return StartOfRound.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.toolTips.Any(tooltip =>
                tooltip.StartsWith("Use Battery : ["));
        }
    }
}
