using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasSetCamera : MonoBehaviour
{
    private void Start() {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
