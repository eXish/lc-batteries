﻿using BepInEx.Configuration;

namespace Batteries
{
    internal class ConfigManager
    {
        public static ConfigEntry<bool> toolTipEnabled;
        public static ConfigEntry<int> useBatteryKeybind;

        public static ConfigEntry<float> batteryChargeAmount;
        public static ConfigEntry<int> batteryRarity;
        public static ConfigEntry<int> batteryScrapValue;

        public static void Init()
        {
            toolTipEnabled = BatteriesMod.instance.Config.Bind("General Settings", "toolTipEnabled", true, "Allows the mod to add/remove a tool tip to battery powered items when necessary.");
            useBatteryKeybind = BatteriesMod.instance.Config.Bind("General Settings", "useBatteryKeybind", 17, "The value for the keybind to use a battery. Values for keybinds can be found here: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html.");

            batteryChargeAmount = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryChargeAmount", 0.25f, "The amount that batteries charge battery powered items. Can be set to a minimum of 0 (0% charge) and a maximum of 1 (100% charge).");
            batteryRarity = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryRarity", 70, "How rare it is for a battery to spawn. Can be set to a minimum of 1 (rare) and a maximum of 100 (common).");
            batteryScrapValue = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryScrapValue", 5, "The scrap value of batteries.");
        }
    }
}