using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Boat Upgrade", fileName = "BoatUpgrade")]
public class BoatUpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    public int additionalInventorySlots;
    public float moneyCost;
    public List<MaterialCost> materialCosts;
    public string prerequisiteUpgradeId;
}
