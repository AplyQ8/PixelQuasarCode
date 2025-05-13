using UnityEngine;

namespace Tests
{
    public class IncenserAnimEvent : MonoBehaviour
    {
        [SerializeField] private IncenserScript incenserScript;

        public void EndBuffPrep()
        {
            incenserScript.StopCast();
        }
    }
}