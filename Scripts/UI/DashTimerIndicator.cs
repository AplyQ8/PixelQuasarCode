using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class DashTimerIndicator : MonoBehaviour
    {
        public static DashTimerIndicator Instance { get; private set; }

        [SerializeField] private Image uiIndicator;
        [SerializeField] private Image cooldownMask;
        [SerializeField] private TMP_Text textIndicator;
        [SerializeField] private float appearanceTime;
        [SerializeField] private float fadeDuration;

        private Timer _appearanceTimer;
        private Camera _mainCamera;
        private RectTransform _canvasRectTransform;
        private Coroutine _fadeOutCoroutine;
        private HeroStateHandler _heroStateHandler;

        private void Start()
        {
            // Ensure that there is only one instance of the singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            ToggleOff();

            _appearanceTimer = new Timer(appearanceTime);
            _appearanceTimer.OnTimerDone += OnAppearanceTimerDone;

            _heroStateHandler = GameObject.FindWithTag("Player").GetComponent<HeroStateHandler>();

            _heroStateHandler.DashState.OnCooldownClickEvent += ToggleOn;

            _mainCamera = Camera.main;
            _canvasRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _appearanceTimer.Tick();
        }

        private void ToggleOn()
        {
            _heroStateHandler.DashState.DashCooldownTimer
                .OnTimeRemaining += OnTimerChangeEvent;

            _appearanceTimer.StopTimer();

            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }

            Color originalColor = uiIndicator.color;
            uiIndicator.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

            uiIndicator.gameObject.SetActive(true);
            _appearanceTimer.StartTimer();

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z - _canvasRectTransform.position.z);
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            _canvasRectTransform.position = worldPosition;
        }

        private void ToggleOff()
        {
            uiIndicator.gameObject.SetActive(false);
        }

        private void OnAppearanceTimerDone()
        {
            _fadeOutCoroutine = StartCoroutine(FadeOutImage());
        }

        private void OnTimerChangeEvent(float time)
        {
            textIndicator.text = time.ToString("F1", CultureInfo.InvariantCulture);
            cooldownMask.fillAmount = time / _heroStateHandler.DashState.dashCooldown;
        }

        private IEnumerator FadeOutImage()
        {
            Color originalColor = uiIndicator.color;

            for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / fadeDuration;
                float alpha = Mathf.Lerp(originalColor.a, 0, normalizedTime);

                uiIndicator.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }

            uiIndicator.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            ToggleOff();
            
            _heroStateHandler.DashState.DashCooldownTimer
                .OnTimeRemaining -= OnTimerChangeEvent;
        }
    }
}
