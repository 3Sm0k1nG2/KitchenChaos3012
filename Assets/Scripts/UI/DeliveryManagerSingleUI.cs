using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach(var kicthenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTranform = Instantiate(iconTemplate, iconContainer);
            iconTranform.gameObject.SetActive(true);
            iconTranform.GetComponent<Image>().sprite = kicthenObjectSO.sprite;
        }
    }
}
