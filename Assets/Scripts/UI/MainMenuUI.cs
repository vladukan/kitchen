using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _singleButton;
    [SerializeField] private Button _multiButton;
    [SerializeField] private Button _quitButton;
    private void Awake()
    {
        _singleButton.onClick.AddListener((() =>
       {
           GameMultiplayer.PlayMultiplayer = false;
           Loader.Load(Loader.Scene.LobbyScene);
       }));
        _multiButton.onClick.AddListener((() =>
        {
            GameMultiplayer.PlayMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        }));
        _quitButton.onClick.AddListener((() =>
        {
            Application.Quit();
        }));
        Time.timeScale = 1f;
    }
}
