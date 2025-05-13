using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Default,
    Tree
}

public class ObstacleTypeHolder : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType;

    public ObstacleType GetObstacleType() => obstacleType;
}
