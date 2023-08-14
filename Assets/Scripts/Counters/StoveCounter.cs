using System;
using UnityEngine;
using Unity.Netcode;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<StoveStateEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.ProgressEventArgs> OnProgressChanged;
    public class StoveStateEventArgs : EventArgs
    {
        public State State;
    }
    public enum State { Idle, Frying, Fried, Burned }
    [SerializeField] private FryingRecipeSO[] _fryingRecipesSO;
    [SerializeField] private BurningRecipeSO[] _burningRecipesSO;
    private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO _fryingRecipe;
    private BurningRecipeSO _burningRecipe;
    public override void OnNetworkSpawn()
    {
        _fryingTimer.OnValueChanged += OnFryingTimerValueChanged;
        _burningTimer.OnValueChanged += OnBurningTimerValueChanged;
        _state.OnValueChanged += OnStateChangedNetwork;
    }
    private void OnStateChangedNetwork(State prev, State next)
    {
        OnStateChanged?.Invoke(this, new StoveStateEventArgs { State = _state.Value });
        if (_state.Value == State.Burned || _state.Value == State.Fried || _state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.ProgressEventArgs { ProgressNormalized = 0f });
        }
    }
    private void OnFryingTimerValueChanged(float prev, float next)
    {
        float max = _fryingRecipe != null ? _fryingRecipe.FryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressEventArgs
        {
            ProgressNormalized = (float)_fryingTimer.Value / max
        });
    }
    private void OnBurningTimerValueChanged(float prev, float next)
    {
        float max = _burningRecipe != null ? _burningRecipe.BurningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressEventArgs
        {
            ProgressNormalized = (float)_burningTimer.Value / max
        });
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().KitchenObjectSO))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                player.GetKitchenObject().SetKitchenObjParent(this);
                InteractLogicServerRpc(GameMultiplayer.Instance.GetIndexKitchenObjectSO(kitchenObject.KitchenObjectSO));
            }
        }
        else
        {
            if (_state.Value == State.Frying) return;
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO))
                    {
                        GameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
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
                case State.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                    if (_fryingTimer.Value > _fryingRecipe.FryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_fryingRecipe.output, this);
                        _state.Value = State.Fried;
                        _burningTimer.Value = 0f;
                        SetBurningObjectSOClientRpc(GameMultiplayer.Instance.GetIndexKitchenObjectSO(GetKitchenObject().KitchenObjectSO));
                        //_burningRecipe = GetBurningRecipeSOWithInput(GetKitchenObject().KitchenObjectSO);
                    }
                    break;
                case State.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    if (_burningTimer.Value > _burningRecipe.BurningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_burningRecipe.output, this);
                        _state.Value = State.Burned;
                    }
                    break;
                case State.Burned: break;
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO recipe = GetFryingRecipeSOWithInput(kitchenObjectSO);
        return recipe != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        FryingRecipeSO recipe = GetFryingRecipeSOWithInput(input);
        if (recipe != null) return recipe.output;
        return null;
    }
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (FryingRecipeSO recipe in _fryingRecipesSO)
            if (recipe.input == input) return recipe;
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO recipe in _burningRecipesSO)
            if (recipe.input == input) return recipe;
        return null;
    }
    public bool IsFried()
    {
        return _state.Value == State.Fried;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc(int index)
    {
        _fryingTimer.Value = 0f;
        _state.Value = State.Frying;
        SetFryingObjectSOClientRpc(index);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetIdleStateServerRpc()
    {
        _state.Value = State.Idle;
    }
    [ClientRpc]
    private void SetFryingObjectSOClientRpc(int index)
    {
        _fryingRecipe = GetFryingRecipeSOWithInput(GameMultiplayer.Instance.GetKitchenObjectSOByIndex(index));
    }
    [ClientRpc]
    private void SetBurningObjectSOClientRpc(int index)
    {
        _burningRecipe = GetBurningRecipeSOWithInput(GameMultiplayer.Instance.GetKitchenObjectSOByIndex(index));
    }
}
