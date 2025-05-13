using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Abilities/FireSkin")]
public class PA_FireSkin : BasePassiveAbilitySO
{
    [SerializeField] private int cooldown;
    [SerializeField] private int activeTime;
    [SerializeField] private PAbilityStates state;
    private enum PAbilityStates
    {
        Ready,
        Active,
        Cooldown
    }
    public override void ApplyEffect(HeroComponents hero)
    {
        state = PAbilityStates.Active;
        hero.Collider.OnCollision += OnCollision;
    }
    private void OnCollision(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Enemy"))
            return;
        if (col.TryGetComponent(out IEffectable effectable))
        {
            effectable.ApplyEffect(effect);
        }
        // switch (state)
        // {
        //     case PAbilityStates.Ready:
        //         StartCoroutine(ActiveStage());
        //         col.gameObject.GetComponent<IEffectable>().ApplyEffect(effect);
        //         state = PAbilityStates.Active;
        //         break;
        //     case PAbilityStates.Active:
        //         col.gameObject.GetComponent<IEffectable>().ApplyEffect(effect);
        //         break;
        // }
    }

    public override void RemoveEffect(HeroComponents hero)
    {
        hero.Collider.OnCollision -= OnCollision;
    }

    // private IEnumerator ActiveStage()
    // {
    //     yield return new WaitForSeconds(activeTime);
    //     state = PAbilityStates.Cooldown;
    //     StartCoroutine(CooldownStage());
    // }
    //
    // private IEnumerator CooldownStage()
    // {
    //     yield return new WaitForSeconds(cooldown);
    //     state = PAbilityStates.Ready;
    // }
}
