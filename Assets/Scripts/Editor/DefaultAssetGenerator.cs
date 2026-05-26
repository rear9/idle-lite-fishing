using UnityEditor;
using UnityEngine;

public static class DefaultAssetGenerator
{
    [MenuItem("Tools/Fishing/Generate Default Assets")]
    private static void GenerateAll()
    {
        GenerateEconomyBalance();
        GenerateFishSpecies();
        AssetDatabase.SaveAssets();

        GenerateMaterials();
        GenerateZone();
        AssetDatabase.SaveAssets();

        GenerateNetTiers();
        GenerateNetRewardTables();
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        Debug.Log("[DefaultAssetGenerator] All default assets created.");
    }

    private static void GenerateEconomyBalance()
    {
        EconomyBalanceData econ = ScriptableObject.CreateInstance<EconomyBalanceData>();
        AssetDatabase.CreateAsset(econ, "Assets/ScriptableObjects/Economy/EconomyBalanceData.asset");
    }

    private static void GenerateFishSpecies()
    {
        CreateFish("sardine", "Sardine", FishRarity.Common, 2f, 0.05f, 0.15f, 10f, 20f, 0.1f, 1, true, FishOutputType.Egg, 120f, 5f);
        CreateFish("mackerel", "Mackerel", FishRarity.Common, 3f, 0.2f, 0.5f, 20f, 35f, 0.15f, 1, true, FishOutputType.Egg, 120f, 6f);
        CreateFish("bass", "Bass", FishRarity.Uncommon, 8f, 0.5f, 2.0f, 30f, 55f, 0.3f, 1, true, FishOutputType.Egg, 180f, 18f);
        CreateFish("cod", "Cod", FishRarity.Uncommon, 10f, 1.0f, 4.0f, 40f, 80f, 0.35f, 1, true, FishOutputType.Egg, 180f, 22f);
        CreateFish("tuna", "Tuna", FishRarity.Rare, 30f, 5.0f, 30.0f, 80f, 200f, 0.6f, 2, true, FishOutputType.Egg, 300f, 80f);
        CreateFish("salmon", "Salmon", FishRarity.Rare, 25f, 2.0f, 12.0f, 60f, 120f, 0.55f, 2, true, FishOutputType.Egg, 300f, 70f);
        CreateFish("swordfish", "Swordfish", FishRarity.Epic, 80f, 20.0f, 120.0f, 150f, 400f, 0.8f, 3, true, FishOutputType.Egg, 480f, 220f);
        CreateFish("shark", "Shark", FishRarity.Legendary, 200f, 80.0f, 400.0f, 200f, 600f, 0.95f, 4, true, FishOutputType.BabyFish, 600f, 600f);
    }

    private static void CreateFish(
        string id, string name, FishRarity rarity, float saleValue,
        float minW, float maxW, float minL, float maxL,
        float difficulty, int rodPower, bool canHouse,
        FishOutputType outputType, float prodTime, float outputVal)
    {
        FishSpeciesData fish = ScriptableObject.CreateInstance<FishSpeciesData>();
        fish.speciesId = id;
        fish.displayName = name;
        fish.rarity = rarity;
        fish.baseSaleValue = saleValue;
        fish.minWeight = minW;
        fish.maxWeight = maxW;
        fish.minLength = minL;
        fish.maxLength = maxL;
        fish.baseDifficulty = difficulty;
        fish.requiredRodPower = rodPower;
        fish.canBeHoused = canHouse;
        fish.outputType = outputType;
        fish.baseProductionTime = prodTime;
        fish.baseOutputValue = outputVal;
        AssetDatabase.CreateAsset(fish, $"Assets/ScriptableObjects/Fish/{id}.asset");
    }

    private static void GenerateMaterials()
    {
        CreateMaterial("wood", "Wood", 1f, 1);
        CreateMaterial("spring", "Spring", 3f, 1);
        CreateMaterial("plastic_bottle", "Plastic Bottle", 1f, 1);
        CreateMaterial("scrap_metal", "Scrap Metal", 4f, 2);
        CreateMaterial("rope", "Rope", 2f, 1);
    }

