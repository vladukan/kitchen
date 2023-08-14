using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject()) player.GetKitchenObject().SetKitchenObjParent(this);
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO))
                        GameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
                }
                else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().KitchenObjectSO))
                            GameMultiplayer.Instance.DestroyKitchenObject(player.GetKitchenObject());
                    }
                }
            }
            else GetKitchenObject().SetKitchenObjParent(player);

        }
    }
}
