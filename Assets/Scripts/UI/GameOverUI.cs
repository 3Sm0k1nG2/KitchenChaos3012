using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private KitchenChaosGameManager gameManager;
    [SerializeField] private DeliveryManager deliveryManager;

    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button MainMenuButton;

    private void Start()
    {
        gameManager.OnStateChanged += GameManager_OnStateChanged;
        MainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (!gameManager.IsGameOverActive())
        {
            if (isActiveAndEnabled)
            {
                Hide();
            }
            return;
        }

        Show();
        UpdateTextWithCorrectRecipesDeliveredCount();
    }

    private void UpdateTextWithCorrectRecipesDeliveredCount()
    {
        recipesDeliveredText.text = deliveryManager.GetCorrectRecipesDeliveredCount().ToString();
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
