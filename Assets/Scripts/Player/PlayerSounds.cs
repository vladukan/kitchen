using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player _player;
    private float _footTimer;
    private float _footTimerMax = .1f;
    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    private void Update()
    {
        _footTimer -= Time.deltaTime;
        if (_footTimer <= 0)
        {
            _footTimer = _footTimerMax;
            if (_player.IsWalking)
            {
                SoundManager.Instance.PlayFootSteps(_player.transform.position);
            }
        }
    }
}
