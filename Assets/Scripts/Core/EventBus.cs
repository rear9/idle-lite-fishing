using System;

public static class EventBus
{
    public static event Action<float> OnMoneyChanged;
    public static event Action OnInventoryChanged;
    public static event Action<string> OnNetStateChanged;
    public static event Action OnHousingStateChanged;
    public static event Action<string> OnUpgradePurchased;
    public static event Action<FishInstance> OnFishCaught;
    public static event Action<string> OnFishHoused;
    public static event Action<OfflineProgressResult> OnOfflineProgressApplied;

    public static void MoneyChanged(float v) => OnMoneyChanged?.Invoke(v);
    public static void InventoryChanged() => OnInventoryChanged?.Invoke();
    public static void NetStateChanged(string id) => OnNetStateChanged?.Invoke(id);
    public static void HousingStateChanged() => OnHousingStateChanged?.Invoke();
    public static void UpgradePurchased(string id) => OnUpgradePurchased?.Invoke(id);
    public static void FishCaught(FishInstance f) => OnFishCaught?.Invoke(f);
    public static void FishHoused(string id) => OnFishHoused?.Invoke(id);
    public static void OfflineProgressApplied(OfflineProgressResult r) => OnOfflineProgressApplied?.Invoke(r);

    public static void Clear()
    {
        OnMoneyChanged = null;
        OnInventoryChanged = null;
        OnNetStateChanged = null;
        OnHousingStateChanged = null;
        OnUpgradePurchased = null;
        OnFishCaught = null;
        OnFishHoused = null;
        OnOfflineProgressApplied = null;
    }
}
