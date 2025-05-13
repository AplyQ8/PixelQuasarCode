using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBagInitializer : MonoBehaviour
{
    [SerializeField] private DropBag bagInventory;

    public void Initialize(List<DropBagItem> listOfLoot)
    {
        bagInventory.InitializeInventory(listOfLoot);
    }
}
