using BepInEx.Configuration;

namespace Batteries
{
    internal class ConfigManager
    {
        // TODO: Add gamepad
        public const string DEFAULT_KEYBINDS = "<Keyboard>/c,<Gamepad>/dpad/up,<XRController>{RightHand}/gripButton";
        
        public static ConfigEntry<bool> toolTipEnabled;
        public static ConfigEntry<bool> disableChargingCoil;
        public static ConfigEntry<string> useBatteryKeybinds;

        public static ConfigEntry<float> batteryChargeAmount;
        public static ConfigEntry<int> batteryRarity;
        public static ConfigEntry<int> batteryScrapValue;
        public static ConfigEntry<int> batteryShopValue;

        public static void Init()
        {
            toolTipEnabled = BatteriesMod.instance.Config.Bind("General Settings", "toolTipEnabled", true, "Allows the mod to add/remove a tool tip to battery powered items when necessary.");
            disableChargingCoil = BatteriesMod.instance.Config.Bind("General Settings", "disableChargingCoil", false, "Disables the charging coil. Mainly for people who wish to have batteries as the only charge source.");
            useBatteryKeybinds = BatteriesMod.instance.Config.Bind("General Settings", "useBatteryKeybinds", DEFAULT_KEYBINDS, "The value for the keybinds to use a battery, split by comma. Values for keybinds can be found here: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html.");

            batteryChargeAmount = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryChargeAmount", 0.25f, "The amount that batteries charge battery powered items. Can be set to a minimum of 0 (0% charge) and a maximum of 1 (100% charge).");
            batteryRarity = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryRarity", 70, "How rare it is for a battery to spawn. Can be set to a minimum of 0 (cannot spawn) and a maximum of 100 (common).");
            batteryScrapValue = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryScrapValue", 5, "The scrap value of batteries.");
            batteryShopValue = BatteriesMod.instance.Config.Bind("Battery Settings", "batteryShopValue", 0, "How much batteries cost in the shop. When set to 0 or less batteries will not appear in the shop.");
        }
    }
}
