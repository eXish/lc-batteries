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
                        BatteryManager.Instance.ReloadBatteryAction.bindings.FirstOrDefault(binding => binding.path
                            .ToLowerInvariant().Contains(
                                StartOfRound.Instance.localPlayerUsingController ? "<gamepad>" : "<keyboard>"
                            ));

                    var bindingText = binding == default ? "?" : binding.ToDisplayString();

                    __instance.currentlyHeldObjectServer.itemProperties.toolTips =
                    [
                        ..__instance.currentlyHeldObjectServer.itemProperties.toolTips.Where(x => !x.StartsWith("Use Battery : [")),
                        $"Use Battery : [{bindingText}]"
                    ];

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
            return StartOfRound.Instance.localPlayerController.ItemSlots.Any(x => x && x.itemProperties.itemId == 5601);
        }
        
        private static bool BatteryTooltipShown()
        {
            return StartOfRound.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.toolTips.Any(tooltip =>
                tooltip.StartsWith("Use Battery : ["));
        }
    }
}
