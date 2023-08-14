using UnityEngine;
using Unity.Netcode;
public interface IKitchenObjectParent
{
    public Transform GetKitchenObjFollowTransform();
    public KitchenObject GetKitchenObject();
    public bool HasKitchenObject();
    public void SetKitchenObject(KitchenObject kitchenObject);
    public void ClearKitchenObject();
    public NetworkObject GetNetworkObject();
}
