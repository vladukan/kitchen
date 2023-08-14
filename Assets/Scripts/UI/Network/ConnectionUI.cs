using System;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    private void Start()
    {
        GameMultiplayer.Instance.OnTryingToJoinGame += OnTryingToJoinGame;
        GameMultiplayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
        Hide();
    }
    private void OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }
    private void OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnTryingToJoinGame -= OnTryingToJoinGame;
        GameMultiplayer.Instance.OnFailedToJoinGame -= OnFailedToJoinGame;
    }
}
