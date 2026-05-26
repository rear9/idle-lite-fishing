using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetView : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private RectTransform _gridContent;
    [SerializeField] private NetSlotWidget _slotPrefab;

    [Header("Filter Area")]
    [SerializeField] private RectTransform _filterArea;
    [SerializeField] private NetFilterRowWidget _filterRowPrefab;

    [Header("Overall Fill")]
    [SerializeField] private Image _overallFillBar;
    [SerializeField] private TextMeshProUGUI _overallFillLabel;

    [Header("Actions")]
    [SerializeField] private Button _collectAllButton;

    private List<NetSlotWidget> _slots = new List<NetSlotWidget>();
    private List<NetFilterRowWidget> _filterRows = new List<NetFilterRowWidget>();

    private void OnEnable()
    {
        EventBus.OnNetStateChanged += OnNetChanged;
        if (_collectAllButton) _collectAllButton.onClick.AddListener(CollectAll);
    }

    private void OnDisable()
    {
        EventBus.OnNetStateChanged -= OnNetChanged;
        if (_collectAllButton) _collectAllButton.onClick.RemoveListener(CollectAll);
    }

    private void Start() { Rebuild(); }

    private void CollectAll()
    {
        GameManager.Instance?.GetNetSystem()?.CollectAllNets();
    }

    private void OnNetChanged(string netId)
    {
        var nets = GameManager.Instance?.GetState()?.nets;
        if (nets == null) return;

        bool matched = false;
        for (int i = 0; i < nets.Count; i++)
        {
            if (nets[i].netId == netId)
            {
                if (i < _slots.Count) _slots[i].Refresh(nets[i]);
                if (i < _filterRows.Count) _filterRows[i].Refresh(nets[i]);
                matched = true;
                break;
            }
        }

        if (matched) RefreshOverall();
    }

    public void Rebuild()
    {
        GameState state = GameManager.Instance?.GetState();
        if (state == null) return;

        RebuildFilterRows(state);
        RebuildGrid(state);
        RefreshOverall();
    }

    private void RebuildFilterRows(GameState state)
    {
        if (_filterArea == null || _filterRowPrefab == null) return;

        foreach (var r in _filterRows) { if (r != null) Destroy(r.gameObject); }
        _filterRows.Clear();

        for (int i = 0; i < state.nets.Count; i++)
        {
            NetFilterRowWidget row = Instantiate(_filterRowPrefab, _filterArea);
            row.Setup(state.nets[i].netId, i, state.nets[i]);
            _filterRows.Add(row);
        }
    }

    private void RebuildGrid(GameState state)
    {
        if (_gridContent == null || _slotPrefab == null) return;

        foreach (var s in _slots) { if (s != null) Destroy(s.gameObject); }
        _slots.Clear();

        foreach (NetState net in state.nets)
        {
            NetSlotWidget slot = Instantiate(_slotPrefab, _gridContent);
            slot.Setup(net);
            _slots.Add(slot);
        }
    }

    private void RefreshOverall()
    {
        GameState state = GameManager.Instance?.GetState();
        if (state == null) return;

        float totalProgress = 0f;
        float totalWeight = 0f;

        foreach (NetState net in state.nets)
        {
            NetTierData tier = GameManager.Instance?.GetNetSystem()?.GetTier(net.tier);
            int cap = tier != null ? tier.capacity : 1;
            totalProgress += net.fillProgress * cap;
            totalWeight += cap;
        }

        float overall = totalWeight > 0f ? totalProgress / totalWeight : 0f;
        overall = Mathf.Clamp01(overall);

        if (_overallFillBar) _overallFillBar.fillAmount = overall;
        if (_overallFillLabel) _overallFillLabel.text = $"Overall: {overall * 100f:F0}%";
    }

    [ContextMenu("Force Rebuild")]
    private void DebugRebuild() => Rebuild();
}
