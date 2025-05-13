using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Status_Effect_System;
using UnityEngine;

public class RockTrap : MonoBehaviour
{
    [SerializeField] private GameObject route;
    [SerializeField] private List<Vector2> controlPoints;
    [SerializeField] private StatusEffectData collisionEffect;
    [SerializeField] private float speed;
    [SerializeField] private float stopTime;
    private IEnumerator _movingCoroutine;
    private IEnumerator _pointControlCoroutine;
    private int _iterator;

    private int Iterator
    {
        get => _iterator;
        set => _iterator = value >= controlPoints.Count ? 0 : value; 
    }

    void Start()
    {
        foreach (Transform point in route.transform)
        {
            controlPoints.Add((Vector2)point.position);
        }
        transform.position = controlPoints[0];
        _pointControlCoroutine = PointController();
        _iterator = 0;
        StartCoroutine(_pointControlCoroutine);
    }
    private IEnumerator MoveToNextPoint(Vector2 endPoint)
    {
        var delay = new WaitForEndOfFrame();
        float sqrRemainDistance = (transform.position - (Vector3)endPoint).sqrMagnitude;
        while (sqrRemainDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint,
                speed * Time.deltaTime);
            sqrRemainDistance = (transform.position - (Vector3)endPoint).sqrMagnitude;
            yield return delay;
        }
    }

    private IEnumerator PointController()
    {
        
        while (true)
        {
            Iterator++;
            _movingCoroutine = MoveToNextPoint(controlPoints[Iterator]);
            if(Iterator == controlPoints.Count - 1)
                controlPoints.Reverse();
            yield return StartCoroutine(_movingCoroutine);
            yield return new WaitForSeconds(stopTime);
        }
    }
}
