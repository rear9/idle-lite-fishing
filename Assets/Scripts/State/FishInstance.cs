using System;

[Serializable]
public class FishInstance
{
    public string instanceId;
    public string speciesId;
    public FishRarity rarity;
    public float weight;
    public float length;
    public int randomSeed;
    public int quality;
    public FishState state;
}
