using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DamageTypeManager
{
    private static DamageTypeManager _instance;
    public static DamageTypeManager GetInstance() => _instance ?? new DamageTypeManager();

    public enum DamageType
    {
        Default,
        Trap,
        MeleeAttack,
        Effect,
        Stun
    }
}

public sealed class AdrenalineModificatorManager
{
    private static AdrenalineModificatorManager _instance;
    public static AdrenalineModificatorManager GetInstance() => _instance ?? new AdrenalineModificatorManager();

    public enum AdrenalineModificator
    {
        HookHit,
        HookMess,
        Attack,
        Dash,
        SuperAttack,
        ReversedAttackLogic
    }
}
