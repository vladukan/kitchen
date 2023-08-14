using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private Transform _iconTemplate;
    private void Start()
    {
        _iconTemplate.gameObject.SetActive(false);
        _plateKitchenObject.OnAddIngredient += OnAddIngredient;
    }
    private void OnAddIngredient(object sender, PlateKitchenObject.OnAddIngredientEventArgs e)
    {
        UpdateVisuals();
    }
    private void UpdateVisuals()
    {
        foreach (Transform child in transform)
        {
            if (child == _iconTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO kitchenObjectSO in _plateKitchenObject.GetKitchenObjects())
        {
            Transform icon = Instantiate(_iconTemplate, transform);
            icon.gameObject.SetActive(true);
            icon.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);

        }
    }
}
