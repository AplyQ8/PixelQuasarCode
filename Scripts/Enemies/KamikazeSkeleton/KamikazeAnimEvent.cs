using UnityEngine;

namespace Tests
{
    public class KamikazeAnimEvent : MonoBehaviour
    {
        [SerializeField] private KamikazeSkeletonScript skeletonScript;

        public void EndAwake()
        {
            skeletonScript.EndAwake();
        }
    }
}