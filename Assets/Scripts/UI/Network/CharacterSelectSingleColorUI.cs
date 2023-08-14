using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSingleColorUI : MonoBehaviour
{
    [SerializeField] private int _colorId;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _selected;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.ChangePlayerColor(_colorId);
        });
    }
    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataListChanged += OnPlayerDataListChanged;
        _image.color = GameMultiplayer.Instance.GetPlayerColor(_colorId);
        UpdateIsSelected();
    }
    private void OnPlayerDataListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }
    private void UpdateIsSelected()
    {
        if (GameMultiplayer.Instance.GetPlayerData(_colorId).ColorId == _colorId) _selected.SetActive(true);
        else _selected.SetActive(false);
    }
    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataListChanged -= OnPlayerDataListChanged;
    }
}
