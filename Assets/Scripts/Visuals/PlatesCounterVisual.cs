using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter _platesCounter;
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _platePrefab;
    private List<GameObject> _platesList = new List<GameObject>();
    private void Start()
    {
        _platesCounter.OnPlateSpawned += OnPlateSpawned;
        _platesCounter.OnPlateRemoved += OnPlateRemoved;
    }
    private void OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateTransform = Instantiate(_platePrefab, _topPoint);
        float plateOffsetY = .1f;
        plateTransform.localPosition = new Vector3(0f, plateOffsetY * _platesList.Count, 0f);
        _platesList.Add(plateTransform.gameObject);
    }
    private void OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject plate = _platesList[_platesList.Count - 1];
        _platesList.Remove(plate);
        Destroy(plate);
    }
}
