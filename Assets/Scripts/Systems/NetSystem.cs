using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetSystem : MonoBehaviour
{
    [SerializeField] private float _baseFillTime = 60f;

    private GameState _state;
    private TimeService _time;
    private EconomyBalanceData _economy;
    private InventorySystem _inventory;
    private NetTierData[] _tiers;
    private NetRewardTableData[] _rewardTables;
    private bool changed = false;

    public void Initialize(GameState state, TimeService time, EconomyBalanceData economy,
        InventorySystem inventory, NetTierData[] allTiers, NetRewardTableData[] allTables)
    {
        _state = state;
        _time = time;
        _economy = economy;
        _inventory = inventory;
        _tiers = allTiers;
        _rewardTables = allTables;
    }

    private void OnEnable()
    {
        if (_time != null)
            _time.OnScaledTick += Tick;
    }

    private void OnDisable()
    {
        if (_time != null)
            _time.OnScaledTick -= Tick;
    }

    private void Update()
    {
        if (_time == null || _time.DebugTimeMultiplier <= 1f)
            Tick(Time.deltaTime);
    }

    private void Tick(float dt)
    {
        if (_state == null) return;
        foreach (NetState net in _state.nets)
        {
            if (!net.isActive) net.isActive = true;
            if (net.assignedSlotIndex < 0) continue;

            NetTierData tier = GetTier(net.tier);
            if (tier == null) continue;

            int currentCount = net.fishContents.Count + net.materialContents.Count;
            if (tier.capacity > 0 && currentCount >= tier.capacity) continue;

            float rate = tier.fillRateMultiplier / _baseFillTime;
            net.fillProgress += rate * dt;

            while (net.fillProgress >= 1f)
            {
                net.fillProgress -= 1f;
                int c = net.fishContents.Count + net.materialContents.Count;
                if (tier.capacity > 0 && c >= tier.capacity) break;
                changed = true;
                GenerateReward(net);
            }
            if (changed) EventBus.NetStateChanged(net.netId);
        }
        changed = false;
    }

    private void GenerateReward(NetState net)
    {
        NetRewardTableData table = GetTable(net.tier);
        if (table == null) return;

        var pool = net.filter switch
        {
            NetFilter.FishFocused => table.fishFocusedTable,
            NetFilter.MaterialFocused => table.materialFocusedTable,
            _ => table.balancedTable
        };

        if (pool == null || pool.Count == 0) return;

        WeightedRewardEntry entry = WeightedPick(pool);
        if (entry == null) return;

        if (entry.type == RewardType.Fish)
        {
            FishSpeciesData sp = GameManager.Instance.GetSpeciesData(entry.rewardId);
            if (sp == null) return;

            float weight = Random.Range(sp.minWeight, sp.maxWeight);
            float length = Random.Range(sp.minLength, sp.maxLength);
            int seed = Random.Range(0, 10000);

            FishInstance fish = new FishInstance
            {
                instanceId = Guid.NewGuid().ToString(),
                speciesId = sp.speciesId,
                rarity = sp.rarity,
                weight = weight,
                length = length,
                randomSeed = seed,
                quality = Mathf.Max(1, FishQualityCalculator.CalculateQuality(sp, weight, length, seed) - 2),
                state = FishState.Inventory
            };

            net.fishContents.Add(fish);
        }
        else if (entry.type == RewardType.Material)
        {
            MaterialSlot existing = net.materialContents.FirstOrDefault(m => m.materialId == entry.rewardId);
            if (existing != null) existing.count++;
            else net.materialContents.Add(new MaterialSlot { materialId = entry.rewardId, count = 1 });
        }
    }

    public void SetFilter(string netId, NetFilter filter)
    {
        NetState net = _state.nets.FirstOrDefault(n => n.netId == netId);
        if (net == null) return;
        net.filter = filter;
        EventBus.NetStateChanged(netId);
    }

    public bool CollectNet(string netId)
    {
        NetState net = _state.nets.FirstOrDefault(n => n.netId == netId);
        if (net == null) return false;

        bool addedAny = false;

        foreach (FishInstance fish in net.fishContents.ToList())
        {
            if (_inventory.AddFish(fish))
            {
                net.fishContents.Remove(fish);
                addedAny = true;
            }
            else break;
        }

        foreach (MaterialSlot slot in net.materialContents.ToList())
        {
            _inventory.AddMaterial(slot.materialId, slot.count);
            net.materialContents.Remove(slot);
            addedAny = true;
        }

        if (addedAny) EventBus.NetStateChanged(netId);
        return addedAny;
    }

    public bool DeployNet(string netId, int slotIndex)
    {
        NetState net = _state.nets.FirstOrDefault(n => n.netId == netId);
        if (net == null) return false;

        NetState existing = _state.nets.FirstOrDefault(n => n.assignedSlotIndex == slotIndex && n.netId != netId);
        if (existing != null) return false;

        net.assignedSlotIndex = slotIndex;
        EventBus.NetStateChanged(netId);
        return true;
    }

    public void RecallNet(string netId)
    {
        NetState net = _state.nets.FirstOrDefault(n => n.netId == netId);
        if (net == null) return;
        net.assignedSlotIndex = -1;
        EventBus.NetStateChanged(netId);
    }

    public NetTierData GetTier(int tier)
    {
        return _tiers.FirstOrDefault(t => t.tier == tier);
    }

    private NetRewardTableData GetTable(int tier)
    {
        return _rewardTables.FirstOrDefault(t => t.tier != null && t.tier.tier == tier);
    }

    private WeightedRewardEntry WeightedPick(System.Collections.Generic.List<WeightedRewardEntry> entries)
    {
        float total = entries.Sum(e => e.weight);
        if (total <= 0f) return null;

        float roll = Random.value * total;
        float acc = 0f;
        foreach (var e in entries)
        {
            acc += e.weight;
            if (roll <= acc) return e;
        }
        return entries.Last();
    }

    public void CollectAllNets()
    {
        if (_state == null) return;
        bool any = false;
        foreach (NetState net in _state.nets)
            any |= CollectNet(net.netId);
    }

    [ContextMenu("Collect All Nets")]
    private void DebugCollectAll() => CollectAllNets();

    [ContextMenu("Print Net Status")]
    private void DebugPrintNets()
    {
        if (_state == null) return;
        foreach (NetState net in _state.nets)
        {
            NetTierData t = GetTier(net.tier);
            Debug.Log($"[Net] {net.netId} tier={net.tier} slot={net.assignedSlotIndex} " +
                      $"filter={net.filter} progress={net.fillProgress:F2} " +
                      $"fish={net.fishContents.Count} mat={net.materialContents.Count} " +
                      $"cap={t?.capacity ?? 0}");
        }
    }
}