    private static void CreateMaterial(string id, string name, float saleValue, int tier)
    {
        MaterialData mat = ScriptableObject.CreateInstance<MaterialData>();
        mat.materialId = id;
        mat.displayName = name;
        mat.saleValue = saleValue;
        mat.tier = tier;
        AssetDatabase.CreateAsset(mat, $"Assets/ScriptableObjects/Materials/{id}.asset");
    }

    private static void GenerateZone()
    {
        ZoneData zone = ScriptableObject.CreateInstance<ZoneData>();
        zone.zoneId = "calm_coastal_waters";
        zone.displayName = "Calm Coastal Waters";

        // Weighted by rarity: common > uncommon > rare > epic > legendary
        zone.availableFish = new System.Collections.Generic.List<ZoneFishEntry>
        {
            new ZoneFishEntry { species = LoadFish("sardine"), spawnWeight = 30f },
            new ZoneFishEntry { species = LoadFish("mackerel"), spawnWeight = 25f },
            new ZoneFishEntry { species = LoadFish("bass"), spawnWeight = 20f },
            new ZoneFishEntry { species = LoadFish("cod"), spawnWeight = 15f },
            new ZoneFishEntry { species = LoadFish("salmon"), spawnWeight = 5f },
            new ZoneFishEntry { species = LoadFish("tuna"), spawnWeight = 3f },
            new ZoneFishEntry { species = LoadFish("swordfish"), spawnWeight = 1.5f },
            new ZoneFishEntry { species = LoadFish("shark"), spawnWeight = 0.5f },
        };

        zone.unlockCondition = "";

        AssetDatabase.CreateAsset(zone, "Assets/ScriptableObjects/Zones/calm_coastal_waters.asset");
    }

    private static FishSpeciesData LoadFish(string id)
    {
        return AssetDatabase.LoadAssetAtPath<FishSpeciesData>($"Assets/ScriptableObjects/Fish/{id}.asset");
    }

    private static void GenerateNetTiers()
    {
        System.IO.Directory.CreateDirectory("Assets/ScriptableObjects/Nets");
        CreateNetTier("net_tier_1", "Basic Net", 1, 10, 1.0f, 0, 100f);
        CreateNetTier("net_tier_2", "Reinforced Net", 2, 20, 1.5f, 5, 500f);
        CreateNetTier("net_tier_3", "Advanced Net", 3, 30, 2.0f, 10, 2000f);
    }

    private static void CreateNetTier(string id, string name, int tier, int capacity, float fillRate, int rodPower, float upgradeCost)
    {
        NetTierData data = ScriptableObject.CreateInstance<NetTierData>();
        data.tier = tier;
        data.displayName = name;
        data.capacity = capacity;
        data.fillRateMultiplier = fillRate;
        data.requiredRodPower = rodPower;
        data.upgradeCost = upgradeCost;
        AssetDatabase.CreateAsset(data, $"Assets/ScriptableObjects/Nets/{id}.asset");
    }

