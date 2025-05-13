using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraFlow : MonoBehaviour
{
    [SerializeField] public Transform objectToFollow;
    [SerializeField] private float distanceFromObject;
    private IMovable _moveScript;
    
    [SerializeField] private CameraShakeInfo shakeInfo;
    private Vector3 _originalPosition;
    private bool _isShaking = false;
    private List<IDamageable> _visibleDamageableObjects = new List<IDamageable>();
    private CameraStates _currentState;
    private HeroAdrenaline _adrenaline;

    private enum CameraStates
    {
        Normal,
        Instability
    }

    void Awake()
    {
        var _object = GameObject.Find("Pudge");
        _adrenaline = _object.GetComponent<HeroAdrenaline>();
        objectToFollow = _object.transform;
        var position = objectToFollow.position;
        transform.position = new Vector3()
        {
            x = position.x,
            y = position.y,
            z = position.z - distanceFromObject
        };
        _moveScript = objectToFollow.GetComponent<IMovable>();
        _visibleDamageableObjects.Clear();
        _currentState = CameraStates.Normal;
        _adrenaline.OnInstabilityEnter += OnInstabilityEnter;
        _adrenaline.OnInstabilityExit += OnInstabilityExit;
    }

    void LateUpdate()
    {
        if (_isShaking) return; // Если тряски нет, следуем за объектом
        var objPos = objectToFollow.position;
        Vector3 target = new Vector3()
        {
            x = objPos.x,
            y = objPos.y,
            z = objPos.z - distanceFromObject
        };
        Vector3 pos = Vector3.Lerp(transform.position, target, _moveScript.GetCurrentMoveSpeed() * Time.deltaTime);
        transform.position = pos;
        _originalPosition = transform.position; // Сохраняем текущую позицию как исходную
    }

    private void TriggerShake(IHealth health)
    {
        if (_isShaking || _currentState is CameraStates.Normal) return;

        // Вычисляем тряску в зависимости от здоровья
        float healthFactor = health.GetMaxHealthPoints();
        float calculatedMagnitude = Mathf.Lerp(shakeInfo.MinMagnitude, shakeInfo.MaxMagnitude, healthFactor);
        StartCoroutine(ShakeCoroutine(calculatedMagnitude, shakeInfo.Duration));
    }

    public void ForceShake(float magnitude, float duration)
    {
        StartCoroutine(ShakeCoroutine(magnitude, duration));
    }

    private IEnumerator ShakeCoroutine(float magnitude, float duration)
    {
        _isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude),
                0); // Тряска только по X и Y
            transform.position = _originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _originalPosition; // Возвращаем камеру в исходную позицию
        _isShaking = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IDamageable damageable) && !col.CompareTag("Player"))
        {
            if (_visibleDamageableObjects.Contains(damageable)) return;
            _visibleDamageableObjects.Add(damageable);

            // Подписываемся на событие смерти и передаем здоровье
            if (col.TryGetComponent(out IHealth health))
            {
                damageable.OnDeathEvent += () => TriggerShake(health);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent(out IDamageable damageable) && !col.CompareTag("Player"))
        {
            if (!_visibleDamageableObjects.Contains(damageable)) return;
            _visibleDamageableObjects.Remove(damageable);

            // Отписываемся от событий
            if (col.TryGetComponent(out IHealth health))
            {
                damageable.OnDeathEvent -= () => TriggerShake(health);
            }
        }
    }

    private void OnInstabilityEnter()
    {
        _currentState = CameraStates.Instability;
    }
    private void OnInstabilityExit()
    {
        _currentState = CameraStates.Normal;
    }

    private void OnDisable()
    {
        foreach (var visibleDamageableObject in _visibleDamageableObjects)
        {
            if (visibleDamageableObject is MonoBehaviour monoBehaviour)
            {
                if (monoBehaviour.TryGetComponent(out IHealth health))
                {
                    visibleDamageableObject.OnDeathEvent -= () => TriggerShake(health);
                }
            }
        }
        _adrenaline.OnInstabilityEnter -= OnInstabilityEnter;
        _adrenaline.OnInstabilityExit -= OnInstabilityExit;
    }
}

[Serializable]
public class CameraShakeInfo
{
    [field: Header("Magnitude")]
    [field: SerializeField] public float MinMagnitude { get; private set; }
    [field: SerializeField] public float MaxMagnitude { get; private set; }
    [field: Header("Duration")]
    [field: SerializeField] public float Duration { get; private set; }
    
}
