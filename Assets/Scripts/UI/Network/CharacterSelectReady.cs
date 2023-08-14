using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }
    public event EventHandler OnSetPlayerReady;
    private Dictionary<ulong, bool> _playersReady = new Dictionary<ulong, bool>();
    private void Awake()
    {
        Instance = this;
    }
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playersReady[serverRpcParams.Receive.SenderClientId] = true;
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        bool allReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playersReady.ContainsKey(clientId) || !_playersReady[clientId])
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            GameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        _playersReady[clientId] = true;
        OnSetPlayerReady?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerReady(ulong clientId)
    {
        return _playersReady.ContainsKey(clientId) && _playersReady[clientId];
    }
}
