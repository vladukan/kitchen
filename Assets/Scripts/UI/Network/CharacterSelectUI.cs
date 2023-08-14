using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button _btnReady;
    [SerializeField] private Button _btnMenu;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _passText;
    private void Awake()
    {
        _btnReady.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
        _btnMenu.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();
        _nameText.text = $"Lobby Name: {lobby.Name}";
        _passText.text = $"Lobby Code: {lobby.LobbyCode}";
    }
}
