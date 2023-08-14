using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using TMPro;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameLobby;
    private Lobby _lobby;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithId(_lobby.Id);
        });
    }
    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _nameLobby.text = lobby.Name;
    }
}
