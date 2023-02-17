using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public abstract void Interact(Player player);
    public abstract void InteractAlternate(Player player);

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject() { return kitchenObject; }
    public void ClearKitchenObject() {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    protected void GiveKitchenObjectToPlayer(Player player) {
        if (!HasKitchenObject()) {
            Debug.Log(name + " has no KitchenObject");
            return;
        }

        GetKitchenObject().SetKitchenObjectParent(player);
    }

    protected void ReceiveKitchenObjectFromPlayer(Player player) {
        if (HasKitchenObject()) {
            Debug.Log(name + " already has a KitchenObject");
            return;
        }

        player.GetKitchenObject().SetKitchenObjectParent(this);
    }
}
