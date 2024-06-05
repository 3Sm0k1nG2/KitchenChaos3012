using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    [SerializeField] private DeliveryManager deliveryManager;

    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
            if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                deliveryManager.DeliverRecipe(plateKitchenObject, this);
                player.GetKitchenObject().DestroySelf();
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        throw new System.NotImplementedException();
    }
}
