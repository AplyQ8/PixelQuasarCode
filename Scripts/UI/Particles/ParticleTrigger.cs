using System.Collections.Generic;
using UnityEngine;

namespace UI.Particles
{
    public class ParticleTrigger : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> particles;

        public void TriggerParticles()
        {
            foreach (var particle in particles)
            {
                particle.Play();
            }
        }
    }
}
