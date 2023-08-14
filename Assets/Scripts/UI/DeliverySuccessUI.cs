using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliverySuccessUI : MonoBehaviour
{
    private const string POPUP = "Popup";
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Color _colorSuccess;
    [SerializeField] private Color _colorFail;
    [SerializeField] private Sprite _iconSuccess;
    [SerializeField] private Sprite _iconFail;
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        DeliveryManager.Instance.OnRecipeSuccess += OnDeliverySuccess;
        DeliveryManager.Instance.OnRecipeFailed += OnDeliveryFailed;
        gameObject.SetActive(false);
    }
    private void OnDeliverySuccess(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        _bgImage.color = _colorSuccess;
        _iconImage.sprite = _iconSuccess;
        _text.text = "DELIVERY\nSUCCESS";
    }
    private void OnDeliveryFailed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        _bgImage.color = _colorFail;
        _iconImage.sprite = _iconFail;
        _text.text = "DELIVERY\nFAILED";
    }
}
