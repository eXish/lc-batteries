using BepInEx;
using HarmonyLib;
using LethalLib.Modules;
using UnityEngine;

namespace Batteries
{
    [BepInPlugin(mGUID, mName, mVersion)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class BatteriesMod : BaseUnityPlugin
    {
        const string mGUID = "eXish.Batteries";
        const string mName = "Batteries";
        const string mVersion = "1.0.5";

        readonly Harmony harmony = new Harmony(mGUID);

        internal static BatteriesMod instance;
        internal static AssetBundle bundle;
        internal static AudioClip useBattery;

        void Awake()
        {
            if (instance == null)
                instance = this;

            ConfigManager.Init();
            if (ConfigManager.useBatteryKeybind.Value < 0 || ConfigManager.useBatteryKeybind.Value > 111)
                instance.Logger.LogWarning($"The value \"{ConfigManager.useBatteryKeybind.Value}\" is not valid for setting \"useBatteryKeybind\"! The default will be used instead.");
            if (ConfigManager.batteryChargeAmount.Value < 0f || ConfigManager.batteryChargeAmount.Value > 1f)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryChargeAmount.Value}\" is not valid for setting \"batteryChargeAmount\"! The default will be used instead.");
            if (ConfigManager.batteryRarity.Value < 0 || ConfigManager.batteryRarity.Value > 100)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryRarity.Value}\" is not valid for setting \"batteryRarity\"! The default will be used instead.");
            if (ConfigManager.batteryScrapValue.Value < 0)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryScrapValue.Value}\" is not valid for setting \"batteryScrapValue\"! The default will be used instead.");

            string modLocation = instance.Info.Location.TrimEnd("Batteries.dll".ToCharArray());
            bundle = AssetBundle.LoadFromFile(modLocation + "batteries");
            if (bundle != null)
            {
                useBattery = bundle.LoadAsset<AudioClip>("Assets/Batteries/UseBattery.wav");
                Item battery = bundle.LoadAsset<Item>("Assets/Batteries/Battery.asset");
                Utilities.FixMixerGroups(battery.spawnPrefab);
                NetworkPrefabs.RegisterNetworkPrefab(battery.spawnPrefab);
                if (ConfigManager.batteryRarity.Value < 0 || ConfigManager.batteryRarity.Value > 100)
                    Items.RegisterScrap(battery, (int)ConfigManager.batteryRarity.DefaultValue, Levels.LevelTypes.All);
                else if (ConfigManager.batteryRarity.Value != 0)
                    Items.RegisterScrap(battery, ConfigManager.batteryRarity.Value, Levels.LevelTypes.All);
                if (ConfigManager.batteryShopValue.Value > 0)
                {
                    TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                    node.clearPreviousText = true;
                    node.displayText = "Are you tired of having to travel all the way back to your ship to charge equipment? Well fear not! Durable's new battery line will solve all your charging needs! Get the power you deserve right when you need it!\n\nDurable is not responsible for any injury or death caused by our product. All items purchased are non-refundable.\n\n";
                    Items.RegisterShopItem(battery, null, null, node, ConfigManager.batteryShopValue.Value);
                }
            }
            else
                instance.Logger.LogError($"Unable to locate the asset file! Batteries will not spawn or appear in the shop.");

            harmony.PatchAll();

            instance.Logger.LogInfo($"{mName}-{mVersion} loaded!");
        }
    }
}
