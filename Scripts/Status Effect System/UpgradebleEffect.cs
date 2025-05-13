using UnityEngine;

namespace Status_Effect_System
{
    public abstract class UpgradebleEffect : StatusEffectData
    {
        public abstract void Upgrade();
        public abstract void ResetUpgrades();
    }
}