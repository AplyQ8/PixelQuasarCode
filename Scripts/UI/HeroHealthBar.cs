using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroHealthBar : HealthBar
    {
        [SerializeField] private Slider topDownSlider;
        [SerializeField] private Slider downTopSlider;
        [SerializeField] private float maxHealth;
        private float _currentHealth;
        
        public override void SetHealth(float value)
        {
            topDownSlider.value = (value / maxHealth);
            downTopSlider.value = topDownSlider.value;
            
            _currentHealth = value;
            
        }

        public override void SetMaxHealth(float value)
        {
            maxHealth = value;
            topDownSlider.value = _currentHealth / 2 * maxHealth;
            downTopSlider.value = topDownSlider.value;
        }
    }
}
