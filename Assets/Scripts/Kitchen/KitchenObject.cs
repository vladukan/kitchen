using UnityEngine;
using Unity.Netcode;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    private IKitchenObjectParent _kitchenObjectParent;
    private FollowTransform _followTransform;
    public KitchenObjectSO KitchenObjectSO => _kitchenObjectSO;
    protected virtual void Awake()
    {
        _followTransform = GetComponent<FollowTransform>();
    }
    public void SetKitchenObjParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjParentServerRpc(kitchenObjectParent.GetNetworkObject());
        // transform.parent = kitchenObjectParent.GetKitchenObjFollowTransform();
        // transform.localPosition = Vector3.zero;
        // transform.localEulerAngles = Vector3.zero;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjParentServerRpc(NetworkObjectReference networkObject)
    {
        SetKitchenObjParentClientRpc(networkObject);
    }
    [ClientRpc]
    private void SetKitchenObjParentClientRpc(NetworkObjectReference networkObject)
    {
        networkObject.TryGet(out NetworkObject kitchenNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenNetworkObject.GetComponent<IKitchenObjectParent>();
        if (_kitchenObjectParent != null) _kitchenObjectParent.ClearKitchenObject();
        _kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()) { }

        _kitchenObjectParent.SetKitchenObject(this);
        _followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjFollowTransform());
    }
    public IKitchenObjectParent GetKitchenObjectParent() { return _kitchenObjectParent; }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void ClearKitchenObject()
    {
        _kitchenObjectParent.ClearKitchenObject();
    }
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        GameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }
    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        GameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }

    }
}
