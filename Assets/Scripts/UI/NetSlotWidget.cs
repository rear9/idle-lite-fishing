using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetSlotWidget : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _contentsLabel;
    [SerializeField] private TextMeshProUGUI _fillLabel;
    [SerializeField] private TextMeshProUGUI _filterLabel;

    [Header("Progress")]
    [SerializeField] private Image _fillBar;

    [Header("Filter Buttons")]
    [SerializeField] private Button _balancedBtn;
    [SerializeField] private Button _fishBtn;
    [SerializeField] private Button _materialBtn;

    private NetState _net;

    public void Setup(NetState net)
    {
        _net = net;

        if (_balancedBtn) _balancedBtn.onClick.AddListener(() => SetFilter(NetFilter.Balanced));
        if (_fishBtn) _fishBtn.onClick.AddListener(() => SetFilter(NetFilter.FishFocused));
        if (_materialBtn) _materialBtn.onClick.AddListener(() => SetFilter(NetFilter.MaterialFocused));

        Refresh(net);
    }

    private void SetFilter(NetFilter filter)
    {
        NetSystem sys = GameManager.Instance?.GetNetSystem();
        if (sys != null) sys.SetFilter(_net.netId, filter);
    }

    public void Refresh(NetState net)
    {
        _net = net;
        NetTierData tier = GameManager.Instance?.GetNetSystem()?.GetTier(net.tier);
        string tierName = tier != null ? tier.displayName : $"Tier {net.tier}";

        if (_nameLabel) _nameLabel.text = tierName;

        if (_filterLabel)
        {
            _filterLabel.text = net.filter switch
            {
                NetFilter.FishFocused => "Fish",
                NetFilter.MaterialFocused => "Mat",
                _ => "Bal"
            };
        }

        if (_contentsLabel)
        {
            int fishC = net.fishContents.Count;
            int matC = net.materialContents.Count;
            int cap = tier != null ? tier.capacity : 0;
            _contentsLabel.text = $"{fishC + matC}/{cap}";
        }

        if (_fillLabel) _fillLabel.text = $"{net.fillProgress * 100f:F0}%";

        if (_fillBar) _fillBar.fillAmount = net.fillProgress;
    }
}
