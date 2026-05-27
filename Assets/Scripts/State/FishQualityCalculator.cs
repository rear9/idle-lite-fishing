using UnityEngine;

public static class FishQualityCalculator
{
    public static int CalculateQuality(FishSpeciesData sp, float weight, float length, int seed)
    {
        float rarity = sp.rarity.ToFloat();
        float w = Mathf.InverseLerp(sp.minWeight, sp.maxWeight, weight);
        float l = Mathf.InverseLerp(sp.minLength, sp.maxLength, length);
        float variance = (Seeded(seed) * 2f - 1f) * 0.1f;

        float raw = 0.4f * rarity
                  + 0.3f * w
                  + 0.3f * l
                  + variance;

        return Mathf.Clamp(Mathf.RoundToInt(raw * 10f), 1, 10);
    }

    private static float Seeded(int seed) => (float)(new System.Random(seed).NextDouble());
}

public static class FishRarityExtensions
{
    public static float ToFloat(this FishRarity r) => r switch
    {
        FishRarity.Common => 0.1f,
        FishRarity.Uncommon => 0.3f,
        FishRarity.Rare => 0.5f,
        FishRarity.Epic => 0.75f,
        FishRarity.Legendary => 1.0f,
        _ => 0.1f
    };
}
