using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour 
{
    public event EventHandler<OnDestroySelfEventArgs> OnDestroySelf;
    public class OnDestroySelfEventArgs : EventArgs {
        public KitchenObject thisAsKitchenObject;
    };

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }

    private void Awake() {
        OnDestroySelf += KitchenObject_OnDestroySelfAfterAllDelegates;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        if(this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;
        if(!kitchenObjectParent.HasKitchenObject()) {
            kitchenObjectParent.SetKitchenObject(this);
        } else {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    public static KitchenObject SpawnObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        kitchenObject.GetComponentInParent<Transform>().localPosition = Vector3.zero;

        return kitchenObject;
    }
    private void KitchenObject_OnDestroySelfAfterAllDelegates(object sender, EventArgs e) {
        kitchenObjectParent.ClearKitchenObject();

        Destroy(gameObject);
    }

    public void DestroySelf() {
        OnDestroySelf?.Invoke(this, new OnDestroySelfEventArgs { thisAsKitchenObject = this });
    }
}
