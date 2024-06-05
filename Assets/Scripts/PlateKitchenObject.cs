using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private Transform plateTopPoint;

    [SerializeField] private List<KitchenObjectSO> IngredientsKitchenObjectSOList;
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;


    private void Awake() {
        IngredientsKitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        if (IngredientsKitchenObjectSOList.Contains(kitchenObjectSO)) return false;

        IngredientsKitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });

        return true;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return IngredientsKitchenObjectSOList;
    }
}
