using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Dismember")]
public class Dismember : Ability
{
    [SerializeField] private StatusEffectData effect;
    public override void Activate(GameObject hero, GameObject targetedObject)
    {
        IHealth enemyCharsScript;
        if (!targetedObject.TryGetComponent(out IHealth health))
            return;
        else
        {
            enemyCharsScript = health;
        }
        
        if (enemyCharsScript.GetCurrentHealth() <= enemyCharsScript.GetMaxHealthPoints() * 0.2)
        {
            //Нельзя уничтожать объект
            //Destroy(targetedObject);
            hero.GetComponent<IEffectable>().ApplyEffect(effect);
            hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
            return;
        }
        hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
        if (targetedObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, DamageTypeManager.DamageType.Default);
        }
    }
}
