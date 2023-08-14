using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class PlayerCharacterSelector : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private PlayerVisualColor _playerVisualColor;
    [SerializeField] private Button _btnKick;
    [SerializeField] private TMP_Text _playerName;

    private void Awake()
    {
        _btnKick.onClick.AddListener(() =>
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromIndex(_playerIndex);
            GameLobby.Instance.KickPlayer(playerData.PlayerId.ToString());
            GameMultiplayer.Instance.KickPlayer(playerData.ClientId);
        });
    }
    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataListChanged += OnPlayerDataListChanged;
        CharacterSelectReady.Instance.OnSetPlayerReady += OnSetPlayerReady;
        _btnKick.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }
    private void OnSetPlayerReady(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void OnPlayerDataListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (GameMultiplayer.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromIndex(_playerIndex);
            _readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
            _playerName.text = playerData.PlayerName.ToString();
            _playerVisualColor.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
        }
        else
        {
            Hide();
        }

    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataListChanged -= OnPlayerDataListChanged;
    }
}
