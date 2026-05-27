using UnityEngine;

public class FishingMinigameState
{
    public float MarkerPos { get; private set; } = 0.5f;
    public float ZonePos   { get; private set; } = 0.5f;
    public float CatchProgress  { get; private set; }
    public float EscapeProgress { get; private set; }
    public bool  IsComplete => CatchProgress >= 1f || EscapeProgress >= 1f;
    public bool  WasCaught  => CatchProgress >= 1f;
    public float PerfScore  => _totalTime > 0f ? _overlapTime / _totalTime : 0f;

    private float _zoneSpeed, _zoneHeight, _gravity, _reelSpeed;
    private float _overlapTime, _totalTime;
    private int   _zoneDir = 1;
    private const float CATCH_RATE = 0.3f;
    private const float ESCAPE_RATE = 0.2f;
    private const float MARKER_HEIGHT = 0.05f;

    public void Setup(FishEncounter enc, float reelSpeed)
    {
        float d = enc.difficulty;
        _zoneSpeed  = 0.4f + d * 2.5f;
        _zoneHeight = Mathf.Max(0.08f, 0.5f - d * 0.35f);
        _gravity    = 0.3f + d * 1.8f;
        _reelSpeed  = reelSpeed;
        MarkerPos = ZonePos = 0.5f;
        CatchProgress = EscapeProgress = _overlapTime = _totalTime = 0f;
        _zoneDir = 1;
    }

    public void Tick(float dt, bool holding)
    {
        MoveZone(dt);
        MoveMarker(dt, holding);
        CheckOverlap(dt);
    }

    private void MoveZone(float dt)
    {
        float noise = Mathf.PerlinNoise(Time.time * 0.7f, 0f) * 2f - 1f;
        ZonePos += _zoneDir * _zoneSpeed * (1f + noise * 0.3f) * dt * 0.5f;
        if (ZonePos > 0.9f) { ZonePos = 0.9f; _zoneDir = -1; }
        if (ZonePos < 0.1f) { ZonePos = 0.1f; _zoneDir =  1; }
    }

    private void MoveMarker(float dt, bool holding)
    {
        MarkerPos = Mathf.Clamp01(MarkerPos + (holding ? _reelSpeed : -_gravity) * dt);
    }

    private void CheckOverlap(float dt)
    {
        bool overlap = (MarkerPos - MARKER_HEIGHT / 2f) < (ZonePos + _zoneHeight / 2f)
                    && (MarkerPos + MARKER_HEIGHT / 2f) > (ZonePos - _zoneHeight / 2f);
        _totalTime += dt;
        if (overlap)
        {
            _overlapTime    += dt;
            CatchProgress    = Mathf.Clamp01(CatchProgress  + CATCH_RATE * dt);
            EscapeProgress   = Mathf.Clamp01(EscapeProgress - ESCAPE_RATE * 2f * dt);
        }
        else
        {
            CatchProgress  = Mathf.Clamp01(CatchProgress  - ESCAPE_RATE * dt);
            EscapeProgress = Mathf.Clamp01(EscapeProgress + ESCAPE_RATE * dt);
        }
    }

    public float GetZoneHeight() => _zoneHeight;
}