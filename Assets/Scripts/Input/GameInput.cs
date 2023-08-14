using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    [SerializeField] private Button[] _pauseButtons;
    [SerializeField] private Joystick _joystick;
    private PlayerInputActions _playerInputActions;
    public event EventHandler OnInteractAction;
    public event EventHandler OnPauseGame;
    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Interact.performed += InteractPerfomed;
        foreach (var button in _pauseButtons)
            button.onClick.AddListener(() => OnPauseGame?.Invoke(this, EventArgs.Empty));
    }
    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= InteractPerfomed;
        _playerInputActions.Dispose();
    }
    private void InteractPerfomed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void OnEnable()
    {
        _playerInputActions.Enable();
    }
    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
    private void Update()
    {
        if (_playerInputActions.Player.Interact.triggered) OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVec = _playerInputActions.Player.Move.ReadValue<Vector2>();
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            inputVec = new Vector2(_joystick.Horizontal, _joystick.Vertical);
        inputVec = inputVec.normalized;
        return inputVec;
    }
}
