using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{

    /// <summary>
    /// Builds a Bezier curve from the given points and anchors
    /// </summary>
    /// <param name="pointA">Starting point of the curve</param>
    /// <param name="anchorA">Control point for <paramref name="pointA"/></param>
    /// <param name="pointB">Ending point of the curve</param>
    /// <param name="anchorB">Control point for <paramref name="pointB"/></param>
    /// <param name="pointCount">Amount of points generated for the curve</param>
    /// <returns>Array of points that describe the Bezier Curve requested</returns>
    public static Vector3[] BezierCurve(Vector3 pointA, Vector3 anchorA, Vector3 pointB, Vector3 anchorB, int pointCount = 30)
    {
        Vector3[] bezierPoints = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            //Vector3 position = Vector3.Lerp(entryA.conduitSocket.transform.position, entryB.conduitSocket.transform.position, (float)i / (lineRenderer.numPositions - 1f));
            Vector3 position = (Mathf.Pow((1f - t), 3f) * pointA) + (3f * Mathf.Pow((1f - t), 2f) * t * anchorA) + (3f * (1f - t) * Mathf.Pow(t, 2f) * anchorB) + (Mathf.Pow(t, 3f) * pointB);

            bezierPoints[i] = position;
        }
        return bezierPoints;
    }
}
