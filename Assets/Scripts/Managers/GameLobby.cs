using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
public class GameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static GameLobby Instance { get; private set; }
    public event EventHandler<ListChangedEventArgs> OnLobbyListChanged;
    public class ListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList;
    }
    public event EventHandler<ReceivedEventArgs> OnLobbyResponceMessageReceived;
    public class ReceivedEventArgs : EventArgs
    {
        public string Message;
    }
    private Lobby _joinedLobby;
    private float _heartbeatTimer;
    private float _listLobbyTimer;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitAuth();
    }
    private async void InitAuth()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;
        InitializationOptions options = new InitializationOptions();
        //options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    private void Update()
    {
        HandleHeartBeat();
        HandleLobbyListChanged();
    }
    private void HandleLobbyListChanged()
    {
        if (_joinedLobby == null && AuthenticationService.Instance.IsSignedIn
            && SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
        {
            _listLobbyTimer -= Time.deltaTime;
            if (_listLobbyTimer <= 0)
            {
                _listLobbyTimer = 3f;
                ListLobbies();
            }
        }
    }
    private void HandleHeartBeat()
    {
        if (!IsLobbyHost()) return;
        _heartbeatTimer -= Time.deltaTime;
        if (_heartbeatTimer <= 0)
        {
            _heartbeatTimer = 15f;
            LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
        }
    }
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocate = await RelayService.Instance.CreateAllocationAsync(GameMultiplayer.MaxPlayerCount - 1);
            return allocate;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }
    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }
    private bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Creating lobby..." });
        try
        {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MaxPlayerCount,
                    new CreateLobbyOptions
                    {
                        IsPrivate = isPrivate
                    });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member,relayJoinCode) }
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
               new RelayServerData(allocation, "dtls")
            );

            GameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(GameMultiplayer.PlayMultiplayer ? Loader.Scene.CharacterScene : Loader.Scene.GameScene);

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Failed to create lobby" });
        }
    }
    public async void QuikJoinLobby()
    {
        OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Joining lobby..." });
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinRelay = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                new RelayServerData(joinRelay, "dtls")
                );
            GameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs
            {
                Message = "Could not find a lobby to Quik join"
            });
        }
    }
    public async void JoinWithCode(string code)
    {
        OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Joining lobby..." });
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinRelay = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                new RelayServerData(joinRelay, "dtls")
                );
            GameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Failed to join lobby" });
        }
    }
    public async void JoinWithId(string lobbyId)
    {
        OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Joining lobby..." });
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinRelay = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                new RelayServerData(joinRelay, "dtls")
                );
            GameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnLobbyResponceMessageReceived?.Invoke(this, new ReceivedEventArgs { Message = "Failed to join lobby" });
        }
    }
    public async void DeleteLobby()
    {
        if (_joinedLobby == null) return;
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
            _joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async void LeaveLobby()
    {
        if (_joinedLobby == null) return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            _joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async void KickPlayer(string playerId)
    {
        if (!IsLobbyHost()) return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public Lobby GetLobby()
    {
        return _joinedLobby;
    }
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GT)
            }
            };
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            OnLobbyListChanged?.Invoke(this, new ListChangedEventArgs
            {
                LobbyList = response.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    private void OnDestroy()
    {

    }
}
