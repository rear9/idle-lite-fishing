using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct FishEncounter
{
    public FishSpeciesData species;
    public float difficulty;
    public bool isReachable;
}

public class FishingSystem : MonoBehaviour
{
    private GameState _state;
    private InventorySystem _inventory;
    private EconomyBalanceData _economy;
    private static readonly string[] junk = {"plastic_bottle", "rope", "wood"};

    public void Initialize(GameState state, InventorySystem inventory, EconomyBalanceData economy)
    {
        _state = state;
        _inventory = inventory;
        _economy = economy;
    }

    public FishEncounter? StartFishing()
    {
        ZoneData zone = GameManager.Instance.GetZoneData(_state.currentZoneId);
        if (zone == null)
        {
            Debug.LogWarning($"[Fishing] Zone '{_state.currentZoneId}' not found.");
            return null;
        }
        if (zone.availableFish == null || zone.availableFish.Count == 0)
        {
            Debug.LogWarning($"[Fishing] Zone '{zone.displayName}' has no fish.");
            return null;
        }

        var reachable = zone.availableFish
            .Where(f => f.species.requiredRodPower <= _state.upgrades.currentRodPower)
            .ToList();

        var pool = reachable.Count > 0 ? reachable : zone.availableFish;
        FishSpeciesData species = WeightedPick(pool);
        if (species == null) return null;
        
        return new FishEncounter
        {
            species = species,
            difficulty = species.baseDifficulty,
            isReachable = reachable.Count > 0
        };
    }

    public void SubmitCatchResult(FishEncounter enc, bool success, float perfScore)
    {
        if (success)
        {
            CreateFish(enc.species, perfScore);
        }
        else if (Random.value < _economy.failedFishingJunkChance)
        {
            _inventory.AddMaterial(junk[Random.Range(0, junk.Length)], 1);
        }
    }

    private void CreateFish(FishSpeciesData sp, float perf)
    {
        float weight = Random.Range(sp.minWeight, sp.maxWeight); // pre-rolled
        float length = Random.Range(sp.minLength, sp.maxLength);
        int seed = Random.Range(0, 10000);
        
        int baseQuality = FishQualityCalculator.CalculateQuality(sp, weight, length, seed);
        int bonusQuality = perf >= 0.8f ? 1 : 0; // bonus point for a clean catch
        int finalQuality = Mathf.Clamp(baseQuality + bonusQuality, 1, 10);

        FishInstance fish = new FishInstance
        {
            instanceId = Guid.NewGuid().ToString(),
            speciesId = sp.speciesId,
            rarity = sp.rarity,
            weight = weight,
            length = length,
            randomSeed = seed,
            quality = finalQuality,
            state = FishState.Inventory
        };

        _inventory.AddFish(fish);
        EventBus.FishCaught(fish);
    }

    private FishSpeciesData WeightedPick(System.Collections.Generic.List<ZoneFishEntry> entries)
    {
        float total = entries.Sum(e => e.spawnWeight);
        if (total <= 0f) return null;

        float roll = Random.value * total;
        float acc = 0f;
        foreach (var e in entries)
        {
            acc += e.spawnWeight;
            if (roll <= acc) return e.species;
        }
        return entries.Last().species;
    }
}
