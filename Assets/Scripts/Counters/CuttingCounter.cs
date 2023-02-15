using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }

    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private static Dictionary<KitchenObject, int> kitchenObjectCuttingProgressDictionary;
    private int _cuttingProgress;

    private int cuttingProgress {
        get {
            return _cuttingProgress;
        }
        set {
            _cuttingProgress = value;

            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / GetProgressThreshold(GetKitchenObject()?.GetKitchenObjectSO())
            });
        }
    }

    private bool hasCuttingRecipeSO;
    private bool isKitchenObjectChanged;

    private void Awake() {
        kitchenObjectCuttingProgressDictionary = new Dictionary<KitchenObject, int>();
    }

    public override void Interact(Player player) {
        if (HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                GiveKitchenObjectToPlayer(player);
                this.cuttingProgress = 0;
            }
        } else {
            if(player.HasKitchenObject()) {
                ReceiveKitchenObjectFromPlayer(player);
                kitchenObjectCuttingProgressDictionary.TryGetValue(GetKitchenObject(), out int cuttingProgress);
                this.cuttingProgress = cuttingProgress;
                isKitchenObjectChanged = true;
            }
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

        OnCut?.Invoke(this, EventArgs.Empty);
        
        if (cuttingProgress < GetProgressThreshold(GetKitchenObject().GetKitchenObjectSO())) {
            Debug.Log(cuttingProgress + "/" + GetProgressThreshold(GetKitchenObject().GetKitchenObjectSO()));
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
        return GetCuttingRecipeSOWithInput(inputKitchenObjectSO)?.output;
    }

    private int GetProgressThreshold(KitchenObjectSO inputKitchenObjectSO) {
        if (GetCuttingRecipeSOWithInput(inputKitchenObjectSO) is null) {
            return 1;
        }

        return GetCuttingRecipeSOWithInput(inputKitchenObjectSO).cuttingProgressThreshold;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

}
