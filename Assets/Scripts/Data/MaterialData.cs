using UnityEngine;

[System.Serializable]
public class MaterialCost
{
    public string materialId;
    public int count;
}

[CreateAssetMenu(menuName = "Fishing/Material", fileName = "Material")]
public class MaterialData : ScriptableObject
{
    public string materialId;
    public string displayName;
    public float saleValue;
    public int tier;
}
