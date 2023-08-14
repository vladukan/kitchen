using UnityEngine;
using System;
using Unity.Netcode;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyDrop;
    public static void ResetStaticData() => OnAnyDrop = null;
    [SerializeField] private Transform TopPoint;
    private KitchenObject KitchenObject;
    public virtual void Interact(Player player) { }
    public virtual void InteractCutting(Player player) { }
    public Transform GetKitchenObjFollowTransform() { return TopPoint; }
    public KitchenObject GetKitchenObject() { return KitchenObject; }
    public bool HasKitchenObject() { return KitchenObject != null; }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        KitchenObject = kitchenObject;
        if (KitchenObject != null)
        {
            OnAnyDrop?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ClearKitchenObject() => KitchenObject = null;
    public NetworkObject GetNetworkObject() { return NetworkObject; }
}
