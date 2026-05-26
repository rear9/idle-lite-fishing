using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public float money;
    public int playerLevel;
    public int playerXp;
    public InventoryState inventory = new InventoryState();
    public List<NetState> nets = new List<NetState>();
    public HousingState housing = new HousingState();
    public UpgradeState upgrades = new UpgradeState();
    public string currentZoneId;
    public string lastSaveTime;
    public string saveVersion = "1.0.0";
}
