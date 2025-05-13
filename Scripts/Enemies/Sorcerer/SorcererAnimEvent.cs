using UnityEngine;

namespace Tests
{
    public class SorcererAnimEvent : MonoBehaviour
    {
        [SerializeField] private Sorcerer sorcererScript;

        public void EndAfterAttack()
        {
            sorcererScript.EndAfterAttack();
        }
    }
}