using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public float distanceTraveled;

        public Vector3 rotationFix;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTraveled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled, endOfPathInstruction);
                transform.rotation = Quaternion.Euler(pathCreator.path.GetRotationAtDistance(distanceTraveled, endOfPathInstruction).eulerAngles + rotationFix);
            }
        }

        // If the path changes during the game, update the distance traveled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTraveled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}