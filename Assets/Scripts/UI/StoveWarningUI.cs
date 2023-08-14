using UnityEngine;

public class StoveWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    private void Start()
    {
        _stoveCounter.OnProgressChanged += OnProgressChanged;
        gameObject.SetActive(false);
    }
    private void OnProgressChanged(object sender, IHasProgress.ProgressEventArgs e)
    {
        float progress = .1f;
        bool show = _stoveCounter.IsFried() && e.ProgressNormalized >= progress;
        if (show) gameObject.SetActive(true); else gameObject.SetActive(false);
    }
}
 