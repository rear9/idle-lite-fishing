using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private FishingInputActions _controls;

    public event Action OnHoldStarted;
    public event Action OnHoldCanceled;

    public bool IsHolding { get; private set; }

    private void Awake()
    {
        _controls = new FishingInputActions();
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Start()
    {
        _controls.Fishing.Hold.performed += OnHoldPerformed;
        _controls.Fishing.Hold.canceled += HandleHoldCanceled;
    }

    private void OnHoldPerformed(InputAction.CallbackContext ctx)
    {
        IsHolding = true;
        OnHoldStarted?.Invoke();
    }

    private void HandleHoldCanceled(InputAction.CallbackContext ctx)
    {
        IsHolding = false;
        OnHoldCanceled?.Invoke();
    }
}
