using UnityEngine;

public class NetworkPauseUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnNetworkPauseGame += GameManager_OnNetworkPauseGame;
        Hide();
    }
    private void GameManager_OnNetworkPauseGame(object sender, bool enable)
    {
        if (GameManager.Instance.IsLocalPaused) return;
        if (enable) Show(); else Hide();
    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
}
