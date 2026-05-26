using System.Collections.Generic;
using UnityEngine;

public enum RecipeOutputType { RodUpgrade, NetUpgrade, BoatUpgrade, HousingUpgrade }

[CreateAssetMenu(menuName = "Fishing/Recipe", fileName = "Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeId;
    public string displayName;
    public float moneyCost;
    public List<MaterialCost> materialCosts;
    public RecipeOutputType outputType;
    public string outputId;
    public int outputAmount;
    public string unlockCondition;
}
