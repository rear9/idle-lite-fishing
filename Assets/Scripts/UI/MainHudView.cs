using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainHudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyLabel;
    [SerializeField] private TextMeshProUGUI _capacityLabel;
    [SerializeField] private Button _fishButton;
    [SerializeField] private Button _sellAllButton;

    private void OnEnable()
    {
        EventBus.OnMoneyChanged += UpdateMoney;
        EventBus.OnInventoryChanged += UpdateCapacity;
    }

    private void OnDisable()
    {
        EventBus.OnMoneyChanged -= UpdateMoney;
        EventBus.OnInventoryChanged -= UpdateCapacity;
    }

    private void Awake()
    {
        if (_fishButton) _fishButton.onClick.AddListener(OnFish);
        if (_sellAllButton) _sellAllButton.onClick.AddListener(OnSellAll);
    }

    private void Start()
    {
        GameState s = GameManager.Instance.GetState();
        if (s != null) { UpdateMoney(s.money); UpdateCapacity(); }
    }

    private void OnFish()
    {
        FishingSystem f = GameManager.Instance?.GetFishingSystem();
        FishingMinigameView m = GameManager.Instance?.GetFishingMinigameView();
        if (f != null && m != null)
        {
            FishEncounter? e = f.StartFishing();
            if (e.HasValue) m.BeginFishing(e.Value);
        }
    }

    private void OnSellAll()
    {
        EconomySystem e = GameManager.Instance?.GetEconomySystem();
        if (e != null) e.SellAllFish();
    }

    private void UpdateMoney(float v) { if (_moneyLabel) _moneyLabel.text = $"${v:F0}"; }

    private void UpdateCapacity()
    {
        if (!_capacityLabel) return;
        GameState s = GameManager.Instance.GetState();
        _capacityLabel.text = $"{(s?.inventory?.fish?.Count ?? 0)}/{(s?.upgrades?.currentBoatCapacity ?? 0)}";
    }
}
