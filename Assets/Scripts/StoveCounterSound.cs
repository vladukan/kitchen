using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    private AudioSource _audioSource;
    private float _warningSoundTimer;
    private bool _playSoundWarning;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        _stoveCounter.OnStateChanged += OnStateChanged;
        _stoveCounter.OnProgressChanged += OnProgressChanged;
    }
    private void OnStateChanged(object sender, StoveCounter.StoveStateEventArgs e)
    {

        bool playSound = e.State == StoveCounter.State.Fried || e.State == StoveCounter.State.Frying;
        if (playSound) _audioSource.Play(); else _audioSource.Pause();
    }
    private void OnProgressChanged(object sender, IHasProgress.ProgressEventArgs e)
    {
        float progress = .1f;
        _playSoundWarning = _stoveCounter.IsFried() && e.ProgressNormalized >= progress;
    }
    private void Update()
    {
        if (_playSoundWarning)
        {
            _warningSoundTimer -= Time.deltaTime;
            if (_warningSoundTimer <= 0)
            {
                float warningTimerMax = .2f;
                _warningSoundTimer = warningTimerMax;
                SoundManager.Instance.PlayWarningSound(_stoveCounter.transform.position);
            }
        }

    }
}
