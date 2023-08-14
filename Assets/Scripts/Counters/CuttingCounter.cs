using System;
using UnityEngine;
using Unity.Netcode;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData() => OnAnyCut = null;
    [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSO;
    public enum State { Idle, Cutting, Cuted }
    private NetworkVariable<float> _cuttingProgress = new NetworkVariable<float>(0f);
    private int _cuttingProgressMax = 1;
    private KitchenObject _currentKitchenObject;
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private CuttingRecipeSO _cuttingRecipe;
    private KitchenObjectSO _outputKitchenObjectSO;
    public event EventHandler<bool> OnCut;
    public event EventHandler<IHasProgress.ProgressEventArgs> OnProgressChanged;
    public override void OnNetworkSpawn()
    {
        _cuttingProgress.OnValueChanged += OnCuttingProgressValueChanged;
        _state.OnValueChanged += OnStateChangedNetwork;
    }
    private void OnStateChangedNetwork(State prev, State next)
    {
        OnCut?.Invoke(this, _state.Value == State.Cutting ? true : false);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressEventArgs
        {
            ProgressNormalized = _state.Value == State.Cuted || _state.Value == State.Idle ? 0f : _cuttingProgress.Value
        });
    }
    private void OnCuttingProgressValueChanged(float prev, float next)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressEventArgs
        {
            ProgressNormalized = _cuttingProgress.Value / _cuttingProgressMax
        });

    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().KitchenObjectSO))
            {
                KitchenObjectSO kitchenObjectSO = player.GetKitchenObject().KitchenObjectSO;
                player.GetKitchenObject().SetKitchenObjParent(this);
                _cuttingRecipe = GetCuttingRecipeSOWithInput(kitchenObjectSO);
                _cuttingProgressMax = _cuttingRecipe.CutProgressMax;
                InteractLogicServerRpc(_cuttingProgressMax);
            }
        }
        else
        {
            if (_state.Value == State.Cutting) return;
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        SetIdleStateServerRpc();
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjParent(player);
                SetIdleStateServerRpc();
            }
        }
    }
    private void Update()
    {
        if (!IsServer) return;
        if (HasKitchenObject())
        {
            switch (_state.Value)
            {
                case State.Idle: break;
                case State.Cutting:
                    _cuttingProgress.Value += Time.deltaTime;
                    if (_cuttingProgress.Value >= _cuttingProgressMax)
                    {
                        KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().KitchenObjectSO);
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                        _state.Value = State.Cuted;
                    }
                    break;
                case State.Cuted: break;
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO recipe = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        return recipe != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        CuttingRecipeSO recipe = GetCuttingRecipeSOWithInput(input);
        if (recipe != null) return recipe.output;
        return null;
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (CuttingRecipeSO recipe in _cuttingRecipeSO)
            if (recipe.input == input) return recipe;
        return null;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc(int max)
    {
        _cuttingProgress.Value = 0f;
        _state.Value = State.Cutting;
        InteractLogicClientRpc(max);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetIdleStateServerRpc()
    {
        _state.Value = State.Idle;
    }
    [ClientRpc]
    private void InteractLogicClientRpc(int max)
    {
        _cuttingProgressMax = max;
    }
}
