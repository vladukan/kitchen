using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Authentication;

public class GameMultiplayer : NetworkBehaviour
{
    public const int MaxPlayerCount = 4;
    public const string PlayerNamePrefs = "PlayerName";
    public static GameMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenListSO _kitchenList;
    [SerializeField] private List<Color> _playerColorList;

    private NetworkList<PlayerData> _playerDataList;
    private string _playerName;
    public string PlayerName => _playerName;
    public static bool PlayMultiplayer;
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataListChanged;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerName = PlayerPrefs.GetString(PlayerNamePrefs, "Player_" + UnityEngine.Random.Range(100, 1000));
        _playerDataList = new NetworkList<PlayerData>();
        _playerDataList.OnListChanged += PlayerDataListChanged;
    }
    private void Start()
    {
        if (!PlayMultiplayer)
        {
            GameLobby.Instance.CreateLobby("Single", true);
        }
    }
    public void SetPlayerName(string name)
    {
        _playerName = name;
        PlayerPrefs.SetString(PlayerNamePrefs, _playerName);
        PlayerPrefs.Save();
    }
    private void PlayerDataListChanged(NetworkListEvent<PlayerData> list)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += Server_OnClientDisconnect;
        NetworkManager.Singleton.StartHost();
    }
    private void Server_OnClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < _playerDataList.Count; i++)
            if (_playerDataList[i].ClientId == clientId) _playerDataList.RemoveAt(i);
    }
    private void OnClientConnect(ulong clientId)
    {
        _playerDataList.Add(new PlayerData
        {
            ClientId = clientId,
            ColorId = GetFirstUnusedColorId(),
        });
        SetPlayerNameServerRpc(PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }
    private void OnClientConnectCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataList[index];
        playerData.PlayerName = name;
        _playerDataList[index] = playerData;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataList[index];
        playerData.PlayerId = playerId;
        _playerDataList[index] = playerData;
    }
    private void OnConnectionApproval(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Scene is not CharacterScene";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MaxPlayerCount)
        {
            response.Approved = false;
            response.Reason = "Game is full";
            return;
        }
        response.Approved = true;
    }
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.StartClient();
    }
    private void OnClientDisconnect(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObject)
    {
        networkObject.TryGet(out NetworkObject kitchenNetworkObject);
        if (kitchenNetworkObject == null) return;
        ClearKitchenObjectParentClientRpc(networkObject);
        kitchenNetworkObject.GetComponent<KitchenObject>().DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference networkObject)
    {
        networkObject.TryGet(out NetworkObject kitchenNetworkObject);
        kitchenNetworkObject.GetComponent<KitchenObject>().ClearKitchenObject();
    }
    public void SpawnKitchenObject(
        KitchenObjectSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent
        )
    {
        SpawnKitchenObjectServerRpc(GetIndexKitchenObjectSO(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int index, NetworkObjectReference networkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOByIndex(index);
        networkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenNetworkObject.GetComponent<IKitchenObjectParent>();
        if (kitchenObjectParent.HasKitchenObject()) return;
        Transform objectSO = Instantiate(kitchenObjectSO.Prefab);
        NetworkObject networkObject = objectSO.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
        KitchenObject kitchenObject = objectSO.GetComponent<KitchenObject>();

        kitchenObject.SetKitchenObjParent(kitchenObjectParent);

    }
    private bool IsColorAvialable(int colorId)
    {
        foreach (PlayerData playerData in _playerDataList)
            if (playerData.ColorId == colorId) return false;
        return true;
    }
    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < _playerColorList.Count; i++)
            if (IsColorAvialable(i)) return i;
        return 0;
    }
    public int GetIndexKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        return _kitchenList.KitchenObjectSoList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
    {
        return _kitchenList.KitchenObjectSoList[index];
    }
    public bool IsPlayerIndexConnected(int playeIndex)
    {
        return playeIndex < _playerDataList.Count;
    }
    public PlayerData GetPlayerDataFromIndex(int index)
    {
        return _playerDataList[index];
    }
    public Color GetPlayerColor(int index)
    {
        return _playerColorList[index];
    }
    public void ChangePlayerColor(int colorId) => ChangePlayerColorServerRpc(colorId);

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvialable(colorId)) return;
        int index = GetIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataList[index];
        playerData.ColorId = colorId;
        _playerDataList[index] = playerData;
    }

    public PlayerData GetPlayerData(int colorId)
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        Server_OnClientDisconnect(clientId);
    }
    public int GetIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < _playerDataList.Count; i++)
            if (_playerDataList[i].ClientId == clientId) return i;
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in _playerDataList)
            if (playerData.ClientId == clientId) return playerData;
        return default;
    }
}
