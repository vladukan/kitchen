using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter _baseCounter;
    [SerializeField] private GameObject[] _visualSelected;
    private Player _player;
    private bool _isSelected = false;
    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            _player = Player.LocalInstance;
            _player.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
        }
        else Player.OnAnyPlayerSpawned += PlayerOnAnyPlayerSpawned;
    }
    private void PlayerOnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            _player = Player.LocalInstance;
            _player.OnSelectedCounterChanged -= PlayerOnSelectedCounterChanged;
            _player.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
        }
    }
    private void PlayerOnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.SelectedCounter == _baseCounter)
        {
            Active(true);
            _isSelected = true;
            _baseCounter.Interact(_player);
        }
        else
        {
            Active(false);
            _isSelected = false;
        }
    }
    private void Active(bool active)
    {
        foreach (GameObject select in _visualSelected) select.SetActive(active);
    }
    private void Update()
    {
        if (_isSelected)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) > 2f)
            {
                Active(false);
                _isSelected = false;
            }
        }
    }
}
