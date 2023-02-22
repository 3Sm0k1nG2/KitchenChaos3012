using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public enum States {
        Off, 
        On,
        Uncookable,
        Cooking
    }

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public States state;
    };

    [SerializeField] private States State;
    private States prevState;
    //private States State {
    //    get {
    //        return _state;
    //    }
    //    set {
    //        _state = value;
    //        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
    //            state = _state
    //        });
    //    }
    //}

    [SerializeField] private StoveRecipeSO[] stoveRecipeSOArray;

    private bool hasKitchenObjectChanged = true;

    private static Dictionary<KitchenObject, float> kitchenObjectCookingTimerDictionary;
    private float _cookingTimer;
    private float CookingTimer {
        get {
            return _cookingTimer;
        }
        set {
            _cookingTimer = value;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = CookingTimer / GetCookingTimerMax(GetKitchenObject()?.GetKitchenObjectSO())
            });
        }
    }

    private void Awake() {
        kitchenObjectCookingTimerDictionary = new Dictionary<KitchenObject, float>();
    }

    private void KitchenObject_OnDestroySelf(object sender, KitchenObject.OnDestroySelfEventArgs e) {
        kitchenObjectCookingTimerDictionary.Remove(e.thisAsKitchenObject);
    }

    private void Update() {

        if (State != prevState) {
            InvokeOnStateChangedEvent(State);
            prevState = State;
        }

        if (hasKitchenObjectChanged) {
            if (HasKitchenObject()
                && kitchenObjectCookingTimerDictionary.TryGetValue(GetKitchenObject(), out float cookingTimer)
            ) {
                CookingTimer = cookingTimer;
            } else {
                CookingTimer = 0f;
            }
        } 

        switch (State) {
            case States.Off:
                break;
            case States.On:
                hasKitchenObjectChanged = false;

                if (!HasKitchenObject()) {
                    return;
                }

                if (GetCookingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO()) is null) {
                    Debug.Log(GetKitchenObject().name + " has no CookingRecipeSO.");
                    State = States.Uncookable;
                    return;
                }

                if (!kitchenObjectCookingTimerDictionary.ContainsKey(GetKitchenObject())){
                    kitchenObjectCookingTimerDictionary.Add(GetKitchenObject(), CookingTimer);
                    GetKitchenObject().OnDestroySelf += KitchenObject_OnDestroySelf;
                }

                State = States.Cooking;

                break;
            case States.Uncookable:
                if (!HasKitchenObject()) {
                    return;
                }

                if (hasKitchenObjectChanged) {
                    State = States.On;
                }

                break;
            case States.Cooking:
                if (!HasKitchenObject()) {
                    return;
                }

                if (!kitchenObjectCookingTimerDictionary.ContainsKey(GetKitchenObject())) {
                    State = States.Uncookable;
                    return;
                }

                CookingTimer += Time.deltaTime;
                kitchenObjectCookingTimerDictionary[GetKitchenObject()] = CookingTimer;
                if (CookingTimer < GetCookingTimerMax(GetKitchenObject().GetKitchenObjectSO())) {
                    return;
                }

                KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                if (kitchenObjectSO == null) {
                    return;
                }

                CompleteCooking(kitchenObjectSO);
                State = States.On;

                break;
        }
    }


    private void InvokeOnStateChangedEvent(States state) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
            state = state
        });
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                Debug.Log(name + " has nothing to interact with.");
            } else {
                ChangeKitchenObjectParentTo(player.GetKitchenObject(), this);
            }
        } else {
            if (player.HasKitchenObject()) {
                Debug.Log(name + " already has a KitchenObject.");
            } else {
                ChangeKitchenObjectParentTo(GetKitchenObject(), player);
                CookingTimer = 0;
                if (State == States.Cooking) {
                    State = States.On;
                }
            }
        }
    }

    public override void InteractAlternate(Player player) {
        State = (State == States.Off) ? States.On : States.Off;
    }

    private void CompleteCooking(KitchenObjectSO kitchenObjectSO) {
        GetKitchenObject().DestroySelf();
        ChangeKitchenObjectParentTo(KitchenObject.SpawnObject(kitchenObjectSO, this), this);

        Debug.Log("Cooked a " + GetKitchenObject().name + "!");
    }

    private void ChangeKitchenObjectParentTo(KitchenObject kitchenObject, IKitchenObjectParent kitchenObjectParent) {
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        hasKitchenObjectChanged = true;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetCookingRecipeSOWithInput(inputKitchenObjectSO)?.output;
    }

    private float GetCookingTimerMax(KitchenObjectSO inputKitchenObjectSO) {
        StoveRecipeSO stoveRecipeSO = GetCookingRecipeSOWithInput(inputKitchenObjectSO);
        if (stoveRecipeSO == null) {
            return 1;
        }

        return stoveRecipeSO.cookingTimerMax;
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
