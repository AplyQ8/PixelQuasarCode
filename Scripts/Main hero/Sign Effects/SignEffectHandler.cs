using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main_hero.Sign_Effects.SignEffects;
using UnityEngine;

namespace Main_hero.Sign_Effects
{
    public class SignEffectHandler : MonoBehaviour
    {
        [SerializeField] private int maxSlots = 1;
        private Queue<(SignEffect effect, Coroutine coroutine)> _activeEffects = new Queue<(SignEffect, Coroutine)>();
        private GameObject _target;
        [SerializeField] private SignEffect test;

        private void Awake()
        {
            _target = transform.root.gameObject;
            ApplyEffect(test);
        }
        
        public void ApplyEffect(SignEffect effect)
        {
            if (_activeEffects.Count >= maxSlots)
            {
                RemoveOldestEffect();
            }

            effect.ApplyEffect(_target);

            Coroutine coroutine = null;
            if (effect.IsTemporary)
            {
                coroutine = StartCoroutine(RemoveEffectAfterDelay(effect, effect.Duration));
            }

            _activeEffects.Enqueue((effect, coroutine));
        }

        private IEnumerator RemoveEffectAfterDelay(SignEffect effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            RemoveEffect(effect);
        }

        public void RemoveEffect(SignEffect effect)
        {
            foreach (var (storedEffect, coroutine) in _activeEffects)
            {
                if (storedEffect == effect)
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    storedEffect.RemoveEffect(_target);
                    _activeEffects = new Queue<(SignEffect, Coroutine)>(_activeEffects.Where(e => e.effect != effect));
                    break;
                }
            }
        }

        private void RemoveOldestEffect()
        {
            if (_activeEffects.Count <= 0) return;
            var (oldEffect, coroutine) = _activeEffects.Dequeue();
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            oldEffect.RemoveEffect(_target);
        }
    }
}
