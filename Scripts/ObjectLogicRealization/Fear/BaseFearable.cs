using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Fear
{
    public class BaseFearable : MonoBehaviour, IFearable
    {
        [SerializeField] private Transform objectOfFear;
        [SerializeField] private bool isFeared;
        
        
        public void ChangeFeared(bool isFear, Transform objectOfFear = null)
        {
            isFeared = isFear;
            if (isFeared)
                this.objectOfFear = objectOfFear;
        }

        public bool IsFeared() => isFeared;
        
        public Transform GetObjectOfFear() => objectOfFear;

    }
}