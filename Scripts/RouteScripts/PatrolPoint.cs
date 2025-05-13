using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RouteScripts
{

    public class PatrolPoint
    {
        private Vector3 position;
        private Vector2 stopoverLookingDirection;
        private float stopoverDuration;

        public PatrolPoint(Vector3 pointPosition, Vector2 stopoverLookDir, float stopoverDur)
        {
            position = pointPosition;
            stopoverLookingDirection = stopoverLookDir;
            stopoverDuration = stopoverDur;
        }

        public Vector3 GetPointPosition() => position;
        public Vector2 GetStopoverLookingDir() => stopoverLookingDirection;
        public float GetStopoverDuration() => stopoverDuration;

    }
}

