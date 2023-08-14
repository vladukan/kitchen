using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Button _btnClose;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        _btnClose.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReady();
            Hide();
        });
        GameManager_OnStateChanged(this, System.EventArgs.Empty);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsWaitStartGame) Show(); else Hide();
    }
    private void Hide() => gameObject.SetActive(false);
    private void Show() => gameObject.SetActive(true);
}
