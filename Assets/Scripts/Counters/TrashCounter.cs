using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyTrashed;
    new public static void ResetStaticData() => OnAnyTrashed = null;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            GameMultiplayer.Instance.DestroyKitchenObject(player.GetKitchenObject());
            //player.GetKitchenObject().DestroySelf();
            OnAnyTrashed?.Invoke(this, EventArgs.Empty);
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
        OnAnyTrashed?.Invoke(this, EventArgs.Empty);
    }
}
