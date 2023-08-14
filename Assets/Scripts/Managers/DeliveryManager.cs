using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipesListSO _recipesList;
    private List<RecipeSO> _waitRecipesList = new List<RecipeSO>();
    private float _timeToWait = 4f;
    private float _timeToWaitMax = 4f;
    private int _waitRecipesMax = 4;
    private int _successRecipesAmount;
    public int SuccessRecipesAmount => _successRecipesAmount;
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!IsServer) return;
        if (GameManager.Instance.IsWaitStartGame) return;
        _timeToWait -= Time.deltaTime;
        if (_timeToWait <= 0)
        {
            _timeToWait = _timeToWaitMax;
            if (_waitRecipesList.Count < _waitRecipesMax)
            {
                int index = UnityEngine.Random.Range(0, _recipesList.RecipesSOList.Count);
                SpawnNewRecipeClientRpc(index);
            }
        }
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitRecipesList.Count; i++)
        {
            RecipeSO recipe = _waitRecipesList[i];
            if (recipe.kitchenObjects.Count == plateKitchenObject.GetKitchenObjects().Count)
            {
                bool isMatch = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in recipe.kitchenObjects)
                {
                    bool isFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjects())
                    {
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (!isFound) isMatch = false;
                }
                if (isMatch)
                {
                    DeliverCorrectRecipeServerRpc(i);
                    plateKitchenObject.DestroySelf();
                    return;
                }
            }
        }
        DeliverInCorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int index) => DeliverCorrectRecipeClientRpc(index);
    [ServerRpc(RequireOwnership = false)]
    private void DeliverInCorrectRecipeServerRpc() => DeliverIncorrectRecipeClientRpc();
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int index)
    {
        _successRecipesAmount++;
        _waitRecipesList.RemoveAt(index);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    [ClientRpc]
    private void SpawnNewRecipeClientRpc(int index)
    {
        RecipeSO waitRecipe = _recipesList.RecipesSOList[index];
        _waitRecipesList.Add(waitRecipe);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }
    public List<RecipeSO> GetWaitRecipesList()
    {
        return _waitRecipesList;
    }
}
