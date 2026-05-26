using TMPro;
using UnityEngine;

public class NetSlotWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _amountLabel;

    private NetState _net;

    public void Setup(NetState net)
    {
        _net = net;
        Refresh(net);
    }

    public void Refresh(NetState net)
    {
        NetTierData tier = GameManager.Instance?.GetNetSystem()?.GetTier(net.tier);
        string tierName = tier != null ? tier.displayName : $"Tier {net.tier}";
        if (_nameLabel) _nameLabel.text = tierName;

        int total = net.fishContents.Count + net.materialContents.Count;
        if (_amountLabel) _amountLabel.text = total > 0 ? $"{total}x" : "";
    }
}
