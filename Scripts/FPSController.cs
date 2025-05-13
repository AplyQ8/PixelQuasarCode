using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class FPSController : MonoBehaviour
    {
        [SerializeField] private Text text;
        private float _dTime;
        void Awake()
        {
            StartCoroutine(FPS());
        }
        private IEnumerator FPS()
        {
            var delay = new WaitForSeconds(2f);
            while (true)
            {
                _dTime += (Time.deltaTime - _dTime) * 0.1f;
                float fps = 1.0f / _dTime;
                text.text = Mathf.Ceil(fps).ToString(CultureInfo.InvariantCulture);
                yield return delay;
            }
        }
    }
}