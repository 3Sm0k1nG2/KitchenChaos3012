using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private KitchenChaosGameManager gameManager;

    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        gameManager.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void Update()
    {
        countdownText.text = Math.Ceiling(gameManager.GetCountdownToStartTimer()).ToString();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (!gameManager.IsCountdownToStartActive())
        {
            if(isActiveAndEnabled)
            {
                Hide();
            }
            return;
        }

        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
