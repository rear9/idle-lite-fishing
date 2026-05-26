using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Net Tier", fileName = "NetTier")]
public class NetTierData : ScriptableObject
{
    public int tier;
    public string displayName;
    public int capacity;
    public float fillRateMultiplier;
    public int requiredRodPower;
    public float upgradeCost;
}
