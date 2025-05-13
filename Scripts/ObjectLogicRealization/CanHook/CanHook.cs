using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.CanHook
{
    public class CanHook : MonoBehaviour, ICanHook
    {
        [SerializeField] private float speedWhileHooking;
        
        public void SpeedUpByPercent(float value)
        {
            if (value < 0) return;
            speedWhileHooking += speedWhileHooking * value;
        }

        public void SlowDownByPercent(float value)
        {
            if (value < 0) return;
            speedWhileHooking -= speedWhileHooking * value;
        }

        public float GetMoveSpeed() => speedWhileHooking;

    }
}