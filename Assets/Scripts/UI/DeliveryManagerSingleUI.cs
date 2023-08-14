using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _textName;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _iconTemplate;
    private void Awake() {
        _iconTemplate.gameObject.SetActive(false);
    }
    public void SetReciprSO(RecipeSO recipeSO)
    {
        _textName.text = recipeSO.recipeName;
        foreach(Transform child in _container){
            if(child == _iconTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjects){
            Transform template = Instantiate(_iconTemplate, _container);
            template.gameObject.SetActive(true);
            template.GetComponent<Image>().sprite = kitchenObjectSO.Sprite;
        }
    }
}
