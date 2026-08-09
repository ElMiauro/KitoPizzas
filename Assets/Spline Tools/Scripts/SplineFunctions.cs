using UnityEngine;
using UnityEngine.Splines;

namespace SplineTools
{
    //Used to work with the spline
    public static class SplineFunctions
    {
        //Get the distance along the spline curve, closest to point
        public static float GetDistanceAlongSpline(SplineContainer splineContainer,int index, Vector3 point, int samples = 100)
        {
            if (splineContainer == null)
            {
                Debug.LogError("SplineContainer is not assigned.");
                return -1f;
            }

            Spline spline = splineContainer.Splines[index];
            float closestDistance = float.MaxValue;
            float closestT = 0f;

            // Find the closest t value
            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples;
                Vector3 splinePoint = spline.EvaluatePosition(t);
                float distanceToSplinePoint = Vector3.Distance(point, splinePoint);
                if (distanceToSplinePoint < closestDistance)
                {
                    closestDistance = distanceToSplinePoint;
                    closestT = t;
                }
            }


            // Calculate distance from start to the closest t value
            float distance = 0f;
            Vector3 previousPoint = spline.EvaluatePosition(0f);
            int segments = 1000; // Increase the segments for higher precision

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments * closestT;
                Vector3 splinePoint = spline.EvaluatePosition(t);
                distance += Vector3.Distance(previousPoint, splinePoint);
                previousPoint = splinePoint;
            }

            return distance;
        }
    }
}
