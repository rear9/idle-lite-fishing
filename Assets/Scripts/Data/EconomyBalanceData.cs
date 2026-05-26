using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Economy Balance", fileName = "EconomyBalance")]
public class EconomyBalanceData : ScriptableObject
{
    [Header("Starting Values")]
    public float startingMoney = 50f;
    public int startingNetCount = 1;
    public int startingNetTier = 1;
    public int startingHousingSlots = 1;
    public int startingBoatInventoryCapacity = 20;

    [Header("Care Costs")]
    public float basicFoodCost = 10f;
    public float basicFoodHungerRestore = 25f;
    public float cleanCost = 15f;
    public float cleanlinessRestore = 30f;

    [Header("Mechanics")]
    [Range(0f, 1f)] public float failedFishingJunkChance = 0.3f;
    public int netSlotCount = 6;

    [Header("Decay Rates")]
    public float hungerDecayPerSecond = 0.00116f;
    public float cleanlinessDecayPerSecond = 0.00093f;
}
