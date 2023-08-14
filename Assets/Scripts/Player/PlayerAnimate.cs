using UnityEngine;
using Unity.Netcode;
public class PlayerAnimate : NetworkBehaviour
{
    private const string NAME_WALK = "IsWalking";
    [SerializeField] private PlayerAiPath _move;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(!IsOwner) return;
        _animator.SetBool(NAME_WALK, _move.IsWalking);
    }
}
