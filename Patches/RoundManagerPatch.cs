using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace Batteries.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("SpawnScrapInLevel")]
        [HarmonyPostfix]
        static void SpawnScrapPatch(ref RoundManager __instance)
        {
            int rarity = ConfigManager.batteryRarity.Value;
            if (rarity < 0 || rarity > 100)
                rarity = (int)ConfigManager.batteryRarity.DefaultValue;
            if (rarity != 0)
            {
                int maxSpawns = ConfigManager.batteryMaxSpawns.Value;
                if (maxSpawns <= 0)
                    maxSpawns = (int)ConfigManager.batteryMaxSpawns.DefaultValue;
                for (int i = 0; i < maxSpawns; i++)
                {
                    int rando = __instance.AnomalyRandom.Next(1, 151);
                    if (rando <= rarity)
                    {
                        int rando2 = __instance.AnomalyRandom.Next(0, __instance.insideAINodes.Length);
                        Vector3 randomNavMeshPositionInBoxPredictable = __instance.GetRandomNavMeshPositionInBoxPredictable(__instance.insideAINodes[rando2].transform.position, 8f, __instance.navHit, __instance.AnomalyRandom, -1);
                        Object.Instantiate(BatteriesMod.batteryItem.spawnPrefab, randomNavMeshPositionInBoxPredictable, Quaternion.identity, __instance.spawnedScrapContainer).GetComponent<NetworkObject>().Spawn();
                    }
                }
            }
        }
    }
}