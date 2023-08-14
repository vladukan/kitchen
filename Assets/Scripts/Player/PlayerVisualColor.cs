using UnityEngine;

public class PlayerVisualColor : MonoBehaviour
{
    [SerializeField] private MeshRenderer _head;
    [SerializeField] private MeshRenderer _body;
    private Material _material;
    private void Awake()
    {
        _material = new Material(_head.material);
        _head.material = _material;
        _body.material = _material;
    }
    private void Start() {
        
    }
    public void SetPlayerColor(Color color)
    {
        _material.color = color;
    }
}
