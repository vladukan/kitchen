using UnityEngine;

public class WaitForPlayersUI : MonoBehaviour
{
    private void Start()
    {
        if (!GameMultiplayer.PlayMultiplayer)
        {
            Hide();
            return;
        }
        GameManager.Instance.OnLocalPlayerReady += GameManager_OnLocalPlayerReady;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }
    private void GameManager_OnLocalPlayerReady(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady) Show();
    }
    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountStartGame) Hide();
    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
}
