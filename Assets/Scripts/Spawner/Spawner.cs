using System.Collections.Generic;
using UnityEngine;

namespace KitoPizza.Spawner
{
    public class Spawner : MonoBehaviour
    {
        public List<SpawnerData> spawnerData;

        [Header("Spawner rules")] public bool spawnAtStart = true;
        public int maxSpawns = 1;
        public bool alwaysSpawn = true;

        [Header("Lanes")]
        public bool leftLaneAvailable = true;
        public bool middleLaneAvailable = true;
        public bool rightLaneAvailable = true;


        [Header("Placement settings")] public Vector3 laneDisplacement;

        // Start is called before the first frame update
        void Start()
        {
            if (spawnAtStart)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            var spawnedObjects = 0;
            if (spawnerData.Count == 0)
            {
                Debug.LogWarning("You didn't add any data to the spawner", gameObject);
                return;
            }

            while (alwaysSpawn && spawnedObjects < maxSpawns)
            {
                foreach (var data in spawnerData)
                {
                    if (spawnedObjects >= maxSpawns)
                    {
                        break;
                    }

                    if (Random.value > data.spawnRate)
                    {
                        continue;
                    }

                    // Random between 0 and 2. 0 is left, 1 is middle, 2 is right

                    var laneIsValid = false;

                    int laneIndex = 0;
                    while (!laneIsValid)
                    {
                        laneIndex = Random.Range(0, 3);

                        if (laneIndex == 0 && leftLaneAvailable)
                        {
                            laneIsValid = true;
                        }
                        else if (laneIndex == 1 && middleLaneAvailable)
                        {
                            laneIsValid = true;
                        }
                        else if (laneIndex == 2 && rightLaneAvailable)
                        {
                            laneIsValid = true;
                        }
                    }


                    // Spawn the object using the lane index and displacement. 
                    // If index = 0 then transform.position.x = -laneDisplacement.x
                    // If index = 1 then transform.position.x = 0
                    // If index = 2 then transform.position.x = laneDisplacement.x
                    Vector3 lanePosition = Vector3.zero;

                    if (laneIndex == 0)
                    {
                        Debug.Log("Left lane");
                        lanePosition = new Vector3(-laneDisplacement.x, 0, -laneDisplacement.z);
                    }
                    else if (laneIndex == 2)
                    {
                        Debug.Log("Right lane");
                        lanePosition = new Vector3(laneDisplacement.x, 0, laneDisplacement.z);
                    }
                    else
                    {
                        Debug.Log("Middle lane");
                    }

                    var obj = Instantiate(data.prefab, transform);
                    obj.transform.localPosition = lanePosition + data.positionOffset;
                    obj.transform.localRotation = Quaternion.Euler(data.rotationOverride);
                    spawnedObjects++;
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            // Set the matrix to object's transform (position, rotation and scale)
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

// Draw a red cube at object's position

            Gizmos.color = middleLaneAvailable ? Color.red : Color.grey;
            Gizmos.DrawCube(Vector3.zero, new Vector3(1, 1, 1));

// Draw a green cube at object's position moved forward
            Gizmos.color = rightLaneAvailable ? Color.green : Color.grey;
            Gizmos.DrawCube(new Vector3(0, 0, laneDisplacement.z), new Vector3(1, 1, 1));

// Draw a blue cube at object's position moved backward
            Gizmos.color = leftLaneAvailable ? Color.blue : Color.grey;
            Gizmos.DrawCube(new Vector3(0, 0, -laneDisplacement.z), new Vector3(1, 1, 1));
        }
    }
}