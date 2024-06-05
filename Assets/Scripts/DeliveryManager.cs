using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static event EventHandler<DeliveryManagerEventArgs> OnAnyRecipeSuccess;
    public static event EventHandler<DeliveryManagerEventArgs> OnAnyRecipeFail;
    public class DeliveryManagerEventArgs : EventArgs
    {
        public DeliveryCounter? DeliveryCounter;
    }

    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeComplete;

    [SerializeField] private RecipeListSO recipeListSO;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int correctRecipesDeliveredCount = 0;

    private void Awake()
    {
        waitingRecipeSOList = new List<RecipeSO> ();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                var waitingRecipeSO = recipeListSO.recipesSOList[UnityEngine.Random.Range(0, recipeListSO.recipesSOList.Count)];
                Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject, DeliveryCounter deliveryCounter = null)
    {
        bool isIngredientListMatching;

        foreach(var recipe in waitingRecipeSOList) {

            if (plateKitchenObject.GetKitchenObjectSOList().Count != recipe.kitchenObjectSOList.Count) continue;

            isIngredientListMatching = true;

            foreach (var recipeIngredient in recipe.kitchenObjectSOList)
            {
                if (!plateKitchenObject.GetKitchenObjectSOList().Exists(ingredient => ingredient == recipeIngredient))
                {
                    isIngredientListMatching = false;
                    break;
                }
            }

            if (!isIngredientListMatching) continue;

            OnCorrectRecipeDeliver(recipe, deliveryCounter);
            return;
        }

        OnIncorrectRecipeDeliver(deliveryCounter);
    }

    private void OnCorrectRecipeDeliver(RecipeSO recipe, DeliveryCounter deliveryCounter = null)
    {
        Debug.Log($"Found a match. Removed {recipe.recipeName} from the waiting recipe list.");
        waitingRecipeSOList.Remove(recipe);

        OnRecipeComplete?.Invoke(this, EventArgs.Empty);
        OnAnyRecipeSuccess?.Invoke(this, new DeliveryManagerEventArgs()
        {
            DeliveryCounter = deliveryCounter
        });

        correctRecipesDeliveredCount++;
    }

    private void OnIncorrectRecipeDeliver(DeliveryCounter deliveryCounter = null)
    {
        Debug.Log($"Found no matches. Wrong recipe.");
        
        OnAnyRecipeFail?.Invoke(this, new DeliveryManagerEventArgs()
        {
            DeliveryCounter = deliveryCounter
        });
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetCorrectRecipesDeliveredCount()
    {
        return correctRecipesDeliveredCount;
    }
}
