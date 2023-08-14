using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class ConnectionResponceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _message;
    [SerializeField] private Button _btnClose;
    private void Awake()
    {
        _btnClose.onClick.AddListener(Hide);
    }
    private void Start()
    {
        GameMultiplayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
        GameLobby.Instance.OnLobbyResponceMessageReceived += GameLobby_OnLobbyResponceMessageReceived;
        Hide();
    }


    private void GameLobby_OnLobbyResponceMessageReceived(object sender, GameLobby.ReceivedEventArgs e)
    {
        ShowMessage(e.Message);
    }
    private void OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "") ShowMessage("Failed to connect");
        else ShowMessage(NetworkManager.Singleton.DisconnectReason);
    }
    private void ShowMessage(string message)
    {
        Show();
        _message.text = message;
    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnFailedToJoinGame -= OnFailedToJoinGame;
        GameLobby.Instance.OnLobbyResponceMessageReceived -= GameLobby_OnLobbyResponceMessageReceived;

    }
}
