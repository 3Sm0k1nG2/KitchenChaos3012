using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    [SerializeField] private DeliveryManager deliveryManager;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        deliveryManager.OnRecipeSpawn += DeliveryManager_OnRecipeSpawn;
        deliveryManager.OnRecipeComplete += DeliveryManager_OnRecipeComplete;

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeComplete(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawn(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform childTransform in container)
        {
            if (childTransform == recipeTemplate) continue;
            
            Destroy(childTransform.gameObject);
        }

        foreach(var recipe in deliveryManager.GetWaitingRecipeSOList())
        {
            Transform recipeTranform = Instantiate(recipeTemplate, container);
            recipeTranform.gameObject.SetActive(true);
            recipeTranform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipe);
        }
    }
}
