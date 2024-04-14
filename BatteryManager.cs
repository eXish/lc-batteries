using UnityEngine;
using UnityEngine.InputSystem;

namespace Batteries
{
    public class BatteryManager : MonoBehaviour
    {
        public static BatteryManager Instance { get; private set; }
        
        public InputAction ReloadBatteryAction { get; private set; }

        private void Awake()
        {
            Instance = this;
            
            ReloadBatteryAction = new InputAction("ReloadBattery");

            if (string.IsNullOrEmpty(ConfigManager.useBatteryKeybinds.Value))
                ConfigManager.useBatteryKeybinds.Value = ConfigManager.DEFAULT_KEYBINDS;
            
            foreach (var bindingPath in ConfigManager.useBatteryKeybinds.Value.Split(','))
            {
                ReloadBatteryAction.AddBinding(bindingPath);
            }
            
            ReloadBatteryAction.Enable();
            ReloadBatteryAction.performed += ReloadBatteryActionPerformed;
        }

        private void OnDestroy()
        {
            ReloadBatteryAction.performed -= ReloadBatteryActionPerformed;
            ReloadBatteryAction.Disable();
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
                    if (player.ItemSlots[i] != null && (player.ItemSlots[i].itemProperties.itemId == 5601 || player.ItemSlots[i].itemProperties.itemId == 5602))
                    {
                        player.currentlyHeldObjectServer.GetComponent<AudioSource>().PlayOneShot(BatteriesMod.useBattery);
                        player.DestroyItemInSlotAndSync(i);
                        if (ConfigManager.batteryChargeAmount.Value < 0f || ConfigManager.batteryChargeAmount.Value > 1f)
                            player.currentlyHeldObjectServer.insertedBattery.charge += (float)ConfigManager.batteryChargeAmount.DefaultValue;
                        else
                            player.currentlyHeldObjectServer.insertedBattery.charge += ConfigManager.batteryChargeAmount.Value;
                        if (player.currentlyHeldObjectServer.insertedBattery.charge > 1f)
                            player.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                        if (player.currentlyHeldObjectServer.insertedBattery.empty)
                            player.currentlyHeldObjectServer.insertedBattery.empty = false;
                        break;
                    }
                }
            }
        }
    }
}