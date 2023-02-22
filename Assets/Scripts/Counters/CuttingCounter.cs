using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private static Dictionary<KitchenObject, int> kitchenObjectCuttingProgressDictionary;
    private int _cuttingProgress;
    private int CuttingProgress {
        get {
            return _cuttingProgress;
        }
        set {
            _cuttingProgress = value;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = (float)CuttingProgress / GetCuttingProgressThreshold(GetKitchenObject()?.GetKitchenObjectSO())
            });
        }
    }

    private bool hasCuttingRecipeSO;
    private bool isKitchenObjectChanged;

    private void Awake() {
        kitchenObjectCuttingProgressDictionary = new Dictionary<KitchenObject, int>();
    }

    private void KitchenObject_OnDestroySelf(object sender, KitchenObject.OnDestroySelfEventArgs e) {
        kitchenObjectCuttingProgressDictionary.Remove(e.thisAsKitchenObject);
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                Debug.Log(name + " has nothing to interact with.");
                return;
            }
            ReceiveKitchenObjectFromPlayer(player);
            kitchenObjectCuttingProgressDictionary.TryGetValue(GetKitchenObject(), out int CuttingProgress);
            this.CuttingProgress = CuttingProgress;
            isKitchenObjectChanged = true;
        } else {
            if (player.HasKitchenObject()) {
                Debug.Log(name + " already has a KitchenObject.");
                return;
            }
            GiveKitchenObjectToPlayer(player);
            this.CuttingProgress = 0;
        }
    }

    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) {
            Debug.Log("Nothing to interact with.");
            return;
        }

        if (isKitchenObjectChanged) {
            if (kitchenObjectCuttingProgressDictionary.ContainsKey(GetKitchenObject())) {
                    if (kitchenObjectCuttingProgressDictionary.TryGetValue(GetKitchenObject(), out int CuttingProgress)) {
                        this.CuttingProgress = CuttingProgress;
                        hasCuttingRecipeSO = true;
                    }
            } else {
                ValidateCurrentKitchenObjectHavingCuttingRecipeSO();
                if (hasCuttingRecipeSO) {
                    kitchenObjectCuttingProgressDictionary.Add(GetKitchenObject(), CuttingProgress);
                    GetKitchenObject().OnDestroySelf += KitchenObject_OnDestroySelf;
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
        CuttingProgress++;
        kitchenObjectCuttingProgressDictionary[GetKitchenObject()] = CuttingProgress;

        OnCut?.Invoke(this, EventArgs.Empty);
        
        if (CuttingProgress < GetCuttingProgressThreshold(GetKitchenObject().GetKitchenObjectSO())) {
            Debug.Log(CuttingProgress + "/" + GetCuttingProgressThreshold(GetKitchenObject().GetKitchenObjectSO()));
            return;
        }

        CompleteSlicing();
    }

    private void CompleteSlicing() {
        kitchenObjectCuttingProgressDictionary.Remove(GetKitchenObject());
        CuttingProgress = 0;
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

    private int GetCuttingProgressThreshold(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO is null) {
            return 1;
        }

        return cuttingRecipeSO.cuttingProgressThreshold;
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
