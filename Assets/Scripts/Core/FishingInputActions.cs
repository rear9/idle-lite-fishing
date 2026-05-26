using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @FishingInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }

    public @FishingInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""FishingInput"",
    ""maps"": [
        {
            ""name"": ""Fishing"",
            ""id"": ""4a3b2c1d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"",
            ""actions"": [
                {
                    ""name"": ""Hold"",
                    ""type"": ""Button"",
                    ""id"": ""1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b1c2d3e4-f5a6-7b8c-9d0e-1f2a3b4c5d6e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c2d3e4f5-a6b7-8c9d-0e1f-2a3b4c5d6e7f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d3e4f5a6-b7c8-9d0e-1f2a-3b4c5d6e7f8a"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4f5a6b7-c8d9-0e1f-2a3b-4c5d6e7f8a9b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                { ""devicePath"": ""<Keyboard>"", ""isOptional"": false, ""isOR"": false },
                { ""devicePath"": ""<Mouse>"", ""isOptional"": false, ""isOR"": false }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                { ""devicePath"": ""<Gamepad>"", ""isOptional"": false, ""isOR"": false }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                { ""devicePath"": ""<Touchscreen>"", ""isOptional"": false, ""isOR"": false }
            ]
        }
    ]
}");
        m_Fishing = asset.FindActionMap("Fishing", throwIfNotFound: true);
        m_Fishing_Hold = m_Fishing.FindAction("Hold", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action) => asset.Contains(action);
    public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Enable() => asset.Enable();
    public void Disable() => asset.Disable();
    public IEnumerable<InputBinding> bindings => asset.bindings;
    public InputAction FindAction(string nameOrId, bool throwIfNotFound = false) => asset.FindAction(nameOrId, throwIfNotFound);
    public int FindBinding(InputBinding mask, out InputAction action) => asset.FindBinding(mask, out action);

    private readonly InputActionMap m_Fishing;
    private readonly InputAction m_Fishing_Hold;
    private List<IFishingActions> m_FishingActionsCallbackInterfaces = new List<IFishingActions>();

    public struct FishingActions
    {
        private @FishingInputActions m_Wrapper;
        public FishingActions(@FishingInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Hold => m_Wrapper.m_Fishing_Hold;
        public InputActionMap Get() => m_Wrapper.m_Fishing;
        public void Enable() => Get().Enable();
        public void Disable() => Get().Disable();
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FishingActions set) => set.Get();

        public void AddCallbacks(IFishingActions instance)
        {
            if (instance == null || m_Wrapper.m_FishingActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_FishingActionsCallbackInterfaces.Add(instance);
            @Hold.started += instance.OnHold;
            @Hold.performed += instance.OnHold;
            @Hold.canceled += instance.OnHold;
        }

        private void UnregisterCallbacks(IFishingActions instance)
        {
            @Hold.started -= instance.OnHold;
            @Hold.performed -= instance.OnHold;
            @Hold.canceled -= instance.OnHold;
        }

        public void RemoveCallbacks(IFishingActions instance)
        {
            if (m_Wrapper.m_FishingActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IFishingActions instance)
        {
            foreach (var item in m_Wrapper.m_FishingActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_FishingActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }

    public FishingActions @Fishing => new FishingActions(this);

    public interface IFishingActions
    {
        void OnHold(InputAction.CallbackContext context);
    }
}
