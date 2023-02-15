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
                ReceiveKitchenObjectFromPlayer(player);
                return;
            }
            KitchenObject.SpawnObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (player.HasKitchenObject()) {
            Debug.Log(player.name + " already has a KitchenObject.");
            return;
        }

        GiveKitchenObjectToPlayer(player);
    }
}
