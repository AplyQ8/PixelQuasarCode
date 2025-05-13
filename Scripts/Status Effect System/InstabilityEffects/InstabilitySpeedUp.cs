using ObjectLogicRealization.CanHook;
using ObjectLogicRealization.Move;
using UnityEngine;

namespace Status_Effect_System.InstabilityEffects
{
    [CreateAssetMenu(fileName = "PercentageSpeedUp", menuName = "Status Effects/Instability/SpeedUp")]
    public class InstabilitySpeedUp : StatusEffectData
    {
        [Range(0, 1)][SerializeField] private float speedUpPercent;
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out BaseMove moveScript))
            {
                moveScript.SpeedUpByPercent(speedUpPercent);
            }
        }
        
        public override void EndEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out BaseMove moveScript))
            {
                moveScript.SlowDownByPercent(speedUpPercent);
            }
        }
    }
}