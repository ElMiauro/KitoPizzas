using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class PathFill : MonoBehaviour
{
    public PathCreator path;
    public GameObject cube;
    public GameObject player;
    //public Vector3[] vertices;
    public float displacement;
    PathCreation.Examples.PathFollower follower;
    // Start is called before the first frame update
    void Start()
    {

        follower = player.GetComponent<PathCreation.Examples.PathFollower>();
        InvokeRepeating(nameof(Spawn4Obstacles), 0, 5);
        
		
        
    }

    void Spawn4Obstacles()
    {
        float d = follower.distanceTraveled;
        
		for (int i = 0; i < 5; i++)
		{
            Vector3 basePos = path.path.GetPointAtDistance(d + i * displacement);
            Quaternion baseRot = path.path.GetRotationAtDistance(d + i * displacement);
            Instantiate(cube, basePos, baseRot);
        }
    }

    
}
