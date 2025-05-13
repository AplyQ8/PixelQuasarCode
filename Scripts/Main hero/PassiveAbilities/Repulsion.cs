using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PassiveAbilities/Repulsion")]
public class Repulsion 
{
    [SerializeField] private PAbilityStates state;
    [SerializeField] private float cooldown;
    [SerializeField] private float kbForce;
    [SerializeField] private float totalTime;
    [SerializeField] private GameObject hero;
    private enum PAbilityStates
    {
        Ready,
        Cooldown
    }
    public void ApplyEffect(GameObject hero)
    {
        this.hero = hero;
        kbForce = 7;
        totalTime = 0.5f;
        cooldown = 1;
        state = PAbilityStates.Ready;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        
        // if (!col.gameObject.CompareTag("Enemy"))
        //     return;
        // switch (state)
        // {
        //     case PAbilityStates.Ready:
        //         var enemy = col.gameObject.GetComponent<EnemyScript>();
        //         var kbDir = (Vector2)(col.transform.position - transform.position);
        //         enemy.SetBounceCharacteristics(kbForce, totalTime, kbDir);
        //         state = PAbilityStates.Cooldown;
        //         StartCoroutine(CooldownStage());
        //         break;
        // }
    }
    
    private IEnumerator CooldownStage()
    {
        yield return new WaitForSeconds(cooldown);
        state = PAbilityStates.Ready;
    }
    
}
