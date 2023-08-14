using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    [SerializeField] private GameObject _stoveOn;
    [SerializeField] private GameObject _fxOn;
    private void Start()
    {
        _stoveCounter.OnStateChanged += OnStateChanged;
    }
    private void OnStateChanged(object sender, StoveCounter.StoveStateEventArgs e)
    {
        bool showVisual = e.State == StoveCounter.State.Fried || e.State == StoveCounter.State.Frying;
        _stoveOn.SetActive(showVisual);
        _fxOn.SetActive(showVisual);
    }
}
