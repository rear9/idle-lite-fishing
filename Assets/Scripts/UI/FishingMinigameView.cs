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

    private FishEncounter _currentEncounter;
    private InputHandler _input;
    private FishingSystem _fishingSystem;

    private float _barHeight;
    private bool _active;

    private void Awake()
    {
        if (_cancelButton) _cancelButton.onClick.AddListener(Cancel);
        _barHeight = _barArea != null ? _barArea.rect.height : 600f;
        _input = FindFirstObjectByType<InputHandler>();
    }

    public void Initialize(FishingSystem system) => _fishingSystem = system;

    private readonly FishingMinigameState _minigame = new FishingMinigameState();

    public void BeginFishing(FishEncounter encounter)
    {
        _currentEncounter = encounter;
        _minigame.Setup(encounter, GameManager.Instance.GetState().upgrades.currentReelSpeed);
        _active = true;
        gameObject.SetActive(true);
        if (_fishInfoLabel) _fishInfoLabel.text = encounter.species.displayName;
    }

    private void Update()
    {
        if (!_active) return;
        _minigame.Tick(Time.deltaTime, _input != null && _input.IsHolding);
        UpdateVisuals();
        if (_minigame.IsComplete) End(_minigame.WasCaught);
    }

    private void End(bool caught)
    {
        _active = false;
        _fishingSystem?.SubmitCatchResult(_currentEncounter, caught, _minigame.PerfScore);
        gameObject.SetActive(false);
    }

    private void UpdateVisuals()
    {
        float zoneHeight = _minigame.GetZoneHeight();

        if (_fishZone != null)
        {
            float zY = Mathf.Lerp(-_barHeight / 2f + zoneHeight * _barHeight / 2f, _barHeight / 2f - zoneHeight * _barHeight / 2f, _minigame.ZonePos);
            _fishZone.anchoredPosition = new Vector2(_fishZone.anchoredPosition.x, zY);
            _fishZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, zoneHeight * _barHeight);
        }

        if (_playerMarker != null)
        {
            float mY = Mathf.Lerp(-_barHeight / 2f, _barHeight / 2f, _minigame.MarkerPos);
            _playerMarker.anchoredPosition = new Vector2(_playerMarker.anchoredPosition.x, mY);
        }

        if (_catchProgressBar) _catchProgressBar.fillAmount = _minigame.CatchProgress;
        if (_escapeProgressBar) _escapeProgressBar.fillAmount = _minigame.EscapeProgress;
    }

    public void Cancel()
    {
        if (!_active) return;
        _active = false;
        gameObject.SetActive(false);
    }
}
