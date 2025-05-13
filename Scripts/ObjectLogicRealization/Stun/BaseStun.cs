using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Stun
{
    public class BaseStun : MonoBehaviour, IStunable
    {
        public event Action<float> OnGetStunnedEvent;

        public void GetStunned(float duration)
        {
            OnGetStunnedEvent?.Invoke(duration);
        }
        
    }
}