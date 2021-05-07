using System.Collections;
using UnityEngine;

public class Utils
{
    static public float DistBetweenPoints(Vector3 a, Vector3 b)
    {
        Vector3 c = b - a;
        return c.magnitude;
    }

}