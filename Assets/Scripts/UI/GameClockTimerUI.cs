using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockTimerUI : MonoBehaviour
{
    [SerializeField] private Image _imageTimer;
    private void Update()
    {
        _imageTimer.fillAmount = GameManager.Instance.GetPlayingTimerNormalized();
    }
}
