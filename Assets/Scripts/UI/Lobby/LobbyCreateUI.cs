using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _createPublicButton;
    [SerializeField] private Button _createPrivateButton;
    [SerializeField] private TMP_InputField _lobbyNameInput;
    private void Awake()
    {
        _cancelButton.onClick.AddListener(Hide);
        _createPublicButton.onClick.AddListener(() => { GameLobby.Instance.CreateLobby(_lobbyNameInput.text, false); });
        _createPrivateButton.onClick.AddListener(() => { GameLobby.Instance.CreateLobby(_lobbyNameInput.text, true); });
    }
    private void Start()
    {
        Hide();
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
