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
        const string mVersion = "1.1.0";

        readonly Harmony harmony = new Harmony(mGUID);

        internal static BatteriesMod instance;
        internal static AssetBundle bundle;
        internal static AudioClip useBattery;
        internal static Item batteryItem;

        void Awake()
        {
            if (instance == null)
                instance = this;

            ConfigManager.Init();
            if (string.IsNullOrEmpty(ConfigManager.useBatteryKeybinds.Value))
                instance.Logger.LogWarning($"The value \"{ConfigManager.useBatteryKeybinds.Value}\" is not valid for setting \"useBatteryKeybinds\"! The default will be used instead.");
            if (ConfigManager.batteryChargeAmount.Value < 0f || ConfigManager.batteryChargeAmount.Value > 1f)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryChargeAmount.Value}\" is not valid for setting \"batteryChargeAmount\"! The default will be used instead.");
            if (ConfigManager.batteryRarity.Value < 0 || ConfigManager.batteryRarity.Value > 100)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryRarity.Value}\" is not valid for setting \"batteryRarity\"! The default will be used instead.");
            if (ConfigManager.batteryMaxSpawns.Value <= 0)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryMaxSpawns.Value}\" is not valid for setting \"batteryMaxSpawns\"! The default will be used instead.");
            if (ConfigManager.batteryScrapValue.Value < 0)
                instance.Logger.LogWarning($"The value \"{ConfigManager.batteryScrapValue.Value}\" is not valid for setting \"batteryScrapValue\"! The default will be used instead.");

            string modLocation = instance.Info.Location.TrimEnd("Batteries.dll".ToCharArray());
            bundle = AssetBundle.LoadFromFile(modLocation + "batteries");
            if (bundle != null)
            {
                useBattery = bundle.LoadAsset<AudioClip>("Assets/Batteries/UseBattery.wav");
                batteryItem = bundle.LoadAsset<Item>("Assets/Batteries/Battery.asset");
                if (ConfigManager.batteryRarity.Value != 0)
                {
                    Utilities.FixMixerGroups(batteryItem.spawnPrefab);
                    NetworkPrefabs.RegisterNetworkPrefab(batteryItem.spawnPrefab);
                    Items.RegisterItem(batteryItem);
                }
                if (ConfigManager.batteryShopValue.Value > 0)
                {
                    Item storeBattery = bundle.LoadAsset<Item>("Assets/Batteries/StoreBattery.asset");
                    Utilities.FixMixerGroups(storeBattery.spawnPrefab);
                    NetworkPrefabs.RegisterNetworkPrefab(storeBattery.spawnPrefab);
                    TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                    node.clearPreviousText = true;
                    node.displayText = "Are you tired of having to travel all the way back to your ship to charge equipment? Well fear not! Durable's new battery line will solve all your charging needs! Get the power you deserve right when you need it!\n\nDurable is not responsible for any injury or death caused by our product. All items purchased are non-refundable.\n\n";
                    Items.RegisterShopItem(storeBattery, null, null, node, ConfigManager.batteryShopValue.Value);
                }
            }
            else
                instance.Logger.LogError($"Unable to locate the asset file! Batteries will not spawn or appear in the shop.");

            harmony.PatchAll();

            instance.Logger.LogInfo($"{mName}-{mVersion} loaded!");
        }
    }
}
