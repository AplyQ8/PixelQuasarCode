using UnityEngine;

namespace Main_hero.Sign_Effects.SignEffects
{
    public abstract class SignEffect : ScriptableObject
    {
        public bool IsTemporary { get; }
        public float Duration { get; }
        public abstract void ApplyEffect(GameObject target);
        public abstract void RemoveEffect(GameObject target);
    }
}
