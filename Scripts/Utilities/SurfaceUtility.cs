using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
    Default,
    Earth,
    Gravel,
    Stone
}

public static class SurfaceUtility
{
    public static string SurfaceTypeToString(SurfaceType surface)
    {
        switch (surface)
        {
            case SurfaceType.Earth:
                return "Earth";
            case SurfaceType.Gravel:
                return "Gravel";
            case SurfaceType.Stone:
                return "Stone";
            default:
                return "Default";
        }
    }
}
