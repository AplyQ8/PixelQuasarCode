using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycastTransparency : MonoBehaviour
{
    [SerializeField] private Transform player; // Ссылка на игрока
    [SerializeField] private LayerMask transparentLayer; // Слой объектов, которые могут становиться прозрачными
    [SerializeField] private float transparentAlpha = 0.5f; // Прозрачность, когда игрок находится за объектом
    [SerializeField] private float normalAlpha = 1.0f; // Прозрачность в нормальном состоянии
    public float transparencyAmount = 0.5f; // Уровень прозрачности
    public float maxDistance = 10f; // Максимальная дистанция для raycast
    private List<TransparableObject> transparentObjects = new List<TransparableObject>();
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    
    void Update()
    {
        HandleTransparency();
    }

    private void HandleTransparency()
    {
        // // Сбрасываем прозрачность всех объектов, которые были прозрачными
        // foreach (var obj in transparentObjects)
        // {
        //     obj.ResetTransparency();
        // }
        // transparentObjects.Clear();
        //
        // // Определяем направление от игрока к камере
        // Vector2 direction = Camera.main.transform.position - player.position;
        //
        // // Используем Physics2D.RaycastAll для получения всех объектов на пути
        // RaycastHit2D[] hits = Physics2D.RaycastAll(player.position, direction, direction.magnitude, transparentLayer);
        //
        // // Проходим по всем объектам, попавшим в луч
        // foreach (RaycastHit2D hit in hits)
        // {
        //     if (!hit.collider.TryGetComponent(out TransparableObject transparableObject)) continue;
        //     // Проверяем, находится ли игрок выше объекта по оси Y
        //     if (player.position.y > hit.collider.transform.position.y)
        //     {
        //         transparableObject.SetTransparency(transparencyAmount); // Делаем объект прозрачным
        //         transparentObjects.Add(transparableObject); // Добавляем объект в список прозрачных объектов
        //     }
        //     else
        //     {
        //         transparableObject.ResetTransparency(); // Возвращаем нормальную прозрачность
        //     }
        // }
    }
}
