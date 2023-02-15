using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    private const int CUTTING_PROGRESS_THRESHOLD = 5;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private static Dictionary<KitchenObject, int> kitchenObjectCuttingProgressDictionary;
    private int cuttingProgress;
    private bool isSliced;
    private bool hasCuttingRecipeSO;
    private bool isKitchenObjectChanged;

    private void Awake() {
        kitchenObjectCuttingProgressDictionary = new Dictionary<KitchenObject, int>();
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            ReceiveKitchenObjectFromPlayer(player);
            isKitchenObjectChanged = true;
            cuttingProgress = 0;
        } else {
            GiveKitchenObjectToPlayer(player);
        }
    }
    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) {
            Debug.Log("Nothing to interact with.");
            return;
        }

        if (isKitchenObjectChanged) {
            if (kitchenObjectCuttingProgressDictionary.ContainsKey(GetKitchenObject())) {
                    if (kitchenObjectCuttingProgressDictionary.TryGetValue(GetKitchenObject(), out int cuttingProgress)) {
                        this.cuttingProgress = cuttingProgress;
                        hasCuttingRecipeSO = true;
                    }
            } else {
                ValidateCurrentKitchenObjectHavingCuttingRecipeSO();
                if (hasCuttingRecipeSO) {
                    kitchenObjectCuttingProgressDictionary.Add(GetKitchenObject(), cuttingProgress);
                }
            }

            isKitchenObjectChanged = false;
        }

        if (!hasCuttingRecipeSO) {
            Debug.Log(GetKitchenObject().name + " has no CuttingRecipeSO.");
            return;
        }

        SliceKitchenObject();
    }

    private void ValidateCurrentKitchenObjectHavingCuttingRecipeSO() {
        KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        hasCuttingRecipeSO = outputKitchenObjectSO != null;
    }

    private void SliceKitchenObject() {
        cuttingProgress++;
        kitchenObjectCuttingProgressDictionary[GetKitchenObject()] = cuttingProgress;
        if (cuttingProgress < CUTTING_PROGRESS_THRESHOLD) {
            Debug.Log(cuttingProgress + "/" + CUTTING_PROGRESS_THRESHOLD);
            return;
        }

        CompleteSlicing();
    }

    private void CompleteSlicing() {
        kitchenObjectCuttingProgressDictionary.Remove(GetKitchenObject());
        cuttingProgress = 0;
        isKitchenObjectChanged = true;

        KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        if(outputKitchenObjectSO == null) {
            return;
        }

        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnObject(outputKitchenObjectSO, this);

        Debug.Log("Sliced!");
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }


}
