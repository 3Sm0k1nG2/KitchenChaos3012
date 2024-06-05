using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private KitchenChaosGameManager gameManager;
    [SerializeField] private Image timerImage;

    private void Update()
    {
        timerImage.fillAmount = gameManager.GetGamePlayingTimerNormalized();
    }
}
