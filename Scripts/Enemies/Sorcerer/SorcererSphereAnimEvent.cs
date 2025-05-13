using UnityEngine;

namespace Tests
{
    public class SorcererSphereAnimEvent : MonoBehaviour
    {
        [SerializeField] private SorcererSphere sphereScript;

        public void EndExplosion()
        {
            sphereScript.EndExplosion();
        }
    }
}