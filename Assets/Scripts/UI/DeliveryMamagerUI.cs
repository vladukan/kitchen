using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMamagerUI : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _template;
    private void Awake()
    {
        _template.gameObject.SetActive(false);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += OnUpdate;
        DeliveryManager.Instance.OnRecipeCompleted += OnUpdate;
        VisualUpdate();
    }
    private void OnUpdate(object sender, System.EventArgs e)
    {
        VisualUpdate();
    }
    private void VisualUpdate()
    {
        foreach (Transform child in _container)
        {
            if (child == _template) continue;
            Destroy(child.gameObject);
        }
        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitRecipesList())
        {
            Transform template = Instantiate(_template, _container);
            template.gameObject.SetActive(true);
            template.GetComponent<DeliveryManagerSingleUI>().SetReciprSO(recipeSO);
        }
    }

}
