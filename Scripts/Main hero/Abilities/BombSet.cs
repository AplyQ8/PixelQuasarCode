using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BombSet")]
public class BombSet : Ability
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int maxAmountOfBombs;
    [SerializeField] private List<GameObject> bombs;
    public override void Activate(GameObject hero, Vector2 position)
    {
        position = Camera.main.ScreenToWorldPoint(position);
        if (Vector2.Distance(hero.transform.position, position) > usabilityDistance)
        {
            var heroPos = (Vector2)hero.transform.position;
            var facing = position - heroPos;
            var destination = heroPos + facing.normalized * usabilityDistance;
            var newBomb = Instantiate(bombPrefab, destination, Quaternion.identity);
            SetBomb(newBomb);
            hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
            return;
        }
        var bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        SetBomb(bomb);
        hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
    }

    private void SetBomb(GameObject bomb)
    {
        if (bombs.Count == maxAmountOfBombs)
        {
            Destroy(bombs[0]);
            bombs.RemoveAt(0);
        }
        bombs.Add(bomb);
    }
}
