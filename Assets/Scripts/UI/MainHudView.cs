using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainHudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyLabel;
    [SerializeField] private TextMeshProUGUI _capacityLabel;
    [SerializeField] private Button _fishButton;

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

    private void Start()
    {
        GameState s = GameManager.Instance.GetState();
        if (s != null) { UpdateMoney(s.money); UpdateCapacity(); }

        if (_fishButton) _fishButton.onClick.AddListener(OnFish);
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

    private void UpdateMoney(float v) { if (_moneyLabel) _moneyLabel.text = $"${v:F0}"; }

    private void UpdateCapacity()
    {
        if (!_capacityLabel) return;
        GameState s = GameManager.Instance.GetState();
        _capacityLabel.text = $"{(s?.inventory?.fish?.Count ?? 0)}/{(s?.upgrades?.currentBoatCapacity ?? 0)}";
    }
}
