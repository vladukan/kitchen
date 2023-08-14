using UnityEngine;

public class StoveFlashingUI : MonoBehaviour
{
    private const string IS_FLAH = "IsFlash";
    [SerializeField] private StoveCounter _stoveCounter;
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _stoveCounter.OnProgressChanged += OnProgressChanged;
        _animator.SetBool(IS_FLAH, false);
    }
    private void OnProgressChanged(object sender, IHasProgress.ProgressEventArgs e)
    {
        float progress = .1f;
        bool show = _stoveCounter.IsFried() && e.ProgressNormalized >= progress;
        _animator.SetBool(IS_FLAH, show);
    }
}
