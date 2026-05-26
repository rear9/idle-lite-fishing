using System.Linq;
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    private GameState _state;
    private InventorySystem _inventory;

    public void Initialize(GameState state, InventorySystem inventory)
    {
        _state = state;
        _inventory = inventory;
    }

    public void AddMoney(float v)
    {
        if (v <= 0f) return;
        _state.money += v;
        EventBus.MoneyChanged(_state.money);
    }

    public bool DeductMoney(float v)
    {
        if (v <= 0f) return true;
        if (_state.money < v) return false;
        _state.money -= v;
        EventBus.MoneyChanged(_state.money);
        return true;
    }

    public bool SellFish(string id)
    {
        FishInstance f = _state.inventory.fish.FirstOrDefault(x => x.instanceId == id);
        if (f == null || f.state != FishState.Inventory) return false;

        FishSpeciesData sp = GameManager.Instance.GetSpeciesData(f.speciesId);
        if (sp == null) return false;

        float value = GetFishSaleValue(f, sp);
        _state.money += value;
        f.state = FishState.Sold;
        _state.inventory.fish.Remove(f);

        EventBus.InventoryChanged();
        EventBus.MoneyChanged(_state.money);
        return true;
    }

    public bool SellMaterial(string id, int count)
    {
        MaterialSlot s = _state.inventory.materials.FirstOrDefault(m => m.materialId == id);
        if (s == null || s.count < count) return false;

        MaterialData m = GameManager.Instance.GetMaterialData(id);
        if (m == null) return false;

        _state.money += m.saleValue * count;
        s.count -= count;
        if (s.count <= 0) _state.inventory.materials.Remove(s);

        EventBus.InventoryChanged();
        EventBus.MoneyChanged(_state.money);
        return true;
    }

    public bool SellAllOfMaterial(string id)
    {
        MaterialSlot s = _state.inventory.materials.FirstOrDefault(m => m.materialId == id);
        return s != null && SellMaterial(id, s.count);
    }

    public void SellAllFish()
    {
        var toSell = _state.inventory.fish.Where(f => f.state == FishState.Inventory).ToList();
        if (toSell.Count == 0) return;
        float total = 0f;
        foreach (FishInstance f in toSell)
        {
            FishSpeciesData sp = GameManager.Instance.GetSpeciesData(f.speciesId);
            if (sp == null) continue;
            total += GetFishSaleValue(f, sp);
            _state.inventory.fish.Remove(f);
        }
        _state.money += total;
        EventBus.InventoryChanged();
        EventBus.MoneyChanged(_state.money);
    }

    public bool SellOutput(FishOutputItem o)
    {
        if (!_state.inventory.outputs.Contains(o)) return false;

        float val = GetOutputSaleValue(o);
        _state.money += val * o.count;
        _state.inventory.outputs.Remove(o);

        EventBus.InventoryChanged();
        EventBus.MoneyChanged(_state.money);
        return true;
    }

    public float GetFishSaleValue(FishInstance f, FishSpeciesData sp)
    {
        float r = RarityMult(f.rarity);
        float q = 0.5f + (f.quality - 1) / 9f * 1.5f;
        return sp.baseSaleValue * r * q;
    }

    public float GetHousingOutputEstimate(FishInstance f)
    {
        FishSpeciesData sp = GameManager.Instance.GetSpeciesData(f.speciesId);
        if (sp == null) return 0f;
        return sp.baseOutputValue * (f.quality / 5f);
    }

    private float RarityMult(FishRarity r) => r switch
    {
        FishRarity.Common => 1f,
        FishRarity.Uncommon => 1.5f,
        FishRarity.Rare => 3f,
        FishRarity.Epic => 6f,
        FishRarity.Legendary => 15f,
        _ => 1f
    };

    private float GetOutputSaleValue(FishOutputItem o)
    {
        float q = 0.5f + (o.quality - 1) / 9f * 1.5f;
        return o.baseOutputValue * q;
    }

    // Debug
    [ContextMenu("Sell First Fish")]
    private void DebugSellFirst()
    {
        FishInstance f = _state?.inventory?.fish?.FirstOrDefault();
        if (f != null) SellFish(f.instanceId);
        else Debug.Log("[Economy] No fish to sell.");
    }

    [ContextMenu("Add +100 Money")]
    private void DebugAddMoney() => AddMoney(100f);

    [ContextMenu("Print Money")]
    private void DebugPrintMoney() => Debug.Log($"[Economy] ${_state?.money ?? 0}");
}
