using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _textCount;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _restartButton;
    private void Awake()
    {
        _restartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });
        _mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += OnStateChanged;
        Hide();
    }
    private void Update()
    {
    }
    private void OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver)
        {
            Show();
            _textCount.text = DeliveryManager.Instance.SuccessRecipesAmount.ToString();
        }
        else Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
