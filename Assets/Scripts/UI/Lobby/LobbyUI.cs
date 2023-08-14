using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _btnMainMenu;
    [SerializeField] private Button _btnCreate;
    [SerializeField] private Button _btnJoin;
    [SerializeField] private Button _btnJoinCode;
    [SerializeField] private TMP_InputField _joinCodeInput;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private LobbyCreateUI _lobbyCreateUI;
    [SerializeField] private Transform _lobbyListContainer;
    [SerializeField] private Transform _lobbyTemplates;
    private void Awake()
    {
        _btnMainMenu.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        _btnCreate.onClick.AddListener(() => _lobbyCreateUI.Show());
        _btnJoin.onClick.AddListener(() => GameLobby.Instance.QuikJoinLobby());
        _btnJoinCode.onClick.AddListener(() => GameLobby.Instance.JoinWithCode(_joinCodeInput.text));
    }
    private void Start()
    {
        _lobbyTemplates.gameObject.SetActive(false);
        _playerName.text = GameMultiplayer.Instance.PlayerName;
        _playerName.onValueChanged.AddListener((string text) =>
        {
            GameMultiplayer.Instance.SetPlayerName(text);
        });
        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }
    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.ListChangedEventArgs e)
    {
        UpdateLobbyList(e.LobbyList);
    }
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyListContainer)
        {
            if (child == _lobbyTemplates) continue;
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTemplate = Instantiate(_lobbyTemplates, _lobbyListContainer);
            lobbyTemplate.gameObject.SetActive(true);
            lobbyTemplate.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
    private void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }
}