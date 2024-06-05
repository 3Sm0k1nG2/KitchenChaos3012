using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform == iconTemplate) continue;

            Destroy(childTransform.gameObject);
        }

        foreach (var kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
        {
            var iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKichenObjectSO(kitchenObjectSO);
            iconTransform.gameObject.SetActive(true);
        }
    }
}
