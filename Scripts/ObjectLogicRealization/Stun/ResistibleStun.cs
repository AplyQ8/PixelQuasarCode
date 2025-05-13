using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Stun
{
    [RequireComponent(typeof(IResistible))]
    public class ResistibleStun : MonoBehaviour, IStunable
    {
        public event Action<float> OnGetStunnedEvent;
        private IResistible _resistanceScript;

        private void Start()
        {
            _resistanceScript = GetComponent<IResistible>();
        }
        public void GetStunned(float duration)
        {
            duration *= (1 - _resistanceScript.GetResistance(DamageTypeManager.DamageType.Stun));
            OnGetStunnedEvent?.Invoke(duration);
        }
        
    }
}