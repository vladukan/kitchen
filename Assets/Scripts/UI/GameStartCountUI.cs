using UnityEngine;
using TMPro;

public class GameStartCountUI : MonoBehaviour
{
    private const string NUMBER_POPUP = "NumberPopup";
    [SerializeField] private TMP_Text _textCountDown;
    private Animator _animator;
    private int _prevCountDown;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        GameManager.Instance.OnStateChanged += OnStateChanged;
        Hide();
    }
    private void Update()
    {
        int countDown = Mathf.CeilToInt(GameManager.Instance.CountTimerToStart);
        _textCountDown.text = countDown.ToString();
        if (_prevCountDown != countDown)
        {
            _prevCountDown = countDown;
            _animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountDownSound();
        }
    }
    private void OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountStartGame) Show();
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
