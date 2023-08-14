using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject _hasProgressGameObject;
    [SerializeField] private Image _barImage;
    private IHasProgress _hasProgress;
    private void Start()
    {
        _hasProgress = _hasProgressGameObject.GetComponent<IHasProgress>();
        _hasProgress.OnProgressChanged += OnProgressChanged;
        _barImage.fillAmount = 0f;
        Hide();
    }
    private void OnProgressChanged(object sender, IHasProgress.ProgressEventArgs e)
    {
        _barImage.fillAmount = e.ProgressNormalized;
        if (e.ProgressNormalized == 1f || e.ProgressNormalized == 0f) Hide();
        else Show();
    }
    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
