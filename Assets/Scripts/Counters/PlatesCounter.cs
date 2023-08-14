using System;
using UnityEngine;
using Unity.Netcode;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    private float _spawnPlatesTimer;
    private float _spawnPlatesTimerMax = 4f;
    private int _platesCount;
    private int _platesCountMax = 4;
    private void Update()
    {
        if (!IsServer) return;
        _spawnPlatesTimer += Time.deltaTime;
        if (_spawnPlatesTimer > _spawnPlatesTimerMax)
        {
            _spawnPlatesTimer = 0f;
            if (_platesCount < _platesCountMax)
            {
                SpawnPlatesServerRpc();
            }
        }
    }
    [ServerRpc]
    private void SpawnPlatesServerRpc()
    {
        SpawnPlatesClientRpc();
    }
    [ClientRpc]
    private void SpawnPlatesClientRpc()
    {
        _platesCount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (_platesCount > 0)
            {
                KitchenObject.SpawnKitchenObject(_kitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        _platesCount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
