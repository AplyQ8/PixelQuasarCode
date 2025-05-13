using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TransparableObject : MonoBehaviour
{
    public float transparencyLevel = 0.5f;   // Уровень прозрачности при нахождении игрока за объектом
    public float fadeSpeed = 2f;             // Скорость плавного перехода
    private SpriteRenderer spriteRenderer;   // Рендерер для спрайтов объекта
    private Color originalColor;             // Исходный цвет объекта
    private Coroutine fadeCoroutine;         // Текущая корутина для плавного изменения

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Сохраняем оригинальный цвет объекта
    }

    // Когда игрок заходит в триггер
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.transform.position.y < transform.position.y)
            return;
        // Если корутина уже запущена, останавливаем ее
        if (fadeCoroutine != null) 
        {
            StopCoroutine(fadeCoroutine);
        }
        // Запускаем корутину для плавного уменьшения прозрачности
        fadeCoroutine = StartCoroutine(FadeToTransparency(transparencyLevel));
    }

    // Когда игрок выходит из триггера
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Если корутина уже запущена, останавливаем ее
        if (fadeCoroutine != null) 
        {
            StopCoroutine(fadeCoroutine);
        }
        // Запускаем корутину для плавного восстановления прозрачности
        fadeCoroutine = StartCoroutine(FadeToTransparency(1f)); // Полностью видимый
    }

    // Корутина для плавного изменения прозрачности
    IEnumerator FadeToTransparency(float targetAlpha)
    {
        // Получаем текущий альфа-канал
        float currentAlpha = spriteRenderer.color.a;

        // Плавно изменяем альфа-канал от текущего значения к целевому
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            Color newColor = spriteRenderer.color;
            newColor.a = currentAlpha;
            spriteRenderer.color = newColor;

            yield return null; // Ждем следующий кадр
        }

        // Обнуляем корутину после завершения
        fadeCoroutine = null;
    }
}
