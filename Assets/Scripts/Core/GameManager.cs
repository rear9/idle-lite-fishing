using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EconomyBalanceData _economyBalance;
    [SerializeField] private SaveService _saveService;
    [SerializeField] private TimeService _timeService;
    [SerializeField] private InputHandler _inputHandler;

    [Header("Systems")]
    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private EconomySystem _economySystem;
    [SerializeField] private FishingSystem _fishingSystem;
    [SerializeField] private FishingMinigameView _fishingMinigameView;
    [SerializeField] private NetSystem _netSystem;
    [SerializeField] private NetView _netView;

    [Header("Data Registries")]
    [SerializeField] private List<FishSpeciesData> _allSpecies;
    [SerializeField] private List<MaterialData> _allMaterials;
    [SerializeField] private List<ZoneData> _allZones;
    [SerializeField] private List<NetTierData> _allNetTiers;
    [SerializeField] private List<NetRewardTableData> _allNetRewardTables;

    private GameState _state;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameState loaded = _saveService.Load();
        if (loaded != null)
        {
            _state = loaded;
            Debug.Log("[GameManager] State loaded from save.");

            if (string.IsNullOrEmpty(_state.currentZoneId))
            {
                _state.currentZoneId = "calm_coastal_waters";
                Debug.Log("[GameManager] Set default zone on loaded state.");
            }
        }
        else
        {
            _state = CreateDefaultState();
        }

        LogSummary();
        InitSystems();
        EventBus.MoneyChanged(_state.money);
        EventBus.InventoryChanged();
    }

    private void InitSystems()
    {
        _inventorySystem.Initialize(_state);
        _economySystem.Initialize(_state, _inventorySystem);
        _fishingSystem.Initialize(_state, _inventorySystem, _economyBalance);
        _fishingMinigameView.Initialize(_fishingSystem);
        _netSystem.Initialize(_state, _timeService, _economyBalance, _inventorySystem,
            _allNetTiers.ToArray(), _allNetRewardTables.ToArray());
    }

    private GameState CreateDefaultState()
    {
        GameState s = new GameState();
        s.money = _economyBalance.startingMoney;
        s.upgrades.currentRodPower = 1;
        s.upgrades.currentReelSpeed = 1f;
        s.upgrades.currentBarStability = 1f;
        s.upgrades.currentBoatCapacity = _economyBalance.startingBoatInventoryCapacity;
        s.upgrades.currentHousingSlots = _economyBalance.startingHousingSlots;
        s.currentZoneId = "calm_coastal_waters";
        s.lastSaveTime = DateTime.UtcNow.ToString("O");

        for (int i = 0; i < _economyBalance.startingNetCount; i++)
        {
            s.nets.Add(new NetState
            {
                netId = Guid.NewGuid().ToString(),
                tier = _economyBalance.startingNetTier,
                assignedSlotIndex = i
            });
        }

        return s;
    }

    public GameState GetState() => _state;
    public EconomyBalanceData GetEconomyBalance() => _economyBalance;
    public FishingSystem GetFishingSystem() => _fishingSystem;
    public FishingMinigameView GetFishingMinigameView() => _fishingMinigameView;
    public NetSystem GetNetSystem() => _netSystem;

    public FishSpeciesData GetSpeciesData(string id) => _allSpecies.FirstOrDefault(s => s.speciesId == id);
    public MaterialData GetMaterialData(string id) => _allMaterials.FirstOrDefault(m => m.materialId == id);
    public ZoneData GetZoneData(string id) => _allZones.FirstOrDefault(z => z.zoneId == id);
    public NetTierData GetNetTier(int tier) => _allNetTiers.FirstOrDefault(t => t.tier == tier);

    private void LogSummary()
    {
        Debug.Log($"[GameManager] money={_state.money} level={_state.playerLevel} " +
                  $"zone={_state.currentZoneId ?? "none"} nets={_state.nets.Count} " +
                  $"rodPower={_state.upgrades.currentRodPower} boat={_state.upgrades.currentBoatCapacity} " +
                  $"housing={_state.upgrades.currentHousingSlots} save={_state.saveVersion}");
    }

    public void ResetSave()
    {
        _saveService.DeleteSave();
        _state = CreateDefaultState();
        LogSummary();
        EventBus.MoneyChanged(_state.money);
        EventBus.InventoryChanged();
    }

    private void OnApplicationPause(bool pause) { if (pause) _saveService.Save(_state); }
    private void OnApplicationQuit() => _saveService.Save(_state);
}
