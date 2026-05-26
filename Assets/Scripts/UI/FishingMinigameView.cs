using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigameView : MonoBehaviour
{
    [Header("Bar Elements")]
    [SerializeField] private RectTransform _barArea;
    [SerializeField] private RectTransform _fishZone;
    [SerializeField] private RectTransform _playerMarker;

    [Header("Progress")]
    [SerializeField] private Image _catchProgressBar;
    [SerializeField] private Image _escapeProgressBar;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _fishInfoLabel;

    [Header("Buttons")]
    [SerializeField] private Button _cancelButton;

    [Header("Settings")]
    [SerializeField] private float _baseCatchRate = 0.3f;
    [SerializeField] private float _baseEscapeRate = 0.2f;

    private FishEncounter _currentEncounter;
    private InputHandler _input;
    private FishingSystem _fishingSystem;

    private float _markerPos;
    private float _zonePos;
    private float _zoneSpeed;
    private float _zoneHeight;
    private float _gravity;
    private float _reelSpeed;
    private float _catchProgress;
    private float _escapeProgress;
    private float _overlapTime;
    private float _totalTime;
    private int _zoneDir = 1;
    private float _barHeight;
    private bool _active;
    private const float MARKER_HEIGHT = 0.05f;

    private void Awake()
    {
        _cancelButton.onClick.AddListener(Cancel);
        _barHeight = _barArea?.rect.height ?? 600f;
        _input = FindFirstObjectByType<InputHandler>();
        gameObject.SetActive(false);
    }

    public void Initialize(FishingSystem system) => _fishingSystem = system;

    public void BeginFishing(FishEncounter encounter)
    {
        _currentEncounter = encounter;
        gameObject.SetActive(true);
        _active = true;

        _markerPos = 0.5f;
        _zonePos = 0.5f;
        _catchProgress = 0f;
        _escapeProgress = 0f;
        _overlapTime = 0f;
        _totalTime = 0f;
        _zoneDir = 1;

        float d = encounter.difficulty;
        _zoneSpeed = 0.4f + d * 2.5f;
        _zoneHeight = Mathf.Max(0.08f, 0.5f - d * 0.35f);
        _gravity = 0.3f + d * 1.8f;
        _reelSpeed = GameManager.Instance.GetState().upgrades.currentReelSpeed;

        _fishInfoLabel.text = encounter.isReachable ? "???" : "Too strong!";
    }

    private void Update()
    {
        if (!_active) return;

        MoveZone();
        MoveMarker();
        CheckOverlap();
        UpdateVisuals();

        if (_catchProgress >= 1f) End(true);
        else if (_escapeProgress >= 1f) End(false);
    }

    private void MoveZone()
    {
        float noise = Mathf.PerlinNoise(Time.time * 0.7f, 0f) * 2f - 1f;
        _zonePos += _zoneDir * _zoneSpeed * (1f + noise * 0.3f) * Time.deltaTime * 0.5f;

        if (_zonePos > 0.9f) { _zonePos = 0.9f; _zoneDir = -1; }
        if (_zonePos < 0.1f) { _zonePos = 0.1f; _zoneDir = 1; }
    }

    private void MoveMarker()
    {
        if (_input != null && _input.IsHolding)
            _markerPos += _reelSpeed * Time.deltaTime;
        else
            _markerPos -= _gravity * Time.deltaTime;

        _markerPos = Mathf.Clamp01(_markerPos);
    }

    private void CheckOverlap()
    {
        float zTop = _zonePos + _zoneHeight / 2f;
        float zBot = _zonePos - _zoneHeight / 2f;
        float mTop = _markerPos + MARKER_HEIGHT / 2f;
        float mBot = _markerPos - MARKER_HEIGHT / 2f;
        bool overlap = mBot < zTop && mTop > zBot;

        float dt = Time.deltaTime;
        _totalTime += dt;

        if (overlap)
        {
            _overlapTime += dt;
            _catchProgress += _baseCatchRate * dt;
            _escapeProgress = Mathf.Max(0f, _escapeProgress - _baseEscapeRate * 2f * dt);
        }
        else
        {
            _catchProgress = Mathf.Max(0f, _catchProgress - _baseEscapeRate * dt);
            _escapeProgress += _baseEscapeRate * dt;
        }

        _catchProgress = Mathf.Clamp01(_catchProgress);
        _escapeProgress = Mathf.Clamp01(_escapeProgress);
    }

    private void UpdateVisuals()
    {
        float zY = Mathf.Lerp(-_barHeight / 2f + _zoneHeight * _barHeight / 2f, _barHeight / 2f - _zoneHeight * _barHeight / 2f, _zonePos);
        _fishZone.anchoredPosition = new Vector2(_fishZone.anchoredPosition.x, zY);
        _fishZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _zoneHeight * _barHeight);

        float mY = Mathf.Lerp(-_barHeight / 2f, _barHeight / 2f, _markerPos);
        _playerMarker.anchoredPosition = new Vector2(_playerMarker.anchoredPosition.x, mY);

        if (_catchProgressBar) _catchProgressBar.fillAmount = _catchProgress;
        if (_escapeProgressBar) _escapeProgressBar.fillAmount = _escapeProgress;
    }

    private void End(bool caught)
    {
        _active = false;
        float perf = _totalTime > 0f ? _overlapTime / _totalTime : 0f;
        _fishingSystem?.SubmitCatchResult(_currentEncounter, caught, perf);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        if (!_active) return;
        _active = false;
        gameObject.SetActive(false);
    }
}