    private static void GenerateNetRewardTables()
    {
        CreateNetRewardTable("reward_table_tier_1", "net_tier_1", new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "sardine", type = RewardType.Fish, weight = 25f },
            new WeightedRewardEntry { rewardId = "mackerel", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 30f },
            new WeightedRewardEntry { rewardId = "rope", type = RewardType.Material, weight = 20f },
            new WeightedRewardEntry { rewardId = "plastic_bottle", type = RewardType.Material, weight = 25f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "sardine", type = RewardType.Fish, weight = 35f },
            new WeightedRewardEntry { rewardId = "mackerel", type = RewardType.Fish, weight = 30f },
            new WeightedRewardEntry { rewardId = "bass", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "cod", type = RewardType.Fish, weight = 15f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 35f },
            new WeightedRewardEntry { rewardId = "rope", type = RewardType.Material, weight = 25f },
            new WeightedRewardEntry { rewardId = "plastic_bottle", type = RewardType.Material, weight = 30f },
            new WeightedRewardEntry { rewardId = "scrap_metal", type = RewardType.Material, weight = 10f },
        });

        CreateNetRewardTable("reward_table_tier_2", "net_tier_2", new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "sardine", type = RewardType.Fish, weight = 10f },
            new WeightedRewardEntry { rewardId = "mackerel", type = RewardType.Fish, weight = 15f },
            new WeightedRewardEntry { rewardId = "bass", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "cod", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "salmon", type = RewardType.Fish, weight = 10f },
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 15f },
            new WeightedRewardEntry { rewardId = "scrap_metal", type = RewardType.Material, weight = 10f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "mackerel", type = RewardType.Fish, weight = 15f },
            new WeightedRewardEntry { rewardId = "bass", type = RewardType.Fish, weight = 25f },
            new WeightedRewardEntry { rewardId = "cod", type = RewardType.Fish, weight = 25f },
            new WeightedRewardEntry { rewardId = "salmon", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "tuna", type = RewardType.Fish, weight = 15f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 25f },
            new WeightedRewardEntry { rewardId = "scrap_metal", type = RewardType.Material, weight = 40f },
            new WeightedRewardEntry { rewardId = "rope", type = RewardType.Material, weight = 15f },
            new WeightedRewardEntry { rewardId = "spring", type = RewardType.Material, weight = 20f },
        });

        CreateNetRewardTable("reward_table_tier_3", "net_tier_3", new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "bass", type = RewardType.Fish, weight = 10f },
            new WeightedRewardEntry { rewardId = "cod", type = RewardType.Fish, weight = 15f },
            new WeightedRewardEntry { rewardId = "salmon", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "tuna", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "swordfish", type = RewardType.Fish, weight = 10f },
            new WeightedRewardEntry { rewardId = "scrap_metal", type = RewardType.Material, weight = 10f },
            new WeightedRewardEntry { rewardId = "spring", type = RewardType.Material, weight = 10f },
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 5f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "salmon", type = RewardType.Fish, weight = 20f },
            new WeightedRewardEntry { rewardId = "tuna", type = RewardType.Fish, weight = 25f },
            new WeightedRewardEntry { rewardId = "swordfish", type = RewardType.Fish, weight = 25f },
            new WeightedRewardEntry { rewardId = "shark", type = RewardType.Fish, weight = 10f },
            new WeightedRewardEntry { rewardId = "cod", type = RewardType.Fish, weight = 20f },
        }, new System.Collections.Generic.List<WeightedRewardEntry>
        {
            new WeightedRewardEntry { rewardId = "scrap_metal", type = RewardType.Material, weight = 30f },
            new WeightedRewardEntry { rewardId = "spring", type = RewardType.Material, weight = 35f },
            new WeightedRewardEntry { rewardId = "wood", type = RewardType.Material, weight = 10f },
            new WeightedRewardEntry { rewardId = "rope", type = RewardType.Material, weight = 10f },
            new WeightedRewardEntry { rewardId = "plastic_bottle", type = RewardType.Material, weight = 15f },
        });
    }

    private static void CreateNetRewardTable(string id, string tierId,
        System.Collections.Generic.List<WeightedRewardEntry> balanced,
        System.Collections.Generic.List<WeightedRewardEntry> fishFocused,
        System.Collections.Generic.List<WeightedRewardEntry> materialFocused)
    {
        NetRewardTableData table = ScriptableObject.CreateInstance<NetRewardTableData>();
        table.tier = AssetDatabase.LoadAssetAtPath<NetTierData>($"Assets/ScriptableObjects/Nets/{tierId}.asset");
        table.balancedTable = balanced;
        table.fishFocusedTable = fishFocused;
        table.materialFocusedTable = materialFocused;
        AssetDatabase.CreateAsset(table, $"Assets/ScriptableObjects/Nets/{id}.asset");
    }
}
