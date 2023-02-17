using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().GetKitchenObjectSO() == kitchenObjectSO) {
                    player.GetKitchenObject().DestroySelf();
                    OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
                } else {
                    ReceiveKitchenObjectFromPlayer(player);
                }
            } else {
                KitchenObject.SpawnObject(kitchenObjectSO, player);
                OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            }
        } else {
            if (player.HasKitchenObject()) {
                Debug.Log(name + " already has a KitchenObject.");
                return;
            }

            GiveKitchenObjectToPlayer(player);
        }
    }
    public override void InteractAlternate(Player player) {
        Debug.Log(name + " has no method InteractAlternate().");
    }
}
