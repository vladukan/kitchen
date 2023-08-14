using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetcodeUI : MonoBehaviour
{
    [SerializeField] private Button _btnHost;
    [SerializeField] private Button _btnClient;
    private void Awake()
    {
        _btnHost.onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.StartHost();
            Hide();
        });
        _btnClient.onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.StartClient();
            Hide();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
