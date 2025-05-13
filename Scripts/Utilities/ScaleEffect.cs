using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    public float maxScale = 1.2f;
    public float minScale = 0.8f;
    public float scaleSpeed = 1.0f;

    private bool _isScalingUp = true;

    void Update()
    {
        // Увеличение или уменьшение масштаба в зависимости от направления
        if (_isScalingUp)
        {
            transform.localScale += Vector3.one * (scaleSpeed * Time.deltaTime);

            // Если достигнут максимальный масштаб, начните уменьшаться
            if (transform.localScale.x >= maxScale)
            {
                _isScalingUp = false;
            }
        }
        else
        {
            transform.localScale -= Vector3.one * (scaleSpeed * Time.deltaTime);

            // Если достигнут минимальный масштаб, начните увеличиваться
            if (transform.localScale.x <= minScale)
            {
                _isScalingUp = true;
            }
        }
    }
}
