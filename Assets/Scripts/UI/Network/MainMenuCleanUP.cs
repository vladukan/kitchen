using UnityEngine;
using Unity.Netcode;

public class MainMenuCleanUP : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
        if (GameMultiplayer.Instance != null) Destroy(GameMultiplayer.Instance.gameObject);
        if (GameLobby.Instance != null) Destroy(GameLobby.Instance.gameObject);
    }
}
