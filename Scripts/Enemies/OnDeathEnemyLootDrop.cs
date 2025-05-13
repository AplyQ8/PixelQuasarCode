using System;
using System.Collections;
using System.Collections.Generic;
using ItemDrop;
using PickableObjects.PickUpSystem;
using UnityEngine;
using Utilities;

public class OnDeathEnemyLootDrop : MonoBehaviour
{
    [SerializeField] private EnemyScript enemy;
    [SerializeField] private Transform dropPoint;
    [Header("Drop containers")]
    [SerializeField] private LootBagScript lootBag;
    [SerializeField] private PickableItem lootItem;
    [Header("Drop list")]
    [SerializeField] private List<LootItem> droppableItemLoot = new List<LootItem>();

    private void Awake()
    {
        var root = transform.parent;
        enemy = root.GetComponent<EnemyScript>();
        dropPoint = root.GetComponentInChildren<SpriteRenderer>().transform;
        enemy.OnDieEvent += DropLoot;
    }

    private void DropLoot()
    {
        var lootList = InitializeLootList();
        switch (lootList.Count)
        {
            case 0:
                return;
            case 1:
                InitializePickableObject(lootList[0].Item, lootList[0].Quantity);
                break;
            default:
                InitializeDropBag(lootList);
                break;
        }
    }

    private List<LootBagItem> InitializeLootList()
    {
        var lootList = new List<LootBagItem>();
        foreach (var lootItem in droppableItemLoot)
        {
            if(RandomGenerator.Instance.IsInRange(lootItem.DropChance))
                lootList.Add(
                    new LootBagItem(
                        lootItem.Item, 
                        RandomGenerator.Instance.RandomValueInRange(lootItem.Range.From, lootItem.Range.To)));
        }
        return lootList;
    }

    private void InitializeDropBag(List<LootBagItem> lootList)
    {
        var newLootBag = Instantiate(lootBag, dropPoint.position, Quaternion.identity);
        newLootBag.Initialize(lootList);
    }

    private void InitializePickableObject(ItemSO item, int quantity)
    {
        var pickableObject = Instantiate(lootItem, dropPoint.position, Quaternion.identity);
        pickableObject.Initialize(item, quantity);
    }
}

[Serializable]
public struct LootItem
{
    [field: SerializeField] public ItemSO Item { get; private set; }
    [field: Range(0, 1)] [field: SerializeField] public float DropChance { get; private set; }
    [field: SerializeField] public LootItemDropRange Range { get; private set; }
}

[Serializable]
public struct LootItemDropRange
{
    [field: SerializeField] public int From { get; private set; }
    [field: SerializeField] public int To { get; private set; }
}