using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRangeDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer circleRenderer;
    [SerializeField] private int segments;
    private Vector3[] _points;
    
    void Start()
    {
        circleRenderer.positionCount = segments + 1;
        circleRenderer.useWorldSpace = true;
        _points = new Vector3[segments + 1];
        DrawCircle(5);
    }
    public void DrawCircle(float radius)
    {
        float x;
        float y;

        float angle = 20f;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

            //circleRenderer.SetPosition (i,new Vector3(x,y,0) );
            //_points[i] = centerTransform.TransformPoint(new Vector3(x, height, y));

            angle += (360f / segments);
        }
    }
}
