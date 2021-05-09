using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Utils
{
     static public float DistBetweenPoints(Vector3 a, Vector3 b)
    {
        Vector3 c = b - a;
        return c.magnitude;
    }

    static public bool VectorsCloseToEqual(Vector3 v1, Vector3 v2)
    {
        Vector3 dif = new Vector3(
            v2.x - v1.x,
            0.0f,
            v2.z - v1.z);

        return (dif.magnitude < 0.2f);
    }

    static public Vector3 Vector3NoY(Vector3 v)
    {
        Vector3 n = new Vector3(
            v.x,
            0.0f,
            v.z);

        return v;

    }



}