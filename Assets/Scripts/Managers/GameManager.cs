using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReady;
    public event EventHandler<bool> OnLocalPauseGame;
    public event EventHandler<bool> OnNetworkPauseGame;
    [SerializeField] private Transform _playerPrefab;
    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }
    private NetworkVariable<float> _countTimerToStart = new NetworkVariable<float>(3f);
    private NetworkVariable<float> _gameTimer = new NetworkVariable<float>(0f);
    private float _gameTimerMax = 240f;
    private Dictionary<ulong, bool> _playersReady = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> _playersPause = new Dictionary<ulong, bool>();
    private bool _isLocalPlayerReady = false;
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);
    private bool _isLocalPaused = false;
    private bool _autoClosePauseGame = false;
    private NetworkVariable<bool> _isGamePause = new NetworkVariable<bool>(false);
    public bool IsLocalPlayerReady => _isLocalPlayerReady;
    public bool IsLocalPaused => _isLocalPaused;
    public float CountTimerToStart => _countTimerToStart.Value;
    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        _state.OnValueChanged += OnStateNetworkChanged;
        _isGamePause.OnValueChanged += OnGamePauseNetworkChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
        }
    }
    private void NetworkManager_OnLoadEventCompleted(string scene, LoadSceneMode mode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform player = Instantiate(_playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        _autoClosePauseGame = true;
    }
    private void OnGamePauseNetworkChanged(bool prev, bool next)
    {
        if (_isGamePause.Value)
        {
            Time.timeScale = 0;
            OnNetworkPauseGame?.Invoke(this, true);
        }
        else
        {
            Time.timeScale = 1;
            OnNetworkPauseGame?.Invoke(this, false);
        }
    }
    private void OnStateNetworkChanged(State prev, State next)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    private void Start()
    {
        GameInput.Instance.OnPauseGame += GameInput_OnPauseGame;
    }
    private void GameInput_OnPauseGame(object sender, System.EventArgs e) => TogglePauseGame();
    private void Update()
    {
        if (!IsServer) return;
        switch (_state.Value)
        {
            case State.WaitingToStart: break;
            case State.CountDownToStart:
                _countTimerToStart.Value -= Time.deltaTime;
                if (_countTimerToStart.Value <= 0)
                {
                    _state.Value = State.GamePlaying;
                    _gameTimer.Value = _gameTimerMax;
                }
                break;
            case State.GamePlaying:
                _gameTimer.Value -= Time.deltaTime;
                if (_gameTimer.Value <= 0)
                {
                    _state.Value = State.GameOver;
                }
                break;
            case State.GameOver: break;

        }
    }
    private void LateUpdate()
    {
        if (_autoClosePauseGame)
        {
            _autoClosePauseGame = false;
            GamePauseState(true);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playersReady[serverRpcParams.Receive.SenderClientId] = true;
        bool allReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playersReady.ContainsKey(clientId) || !_playersReady[clientId])
            {
                allReady = false;
                break;
            }
        }
        if (allReady) _state.Value = State.CountDownToStart;
    }
    public bool IsGamePlaying => _state.Value == State.GamePlaying;
    public bool IsWaitStartGame => _state.Value == State.WaitingToStart;
    public bool IsCountStartGame => _state.Value == State.CountDownToStart;
    public bool IsGameOver => _state.Value == State.GameOver;
    public float GetPlayingTimerNormalized()
    {
        return 1 - (_gameTimer.Value / _gameTimerMax);
    }
    public void SetPlayerReady()
    {
        _isLocalPlayerReady = true;
        OnLocalPlayerReady?.Invoke(this, EventArgs.Empty);
        SetPlayerReadyServerRpc();
    }
    private void TogglePauseGame()
    {
        _isLocalPaused = !_isLocalPaused;
        OnLocalPauseGame?.Invoke(this, _isLocalPaused);
        TogglePauseGameServerRpc(_isLocalPaused);
    }
    [ServerRpc(RequireOwnership = false)]
    private void TogglePauseGameServerRpc(bool enable = true, ServerRpcParams serverRpcParams = default)
    {
        _playersPause[serverRpcParams.Receive.SenderClientId] = enable;
        GamePauseState(enable);
    }
    private void GamePauseState(bool enable)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playersPause.ContainsKey(clientId) && _playersPause[clientId] && enable)
            {
                _isGamePause.Value = true;
                return;
            };
        }
        _isGamePause.Value = false;
    }

}
