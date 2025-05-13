using UnityEngine;

namespace ItemDrop
{
    public class HeroLootBagScript : LootBagScript
    {
        public bool IsActive { get; private set; }

        private protected override void Awake()
        {
            base.Awake();
            IsActive = true;
        }

        private protected override void DestroyBag()
        {
            base.DestroyBag();
            IsActive = false;
        }
    }
}