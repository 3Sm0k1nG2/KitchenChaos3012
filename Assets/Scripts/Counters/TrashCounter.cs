using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            Debug.Log(player.name + " has no KitchenObject.");
            return;
        }
        player.GetKitchenObject().DestroySelf();
    }
    public override void InteractAlternate(Player player) {
        Debug.Log(name + " has no method InteractAlternate().");
    }
}
