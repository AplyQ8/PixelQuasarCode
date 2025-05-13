using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUtility
{
    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    
    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
    
    public static Direction AngleToDirection(float angle)
    {
        Direction result;
        
        if(-157.5 < angle && angle <= -112.5)
            result = Direction.SouthWest;
        else if(-112.5 < angle && angle <= -67.5)
            result = Direction.South;
        else if(-67.5 < angle && angle <= -22.5)
            result = Direction.SouthEast;
        else if(-22.5 < angle && angle <= 22.5)
            result = Direction.East;
        else if(22.5 < angle && angle <= 67.5)
            result = Direction.NorthEast;
        else if (67.5 < angle && angle <= 112.5)
            result = Direction.North;
        else if(112.5 < angle && angle <= 157.5)
            result = Direction.NorthWest;
        else
            result = Direction.West;
        
        return result;
    }

    public static float DirectionToAngle(Direction direction)
    {
        float result = 0;
        switch (direction)
        {
            case Direction.North:
                result = 90;
                break;
            case Direction.NorthEast:
                result = 45;
                break;
            case Direction.East:
                result = 0;
                break;
            case Direction.SouthEast:
                result = -45;
                break;
            case Direction.South:
                result = -90;
                break;
            case Direction.SouthWest:
                result = -135;
                break;
            case Direction.West:
                result = 180;
                break;
            case Direction.NorthWest:
                result = 135;
                break;
        }

        return result;
    }

    public static Vector2 DirectionToVector(Direction direction)
    {
        Vector2 directionVector = Vector2.right;
        
        switch (direction)
        {
            case Direction.North:
                directionVector = Vector2.up;
                break;
            case Direction.NorthEast:
                directionVector = (Vector2.up + Vector2.right).normalized;
                break;
            case Direction.East:
                directionVector = Vector2.right;
                break;
            case Direction.SouthEast:
                directionVector = (Vector2.right + Vector2.down).normalized;
                break;
            case Direction.South:
                directionVector = Vector2.down;
                break;
            case Direction.SouthWest:
                directionVector = (Vector2.down + Vector2.left).normalized;
                break;
            case Direction.West:
                directionVector = Vector2.left;
                break;
            case Direction.NorthWest:
                directionVector = (Vector2.left + Vector2.up).normalized;
                break;
        }

        return directionVector;
    }
    
    public static int RoundAngle(float angle)
    {
        int result = 0;
        
        if(-157.5 < angle && angle <= -112.5)
            result = -135;
        else if(-112.5 < angle && angle <= -67.5)
            result = -90;
        else if(-67.5 < angle && angle <= -22.5)
            result = -45;
        else if(-22.5 < angle && angle <= 22.5)
            result = 0;
        else if(22.5 < angle && angle <= 67.5)
            result = 45;
        else if (67.5 < angle && angle <= 112.5)
            result = 90;
        else if(112.5 < angle && angle <= 157.5)
            result = 135;
        else
            result = 180;

        return result;
    }
    
    public static Vector2[] base4Directions = {
            Vector2.up, 
            Vector2.right,
            Vector2.down,
            Vector2.left
    };
}
