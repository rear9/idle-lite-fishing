using UnityEngine;

public enum FishRarity { Common, Uncommon, Rare, Epic, Legendary }
public enum FishOutputType { Egg, BabyFish }
public enum FishState { Inventory, Housed, Sold }

[CreateAssetMenu(menuName = "Fishing/Fish Species", fileName = "FishSpecies")]
public class FishSpeciesData : ScriptableObject
{
    public string speciesId;
    public string displayName;
    public FishRarity rarity;
    public float baseSaleValue;
    public float minWeight;
    public float maxWeight;
    public float minLength;
    public float maxLength;
    [Range(0f, 1f)] public float baseDifficulty;
    public int requiredRodPower;
    public bool canBeHoused;
    public FishOutputType outputType;
    public float baseProductionTime;
    public float baseOutputValue;
}
