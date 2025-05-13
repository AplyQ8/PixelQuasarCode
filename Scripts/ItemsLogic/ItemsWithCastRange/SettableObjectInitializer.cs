using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemsLogic.ItemsWithCastRange
{
    public class SettableObjectInitializer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private EffectApplier effectApplier;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private float lifeTime;
        [SerializeField] private Animator animationController;

        private float _lifeValue = 100;
        private float _lifePerSecond;
        private float _delayBetweenSubtraction = 0.2f;
        private static readonly int Die = Animator.StringToHash("Die");

        private void Awake()
        {
            healthBar.gameObject.SetActive(false);
            effectApplier.enabled = false;
        }

        public void InitializeSettableItem(List<ModifierData> modifiersToApply, float durability)
        {
            lifeTime = durability;
            healthBar.SetMaxHealth(_lifeValue);
            healthBar.SetHealth(_lifeValue);

            _lifePerSecond = _lifeValue / lifeTime;

            effectApplier.Initialize(modifiersToApply);
        }

        private IEnumerator LifeCycle()
        {
            
            var delay = new WaitForSeconds(_delayBetweenSubtraction);
            while (true)
            {
                yield return delay;

                var subtractedValue = _lifePerSecond * _delayBetweenSubtraction;
                _lifeValue = Mathf.Max(0, _lifeValue - subtractedValue);
                healthBar.SetHealth(_lifeValue);
                if (!(_lifeValue <= 0)) 
                    continue;
                effectApplier.StopApplyingEffect();
                particleSystem.Stop();
                particleSystem.Clear();
                animationController.SetTrigger(Die);
                break;

            }
        }
        
        public void EndAwake()
        {
            healthBar.gameObject.SetActive(true);
            effectApplier.StartApplyingEffect();
            StartCoroutine(LifeCycle());
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}