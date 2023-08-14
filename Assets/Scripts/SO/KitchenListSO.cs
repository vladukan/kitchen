using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenListSO : ScriptableObject
{
   [SerializeField] private List<KitchenObjectSO> _kitchenObjectSoList;
   public List<KitchenObjectSO> KitchenObjectSoList => _kitchenObjectSoList;
}
