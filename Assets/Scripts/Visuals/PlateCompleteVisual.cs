using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> _kitchenObjectSO_GameObjects;
    private void Start()
    {
        _plateKitchenObject.OnAddIngredient += OnPlateSpawned;
        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in _kitchenObjectSO_GameObjects)
            kitchenObjectSO_GameObject.gameObject.SetActive(false);
    }
    private void OnPlateSpawned(object sender, PlateKitchenObject.OnAddIngredientEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in _kitchenObjectSO_GameObjects)
        {
            if (kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO)
                kitchenObjectSO_GameObject.gameObject.SetActive(true);
        }
    }
}
