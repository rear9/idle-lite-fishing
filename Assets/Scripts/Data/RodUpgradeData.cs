using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Rod Upgrade", fileName = "RodUpgrade")]
public class RodUpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    public int rodPowerIncrease;
    public float reelSpeedBonus;
    public float barStabilityBonus;
    public float moneyCost;
    public List<MaterialCost> materialCosts;
    public string prerequisiteUpgradeId;
}
