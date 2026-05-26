using System;

[Serializable]
public class HousedFishState
{
    public string instanceId;
    public float hunger = 100f;
    public float cleanliness = 100f;
    public float productionProgress;
    public bool producedOffline;
    public string lastCareUpdate;
    public string lastProductionTick;

    public float happiness => (hunger + cleanliness) / 2f;
}
