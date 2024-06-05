using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if(!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                ReceiveKitchenObjectFromPlayer(player);
            }
            else
            {
                GiveKitchenObjectToPlayer(player);
            }

        } else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                } else
                {
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                GiveKitchenObjectToPlayer(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        Debug.Log(name + " has no method InteractAlternate().");
    }
}
