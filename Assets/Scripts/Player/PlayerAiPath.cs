using UnityEngine;
using Pathfinding;
using Unity.Netcode;

public class PlayerAiPath : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private LayerMask _mask;
    private Player _player;
    private AIPath _aiPath;
    private bool _isWalking = false;
    private Transform _tr;
    private Seeker _seeker;
    private Ray _ray;
    private RaycastHit _hit;
    private Camera _camera;
    private BaseCounter _counter;
    public bool IsWalking => _isWalking;
    private void Awake()
    {
        _tr = transform;
        _seeker = GetComponent<Seeker>();
        _aiPath = GetComponent<AIPath>();
        _player = GetComponent<Player>();
        _camera = Camera.main;
        _aiPath.maxSpeed = _moveSpeed;
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (!GameManager.Instance.IsGamePlaying)
        {
            _isWalking = false;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, _mask)) GoToTransrom(_hit.transform, _hit.point);
        }
        _isWalking = _aiPath.velocity.magnitude > 0.1f ? true : false;
    }
    private void GoToTransrom(Transform counter, Vector3 point)
    {
        if (counter.TryGetComponent<BaseCounter>(out BaseCounter counterScript))
        {
            _counter = counterScript;
            _player.SetWaitForSelect(_counter);
            _aiPath.destination = point;
        }
        else
        {
            _counter = null;
            _aiPath.destination = point;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(Vector3 pos)
    {
        if (_counter != null) _player.SetWaitForSelect(_counter);
        _aiPath.destination = pos;
    }

}
