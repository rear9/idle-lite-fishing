using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneFishEntry
{
    public FishSpeciesData species;
    public float spawnWeight;
}

[CreateAssetMenu(menuName = "Fishing/Zone", fileName = "Zone")]
public class ZoneData : ScriptableObject
{
    public string zoneId;
    public string displayName;
    public List<ZoneFishEntry> availableFish;
    public NetRewardTableData netRewardTable;
    public string unlockCondition;
}
