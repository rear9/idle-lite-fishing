using System;
using System.Collections.Generic;

public enum NetFilter { Balanced, FishFocused, MaterialFocused }

[Serializable]
public class NetState
{
    public string netId;
    public int tier;
    public int assignedSlotIndex = -1;
    public NetFilter filter;
    public List<FishInstance> fishContents = new List<FishInstance>();
    public List<MaterialSlot> materialContents = new List<MaterialSlot>();
    public float fillProgress;
    public bool isActive;
}
