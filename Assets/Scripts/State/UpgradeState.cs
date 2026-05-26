using System;
using System.Collections.Generic;

[Serializable]
public class UpgradeState
{
    public List<string> purchasedUpgradeIds = new List<string>();
    public int currentRodPower = 1;
    public float currentReelSpeed = 1f;
    public float currentBarStability = 1f;
    public int currentBoatCapacity = 20;
    public int currentHousingSlots = 1;
    public float currentHungerDecayMultiplier = 1f;
    public float currentCleanlinessDecayMultiplier = 1f;
    public float currentProductionSpeedMultiplier = 1f;
}
