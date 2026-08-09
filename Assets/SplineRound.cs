using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRound : MonoBehaviour
{
    private SplineAnimate follower;
    public List<SplineContainer> splineContainers; // List of splines editable in the inspector
    public GameObject otherThings;
    public int currentRound = 0; // Initialize current round

    private void OnEnable()
    {
        follower = GetComponent<SplineAnimate>();
        if (follower != null)
        {
            // Subscribe to the event
            follower.Completed += OnSplineAnimationCompleted;
        }
    }

    private void OnDisable()
    {
        if (follower != null)
        {
            // Unsubscribe from the event
            follower.Completed -= OnSplineAnimationCompleted;
        }
    }

    // This method will be called when the event is triggered
    private void OnSplineAnimationCompleted()
    {
        // Stop processing if we've reached the end of the spline list
        if (currentRound >= splineContainers.Count)
        {
            Debug.Log("No more splines to move to. Remaining static.");
            return;
        }
        currentRound++; // Increment the round counter

        // Get the next spline container from the list
        SplineContainer splineContainer = splineContainers[currentRound];

        Debug.Log($"Spline animation completed! Moving to spline index: {currentRound}");

        // Assign the selected spline container
        follower.Container = splineContainer;

        

        // Uncomment if you want to move "otherThings" as well
        // otherThings.transform.position += new Vector3(30, 0);
    }
}
