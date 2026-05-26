using System;
using System.Collections.Generic;

[Serializable]
public class MaterialSlot
{
    public string materialId;
    public int count;
}

[Serializable]
public class FishOutputItem
{
    public string instanceId;
    public string speciesId;
    public FishOutputType outputType;
    public float baseOutputValue;
    public int quality;
    public int count;
}

[Serializable]
public class InventoryState
{
    public List<FishInstance> fish = new List<FishInstance>();
    public List<MaterialSlot> materials = new List<MaterialSlot>();
    public List<FishOutputItem> outputs = new List<FishOutputItem>();
}
