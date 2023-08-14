
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnAddIngredientEventArgs> OnAddIngredient;
    public class OnAddIngredientEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }
    [SerializeField] private List<KitchenObjectSO> _validKitchenObjectSO;
    private List<KitchenObjectSO> _kitchenObjects = new List<KitchenObjectSO>();
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!_validKitchenObjectSO.Contains(kitchenObjectSO)) return false;
        if (_kitchenObjects.Contains(kitchenObjectSO)) return false;
        else
        {
            AddIngredientServerRpc(GameMultiplayer.Instance.GetIndexKitchenObjectSO(kitchenObjectSO));
            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjects()
    {
        return _kitchenObjects;
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int index)
    {
        AddIngredientClientRpc(index);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int index)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOByIndex(index);
        _kitchenObjects.Add(kitchenObjectSO);
        OnAddIngredient?.Invoke(this, new OnAddIngredientEventArgs { kitchenObjectSO = kitchenObjectSO });
    }
}
