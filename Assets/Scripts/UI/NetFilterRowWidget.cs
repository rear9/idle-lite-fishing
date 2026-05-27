using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetFilterRowWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private Button _balancedBtn;
    [SerializeField] private Button _fishBtn;
    [SerializeField] private Button _materialBtn;

    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _inactiveColor = Color.gray;

    private string _netId;

    private void Awake()
    {
        _balancedBtn.onClick.AddListener(() => Set(NetFilter.Balanced));
        _fishBtn.onClick.AddListener(() => Set(NetFilter.FishFocused));
        _materialBtn.onClick.AddListener(() => Set(NetFilter.MaterialFocused));
    }

    public void Setup(string netId, int index, NetState net)
    {
        _netId = netId;
        if (_nameLabel) _nameLabel.text = $"Net {index + 1}";
        Refresh(net);
    }

    private void Set(NetFilter filter)
    {
        GameManager.Instance?.GetNetSystem()?.SetFilter(_netId, filter);
    }

    public void Refresh(NetState net)
    {
        SetButtonVisual(_balancedBtn, net.filter == NetFilter.Balanced);
        SetButtonVisual(_fishBtn, net.filter == NetFilter.FishFocused);
        SetButtonVisual(_materialBtn, net.filter == NetFilter.MaterialFocused);
    }

    private void SetButtonVisual(Button btn, bool active)
    {
        if (btn == null) return;
        ColorBlock cb = btn.colors;
        cb.normalColor = active ? _activeColor : _inactiveColor;
        cb.selectedColor = active ? _activeColor : _inactiveColor;
        btn.colors = cb;
    }
}
