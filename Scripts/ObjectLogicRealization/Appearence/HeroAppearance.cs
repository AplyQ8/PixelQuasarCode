using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.HeroStats
{
    [RequireComponent(typeof(IColorChangeable))]
    public class HeroAppearance : MonoBehaviour, IInvisible
    {
        [field: SerializeField] public bool IsInvisible { get; private set; }
        [field: SerializeField] public float TransparencyValue { get; private set; }
        private IColorChangeable _colorChanger;

        private void Awake()
        {
            _colorChanger = GetComponent<IColorChangeable>();
        }

        public void MakeVisible()
        {
            IsInvisible = false;
            _colorChanger.MakeOpaque();
        }

        public void MakeInvisible()
        {
            IsInvisible = true;
            _colorChanger.ChangeTransparency(TransparencyValue);
        }

        public bool IsVisible() => !IsInvisible;

    }
}