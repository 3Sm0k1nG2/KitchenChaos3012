using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            ReceiveKitchenObjectFromPlayer(player);
        } else {
            GiveKitchenObjectToPlayer(player);
        }
    }

    public override void InteractAlternate(Player player) {
        Debug.Log(name + " has no method InteractAlternate().");
    }
}
