using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : ProgressBarUIParentCounter
{
    public override event EventHandler<IProgressBarUIParent.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<OnTurnOnOffEventArgs> OnTurnOnOff;
    public class OnTurnOnOffEventArgs : EventArgs {
        public bool isOn;
    };

    public event EventHandler OnStartCooking;
    public event EventHandler OnStopCooking;

    [SerializeField] private StoveRecipeSO[] stoveRecipeSOArray;

    public GameObject sizzlingParticles;
    public GameObject stoveOnVisual;

    private bool _isOn;
    private bool IsOn {
        get {
            return _isOn;
        }
        set {
            _isOn = value;
            OnTurnOnOff?.Invoke(this, new OnTurnOnOffEventArgs {
                isOn = _isOn
            });
            if(!_isOn && IsCooking) {
                OnStopCooking?.Invoke(this, EventArgs.Empty);
                IsCooking = false;
            }
        }
    }

    private bool _isCooking;
    private bool IsCooking {
        get {
            return _isCooking;
        }
        set {
            _isCooking = value;
            if (_isCooking) {
                OnStartCooking?.Invoke(this, EventArgs.Empty);
            } else {
                OnStopCooking?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private bool hasKitchenObjectChanged;

    private static Dictionary<KitchenObject, int> kitchenObjectCookingProgressDictionary;
    private int _cookingProgress;
    private int CookingProgress {
        get {
            return _cookingProgress;
        }
        set {
            _cookingProgress = value;

            OnProgressChanged?.Invoke(this, new IProgressBarUIParent.OnProgressChangedEventArgs {
                progressNormalized = (float)CookingProgress / GetCookingProgressThreshold(GetKitchenObject()?.GetKitchenObjectSO())
            });
        }
    }

    private void Awake() {
        kitchenObjectCookingProgressDictionary = new Dictionary<KitchenObject, int>();
    }

    private void KitchenObject_OnDestroySelf(object sender, KitchenObject.OnDestroySelfEventArgs e) {
        kitchenObjectCookingProgressDictionary.Remove(e.thisAsKitchenObject);
    }

    private void Update() {
        if (!IsOn) {
            return;
        }

        if (!HasKitchenObject()) {
            return;
        }

        if (!IsCooking) {
            if (kitchenObjectCookingProgressDictionary.TryGetValue(GetKitchenObject(), out int cookingProgress)) {
                CookingProgress = cookingProgress;
                IsCooking = true;
            } else {
                if (GetCookingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO()) is null) {
                    Debug.Log(GetKitchenObject().name + " has no CookingRecipeSO.");
                    return;
                }

                kitchenObjectCookingProgressDictionary.Add(GetKitchenObject(), CookingProgress);
                IsCooking = true;
                GetKitchenObject().OnDestroySelf += KitchenObject_OnDestroySelf;
            }
        } else {
            CookingProgress++;
            kitchenObjectCookingProgressDictionary[GetKitchenObject()] = CookingProgress;
            if (CookingProgress < GetCookingProgressThreshold(GetKitchenObject().GetKitchenObjectSO())) {
                return;
            }

            KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            if (kitchenObjectSO == null) {
                return;
            }

            CompleteCooking(kitchenObjectSO);
        }

        //if (HasKitchenObject()) {
        //    if (!IsCooking) {
        //        if(kitchenObjectCookingProgressDictionary.TryGetValue(GetKitchenObject(), out int cookingProgress)) {
        //            CookingProgress = cookingProgress;
        //            IsCooking = true;
        //        } else {
        //            if (GetCookingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO()) is null) {
        //                return;
        //            }

        //            kitchenObjectCookingProgressDictionary.Add(GetKitchenObject(), CookingProgress);
        //            IsCooking = true;
        //        }
        //    } else {
        //        CookingProgress++;
        //        kitchenObjectCookingProgressDictionary[GetKitchenObject()] = CookingProgress;
        //        if (CookingProgress < GetCookingProgressThreshold(GetKitchenObject().GetKitchenObjectSO())) {
        //            return;
        //        }

        //        KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        //        if(kitchenObjectSO == null) {
        //            return;
        //        }

        //        CompleteCooking(kitchenObjectSO);
        //    }
        //} else {
        //    if(IsCooking) {
        //        IsCooking = false;
        //    }
        //}
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                Debug.Log(name + " has nothing to interact with.");
            } else {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                hasKitchenObjectChanged = true;
            }
        } else {
            if (player.HasKitchenObject()) {
                Debug.Log(name + " already has a KitchenObject.");
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
                CookingProgress = 0;
                if (IsCooking) {
                    IsCooking = false;
                }
            }
        }
    }

    public override void InteractAlternate(Player player) {
        IsOn = !IsOn;
    }

    private void CompleteCooking(KitchenObjectSO kitchenObjectSO) {
        CookingProgress = 0;
        IsCooking = false;
        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnObject(kitchenObjectSO, this);
        hasKitchenObjectChanged = true;

        Debug.Log("Cooked a " + GetKitchenObject().name + "!");
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetCookingRecipeSOWithInput(inputKitchenObjectSO)?.output;
    }

    private int GetCookingProgressThreshold(KitchenObjectSO inputKitchenObjectSO) {
        StoveRecipeSO stoveRecipeSO = GetCookingRecipeSOWithInput(inputKitchenObjectSO);
        if (stoveRecipeSO == null) {
            return 1;
        }

        return stoveRecipeSO.cookingProgressThreshold;
    }

    private StoveRecipeSO GetCookingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (StoveRecipeSO stoveRecipeSO in stoveRecipeSOArray) {
            if (stoveRecipeSO.input == inputKitchenObjectSO) {
                return stoveRecipeSO;
            }
        }

        return null;
    }
}
