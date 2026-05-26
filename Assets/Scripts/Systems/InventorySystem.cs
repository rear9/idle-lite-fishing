using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private GameState _state;

    public void Initialize(GameState state) => _state = state;

    public bool AddFish(FishInstance fish)
    {
        if (_state.inventory.fish.Count >= _state.upgrades.currentBoatCapacity)
        {
            Debug.LogWarning("[Inventory] Capacity reached.");
            return false;
        }

        fish.state = FishState.Inventory;
        _state.inventory.fish.Add(fish);
        EventBus.InventoryChanged();
        return true;
    }

    public bool RemoveFish(string id)
    {
        FishInstance f = _state.inventory.fish.FirstOrDefault(x => x.instanceId == id);
        if (f == null) return false;

        _state.inventory.fish.Remove(f);
        EventBus.InventoryChanged();
        return true;
    }

    public void AddMaterial(string id, int count)
    {
        if (count <= 0) return;

        MaterialSlot s = _state.inventory.materials.FirstOrDefault(m => m.materialId == id);
        if (s != null) s.count += count;
        else _state.inventory.materials.Add(new MaterialSlot { materialId = id, count = count });

        EventBus.InventoryChanged();
    }

    public bool RemoveMaterial(string id, int count)
    {
        if (count <= 0) return false;

        MaterialSlot s = _state.inventory.materials.FirstOrDefault(m => m.materialId == id);
        if (s == null || s.count < count) return false;

        s.count -= count;
        if (s.count <= 0) _state.inventory.materials.Remove(s);

        EventBus.InventoryChanged();
        return true;
    }

    public void AddOutput(FishOutputItem o)
    {
        FishOutputItem e = _state.inventory.outputs.FirstOrDefault(x => x.instanceId == o.instanceId);
        if (e != null) e.count += o.count;
        else _state.inventory.outputs.Add(o);

        EventBus.InventoryChanged();
    }

    public bool RemoveOutput(string id, int count)
    {
        FishOutputItem o = _state.inventory.outputs.FirstOrDefault(x => x.instanceId == id);
        if (o == null || o.count < count) return false;

        o.count -= count;
        if (o.count <= 0) _state.inventory.outputs.Remove(o);

        EventBus.InventoryChanged();
        return true;
    }

    public int GetFishCount() => _state.inventory.fish.Count;
    public bool IsCapacityAvailable() => _state.inventory.fish.Count < _state.upgrades.currentBoatCapacity;
}
