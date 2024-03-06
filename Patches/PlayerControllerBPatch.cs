using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        private static InputAction reloadBatteryAction;
        private static InputControl inputControl;
        
        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        static void PlayerOnEnablePatch(ref PlayerControllerB __instance)
        {
            // Only apply to local player
            if (__instance != StartOfRound.Instance.localPlayerController)
                return;

            reloadBatteryAction = new InputAction("ReloadBattery");

            if (string.IsNullOrEmpty(ConfigManager.useBatteryKeybinds.Value))
                ConfigManager.useBatteryKeybinds.Value = ConfigManager.DEFAULT_KEYBINDS;
            
            foreach (var bindingPath in ConfigManager.useBatteryKeybinds.Value.Split(","))
            {
                reloadBatteryAction.AddBinding(bindingPath);
            }
            
            reloadBatteryAction.Enable();
            reloadBatteryAction.performed += ReloadBatteryActionPerformed;
        }
        
        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        static void PlayerOnDisablePatch(ref PlayerControllerB __instance)
        {
            // Only apply to local player
            if (__instance != StartOfRound.Instance.localPlayerController)
                return;

            if (reloadBatteryAction is null)
                return;

            reloadBatteryAction.performed -= ReloadBatteryActionPerformed;
            reloadBatteryAction.Disable();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlayerUpdatePatch(ref PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || __instance.currentlyHeldObjectServer == null || __instance.inSpecialInteractAnimation)
                return;
            
            // Attempt to get latest "control"
            var currentControl = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Move")?.activeControl;
            if (currentControl != null)
            {
                inputControl = currentControl;
            }
            
            if (ConfigManager.toolTipEnabled.Value && __instance.currentlyHeldObjectServer.itemProperties.requiresBattery)
            {
                if (__instance.currentlyHeldObjectServer.insertedBattery.charge < 1f && !__instance.currentlyHeldObjectServer.itemProperties.toolTips.Any(x => x.StartsWith("Use Battery : [")) && __instance.ItemSlots.Where(x => x != null).Any(x => x.itemProperties.itemId == 5601))
                {
                    __instance.currentlyHeldObjectServer.itemProperties.toolTips =
                    [
                        ..__instance.currentlyHeldObjectServer.itemProperties.toolTips,
                        // TODO: Determine binding (if possible)
                        "Use Battery : [C]"
                    ];
                    
                    HUDManager.Instance.ClearControlTips();
                    __instance.currentlyHeldObjectServer.SetControlTipsForItem();
                }

                if (__instance.currentlyHeldObjectServer.itemProperties.toolTips.Any(x => x.StartsWith("Use Battery : [")) && (__instance.currentlyHeldObjectServer.insertedBattery.charge >= 1f || !__instance.ItemSlots.Where(x => x != null).Any(x => x.itemProperties.itemId == 5601)))
                {
                    __instance.currentlyHeldObjectServer.itemProperties.toolTips = __instance.currentlyHeldObjectServer.itemProperties.toolTips.Where(x => !x.StartsWith("Use Battery : [")).ToArray();
                    HUDManager.Instance.ClearControlTips();
                    __instance.currentlyHeldObjectServer.SetControlTipsForItem();
                }
            }
        }

        private static void ReloadBatteryActionPerformed(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var player = StartOfRound.Instance.localPlayerController;
            
            // Don't recharge if not holding item or while being busy performing an animation
            if (player.currentlyHeldObjectServer == null || player.inSpecialInteractAnimation)
                return;
            
            if (player.currentlyHeldObjectServer.itemProperties.requiresBattery && player.currentlyHeldObjectServer.insertedBattery.charge < 1f)
            {
                for (int i = 0; i < player.ItemSlots.Length; i++)
                {
                    if (player.ItemSlots[i] != null && player.ItemSlots[i].itemProperties.itemId == 5601)
                    {
                        player.currentlyHeldObjectServer.GetComponent<AudioSource>().PlayOneShot(BatteriesMod.useBattery);
                        player.DestroyItemInSlotAndSync(i);
                        if (ConfigManager.batteryChargeAmount.Value < 0f || ConfigManager.batteryChargeAmount.Value > 1f)
                            player.currentlyHeldObjectServer.insertedBattery.charge += (float)ConfigManager.batteryChargeAmount.DefaultValue;
                        else
                            player.currentlyHeldObjectServer.insertedBattery.charge += ConfigManager.batteryChargeAmount.Value;
                        if (player.currentlyHeldObjectServer.insertedBattery.charge > 1f)
                            player.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                        break;
                    }
                }
            }
        }
    }
}
