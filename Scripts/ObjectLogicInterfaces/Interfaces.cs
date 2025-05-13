using System;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using Main_hero.HookScripts.HookStrategies;
using QuestScripts;
using UnityEngine;

namespace ObjectLogicInterfaces
{
    public interface IDamageable
    {
        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;

        public event Action OnDeathEvent;
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType);
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition);
    }

    public interface IHealable
    {
        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;

        public event Action OnDeathEvent;
        public void TakeHeal(float value);
    }

    public interface IResistible
    {
        public float GetResistance(DamageTypeManager.DamageType damageType);
        public void ApplyResistance(float value, DamageTypeManager.DamageType damageType);
        
        
    }

    public interface IBloodContent
    {
        public event Action<float>
            OnBloodValueChangeEvent,
            OnBloodBoundaryChange;
        public void AddBlood(float value);
        public void SubtractBlood(float value);
        public void FulFill();
        public void ExpandBloodCapacity(float value);
        public void ReduceBloodCapacity(float value);
        public void IncreaseBloodConsumption(float value);
        public void ReduceBloodConsumption(float value);
        public float GetCurrentBloodValue();
        public float GetMaxBloodValue();
    }

    public interface IAdrenalineContent
    {
        public event Action<float>
            OnAdrenalineValueChange,
            OnAdrenalineBoundaryChange;
        public void ChangeAdrenaline(AdrenalineModificatorManager.AdrenalineModificator adrenalineModificator);
        public void ExpandAdrenalineBoundary(float value);
        public void DecreaseAdrenalineBoundary(float value);
        public void IncreaseAdrenalineReduction(float value);
        public void DecreaseAdrenalineReduction(float value);
        public float GetCurrentAdrenalineValue();
        public float GetMaxAdrenalineBoundary();
    }

    public interface IInvisible
    {
        public void MakeVisible();
        public void MakeInvisible();
        public bool IsVisible();
    }

    public interface ICanAttack
    {
        public float GetCurrentAttack();
        public float GetMaxAttackBoundary();
    }

    public interface IEvasionable
    {
        public bool AvoidedDamage();
        public void RecalculateEvasionChance(float value);
    }

    public interface IMovable
    {
        public void SetSpeed(float value);
        public void SpeedUp(float value);
        public void SlowDown(float value);
        public void SpeedUpByPercent(float value);
        public void SlowDownByPercent(float value);
        public float GetCurrentMoveSpeed();
        public Vector3 GetPivotPosition();
    }

    public interface IFearable
    {
        public void ChangeFeared(bool isFear, Transform objectOfFear);
        public bool IsFeared();
        public Transform GetObjectOfFear();
    }

    public interface IStunable
    {
        public event Action<float> OnGetStunnedEvent;
        public void GetStunned(float duration);
    }

    public interface IPassiveAbilityHolder
    {
        public void ExpandPassiveAbilityPool(int numberOfSlots);
        public void ReducePassiveAbilityPool(int numberOfSlots);
    }

    public interface IHookable
    {
        public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler);
        public void EndPulledByHook();
        public bool IsIntangible();
    }

    public interface IForceHookBehaviourChanger
    {
        public HookBehaviour HookBehaviour { get; set; }
        public void SetHookBehaviour(HookStrategyHandler hookStrategyHandler);
    }

    public interface IObstructed
    {
        public Vector3 GetPivotPoint();
    }

    public interface IColorChangeable
    {
        public void ChangeColor(Color color);
        public void ChangeColorTemporarily(Color color, float duration);
        public void ChangeTransparency(float value);
        public void MakeOpaque();
    }

    public interface IHealth
    {
        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;
        public float GetCurrentHealth();
        public float GetMaxHealthPoints();
        public void IncreaseHealthPoints(float value);
        public void DecreaseHealthPoints(float value);
    }

    public interface ICanHook
    {
        public float GetMoveSpeed();
    }
    public interface IForceCameraShake
    {
        public float ShakeMagnitude { get; }
        public float ShakeDuration { get; }
        public void ShakeCamera();
    }

    public interface ICanFall
    {
        public void DeathFromFalling();
        public bool CanFall();
    }

    public interface IMouseHoverable
    {
        public bool IsMouseOver { get; }
    }

    public interface IDistanceCheckable
    {
        // Свойство, которое указывает, можно ли взаимодействовать с объектом
        bool CanBeInteractedWith { get; }

        // Метод, возвращающий расстояние от игрока до объекта
        float DistanceToPlayer();
    }

    public interface IQuestTrigger
    {
        public QuestSo Quest { get; }
        void ActivateTrigger(HeroInventory_Quest questInventory);
    }
    
}

