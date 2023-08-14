using UnityEngine;

public class CuttingContainerVisual : MonoBehaviour
{
    private const string CUT = "Cutting";
    [SerializeField] private CuttingCounter _cuttingCounter;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _cuttingCounter.OnCut += OnCut;
    }
    private void OnCut(object sender, bool isCut)
    {
        _animator.SetBool(CUT, isCut);
    }
}
