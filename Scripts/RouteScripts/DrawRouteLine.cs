using System;
using System.Collections.Generic;
using UnityEngine;

namespace RouteScripts
{
    public class DrawRouteLine : MonoBehaviour
    {
        [SerializeField] private GameObject route;
        private int _iterator = 0;

        public int Iterator
        {
            get => _iterator;
            private set => _iterator = value >= route.transform.childCount - 1 ? 0 : value;
        }

        private void OnDrawGizmosSelected()
        {
            if (route.transform.childCount <= 1) return;
            var amountOfPoints = route.transform.childCount - 1;
            //Debug.DrawLine(route.transform.GetChild(0).transform.position, route.transform.GetChild(1).transform.position);
            Debug.DrawLine(
                route.transform.GetChild(Iterator).transform.position,
                route.transform.GetChild(Iterator+1).transform.position, Color.magenta, 0.5f, false);
            Iterator++;
        }
    }
}