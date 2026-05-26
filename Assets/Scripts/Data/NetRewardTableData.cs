using System.Collections.Generic;
using UnityEngine;

public enum RewardType { Fish, Material }

[System.Serializable]
public class WeightedRewardEntry
{
    public string rewardId;
    public RewardType type;
    public float weight;
}

[CreateAssetMenu(menuName = "Fishing/Net Reward Table", fileName = "NetRewardTable")]
public class NetRewardTableData : ScriptableObject
{
    public NetTierData tier;
    public List<WeightedRewardEntry> balancedTable;
    public List<WeightedRewardEntry> fishFocusedTable;
    public List<WeightedRewardEntry> materialFocusedTable;
}
