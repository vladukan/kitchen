using System;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPicked;
    public static void ResetStaticData() => OnAnyPlayerSpawned = null;
    public static Player LocalInstance { get; private set; }

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private float _playerRadius = 0.7f;
    [SerializeField] private float _minDistanceSelect = 1.3f;
    [SerializeField] private GameInput _gameInput;
    [SerializeField] private LayerMask _counterLayerMask;
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Vector3[] _startPositions;
    [SerializeField] private PlayerVisualColor _playerVisualColor;
    private KitchenObject _kitchenObject;
    private bool _isWalking = false;
    private bool _isWaitSelect = false;
    private Transform _tr;
    private Vector3 _moveDir;
    private Vector3 _pos;
    private Vector3 _lastInteraction;
    private BaseCounter _selectedCounter;
    private bool _canMove;
    public bool IsWalking => _isWalking;
    public event EventHandler OnPicked;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }
    private void Start()
    {
        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _playerVisualColor.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
    }
    public override void OnNetworkSpawn()
    {
        _tr = transform;
        _tr.position = _startPositions[GameMultiplayer.Instance.GetIndexFromClientId(OwnerClientId)];
        if (IsOwner) LocalInstance = this;
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if (IsServer) NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject()) KitchenObject.DestroyKitchenObject(GetKitchenObject());
    }
    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying)
        {
            _isWalking = false;
            return;
        }
        if (_isWaitSelect)
        {
            if (Vector3.Distance(_tr.position, _selectedCounter.transform.position) < _minDistanceSelect)
                SetSelectedCounter();
        }
        //HandleMovement();
        //HandleInteractions();
    }
    private void HandleInteractions()
    {
        Vector2 inputVector = _gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) _lastInteraction = moveDir;
        //float interactDistance = 2f;
        // if (Physics.Raycast(_tr.position, _lastInteraction, out RaycastHit hit, interactDistance, _counterLayerMask))
        // {
        //     if (hit.transform.TryGetComponent(out BaseCounter counter))
        //     {
        //         if (counter != _selectedCounter) SetSelectedCounter(counter);
        //     }
        //     else SetSelectedCounter(null);
        // }
        // else SetSelectedCounter(null);
    }
    private void HandleMovement()
    {
        Vector2 inputVector = _gameInput.GetMovementVectorNormalized();
        _moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        float moveDistance = _moveSpeed * Time.deltaTime;
        _pos = _tr.position;
        _canMove = !Physics.CapsuleCast(_pos, _pos + Vector3.up * _playerHeight, _playerRadius, _moveDir, moveDistance);
        if (!_canMove)
        {
            Vector3 moveDirX = new Vector3(_moveDir.x, 0, 0).normalized;
            _canMove = (_moveDir.x < -.5f || _moveDir.x > +.5f) && !Physics.CapsuleCast(_pos, _pos + Vector3.up * _playerHeight, _playerRadius, moveDirX, moveDistance);
            if (_canMove) _moveDir = moveDirX;
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, _moveDir.z).normalized;
                _canMove = (_moveDir.z < -.5f || _moveDir.z > +.5f) && !Physics.CapsuleCast(_pos, _pos + Vector3.up * _playerHeight, _playerRadius, moveDirZ, moveDistance);
                if (_canMove) _moveDir = moveDirZ;
            }
        }
        if (_canMove) _tr.position += _moveDir * moveDistance;
        _isWalking = _moveDir != Vector3.zero;
        _tr.forward = Vector3.Slerp(_tr.forward, _moveDir, _rotationSpeed * Time.deltaTime);
    }
    private void SetSelectedCounter()
    {
        _isWaitSelect = false;
        //_selectedCounter.Interact(this);
        OnSelectedCounterChanged?.Invoke(this,
            new OnSelectedCounterChangedEventArgs { SelectedCounter = _selectedCounter });
    }
    public Transform GetKitchenObjFollowTransform() { return _topPoint; }
    public KitchenObject GetKitchenObject() { return _kitchenObject; }
    public bool HasKitchenObject() { return _kitchenObject != null; }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (_kitchenObject != null)
        {
            OnPicked?.Invoke(this, EventArgs.Empty);
            OnAnyPicked?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ClearKitchenObject() => _kitchenObject = null;
    public void SetWaitForSelect(BaseCounter counter)
    {
        _isWaitSelect = true;
        this._selectedCounter = counter;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
