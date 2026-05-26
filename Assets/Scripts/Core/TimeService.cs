using System;
using UnityEngine;

public class TimeService : MonoBehaviour
{
    [SerializeField] private float _debugMultiplier = 1f;

    public float DebugTimeMultiplier => _debugMultiplier;
    public event Action<float> OnScaledTick;

    private void Update()
    {
        if (_debugMultiplier > 1f)
            OnScaledTick?.Invoke(Time.deltaTime * _debugMultiplier);
    }

    public TimeSpan GetOfflineDuration(string lastSave)
    {
        if (string.IsNullOrEmpty(lastSave)) return TimeSpan.Zero;
        if (!DateTime.TryParse(lastSave, out DateTime t)) return TimeSpan.Zero;

        TimeSpan elapsed = DateTime.UtcNow - t;
        if (elapsed.TotalSeconds <= 0) return TimeSpan.Zero;
        if (elapsed.TotalHours > 24) elapsed = TimeSpan.FromHours(24);
        return elapsed;
    }

    public void SetDebugMultiplier(float m) => _debugMultiplier = Mathf.Max(1f, m);
}
