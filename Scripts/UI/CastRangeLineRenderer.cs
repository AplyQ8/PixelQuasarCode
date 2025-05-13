using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class CastRangeLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject indicator;
        [SerializeField] private float distance;
        [SerializeField] private Transform startingPoint;
        private IEnumerator _drawLineCoroutine;

        private void Awake()
        {
            _drawLineCoroutine = DrawLine();
        }

        public void Activate(float castDistance, Transform startPoint)
        {
            gameObject.SetActive(true);
            distance = castDistance;
            startingPoint = startPoint;
            lineRenderer.SetPosition(0, startingPoint.position);
            lineRenderer.SetPosition(1, startingPoint.position);
            StartCoroutine(_drawLineCoroutine);
            
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            try
            {
                StopCoroutine(_drawLineCoroutine);
            }
            catch (NullReferenceException)
            {
                //Routine has not been assigned yet
            }
        }
        
        private IEnumerator DrawLine()
        {
            var delay = new WaitForEndOfFrame();
            while (true)
            {
                //Draw
                lineRenderer.SetPosition(0, startingPoint.position);
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                // Определение конечной точки линии в зависимости от расстояния до мыши
                float distanceToMouse = Vector3.Distance(startingPoint.position, mousePosition);
                if (distanceToMouse <= distance)
                {
                    // Если расстояние до мыши меньше или равно максимальному расстоянию,
                    // то конечная точка линии будет на позиции мыши
                    lineRenderer.SetPosition(1, mousePosition);
                }
                else
                {
                    // Если расстояние до мыши больше максимального расстояния,
                    // то конечная точка линии будет на определенном расстоянии от объекта по направлению мыши
                    Vector3 directionToMouse = (mousePosition - startingPoint.position).normalized;
                    Vector3 endpoint = startingPoint.position + directionToMouse * distance;
                    lineRenderer.SetPosition(1, endpoint);
                }
                yield return delay;
            }
        }
    }
}