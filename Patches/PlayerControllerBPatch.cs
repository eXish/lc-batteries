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
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlayerUpdatePatch(ref PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || __instance.currentlyHeldObjectServer == null || __instance.inSpecialInteractAnimation)
                return;
            int useKey = ConfigManager.useBatteryKeybind.Value;
            if (useKey < 0 || useKey > 111)
                useKey = (int)ConfigManager.useBatteryKeybind.DefaultValue;
            if (__instance.currentlyHeldObjectServer.itemProperties.requiresBattery && __instance.currentlyHeldObjectServer.insertedBattery.charge < 1f && Keyboard.current[(Key)useKey].wasPressedThisFrame)
            {
                for (int i = 0; i < __instance.ItemSlots.Length; i++)
                {
                    if (__instance.ItemSlots[i] != null && __instance.ItemSlots[i].itemProperties.itemId == 5601)
                    {
                        __instance.currentlyHeldObjectServer.GetComponent<AudioSource>().PlayOneShot(BatteriesMod.useBattery);
                        __instance.DestroyItemInSlotAndSync(i);
                        if (ConfigManager.batteryChargeAmount.Value < 0f || ConfigManager.batteryChargeAmount.Value > 1f)
                            __instance.currentlyHeldObjectServer.insertedBattery.charge += (float)ConfigManager.batteryChargeAmount.DefaultValue;
                        else
                            __instance.currentlyHeldObjectServer.insertedBattery.charge += ConfigManager.batteryChargeAmount.Value;
                        if (__instance.currentlyHeldObjectServer.insertedBattery.charge > 1f)
                            __instance.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                        break;
                    }
                }
            }

            if (ConfigManager.toolTipEnabled.Value && __instance.currentlyHeldObjectServer.itemProperties.requiresBattery)
            {
                if (__instance.currentlyHeldObjectServer.insertedBattery.charge < 1f && !__instance.currentlyHeldObjectServer.itemProperties.toolTips.Any(x => x.StartsWith("Use Battery : [")) && __instance.ItemSlots.Where(x => x != null).Any(x => x.itemProperties.itemId == 5601))
                {
                    string[] toolTips = __instance.currentlyHeldObjectServer.itemProperties.toolTips;
                    string[] newToolTips = new string[toolTips.Length + 1];
                    for (int i = 0; i < newToolTips.Length; i++)
                    {
                        if (i != newToolTips.Length - 1)
                            newToolTips[i] = toolTips[i];
                        else
                            newToolTips[i] = "Use Battery : [" + (Key)useKey + "]";
                    }
                    __instance.currentlyHeldObjectServer.itemProperties.toolTips = newToolTips;
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
    }
}
