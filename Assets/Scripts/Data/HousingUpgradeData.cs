using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Housing Upgrade", fileName = "HousingUpgrade")]
public class HousingUpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    public int additionalSlots;
    public float hungerDecayMultiplier;
    public float cleanlinessDecayMultiplier;
    public float productionSpeedMultiplier;
    public float moneyCost;
    public List<MaterialCost> materialCosts;
    public string prerequisiteUpgradeId;
}
