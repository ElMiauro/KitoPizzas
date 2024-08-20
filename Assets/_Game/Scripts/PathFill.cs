using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class PathFill : MonoBehaviour
{
	public PathCreator path;
	public GameObject player;

	public float displacement;
	PathCreation.Examples.PathFollower follower;
	public int maxSpawns;

	public SpawneableType lastSpawned = SpawneableType.obstacle;
	public List<GameObject> obstacles = new List<GameObject>();
	public List<GameObject> toppings = new List<GameObject>();

	public Vector3 rotationDisplacement;
	public float laneDisplacement;
	public float heigthAdjustment;

	public float headStart;


	void Start()
	{
		follower = player.GetComponent<PathCreation.Examples.PathFollower>();
		Spawn();
		float d = follower.distanceTraveled;
	}

	void Spawn()
	{
		

		for (int i = 0; i < maxSpawns; i++)
		{
			GameObject spawneable;

			// Alternate between obstacles and toppings
			if (lastSpawned == SpawneableType.obstacle)
			{
				spawneable = GetRandomFromList(toppings);
				lastSpawned = SpawneableType.topping;
			}
			else
			{
				spawneable = GetRandomFromList(obstacles);
				lastSpawned = SpawneableType.obstacle;
			}

			SpawneableObject so = spawneable.GetComponent<SpawneableObject>();
			Vector3 laneDisplacement = GetLaneDisplacement(so.placement);

			float distance = headStart + (i * displacement);
			Vector3 basePos = path.path.GetPointAtDistance(distance);
			Quaternion baseRot = path.path.GetRotationAtDistance(distance);
			Instantiate(spawneable, basePos + so.posDisplacement + laneDisplacement + new Vector3(0, heigthAdjustment,0), Quaternion.Euler(rotationDisplacement + baseRot.eulerAngles));
		}
	}

	Vector3 GetLaneDisplacement(ItemPlacement placement)
	{
		int[] choices;
		int lane;

		switch (placement)
		{
			case ItemPlacement.all:
				choices = new int[] { 1, 0, -1 };
				lane = GetRandomChoice(choices);
				return new Vector3(lane * laneDisplacement, 0,0);
			case ItemPlacement.side:
				choices = new int[] { 1, -1 };
				lane = GetRandomChoice(choices);
				return new Vector3(lane * laneDisplacement, 0,0);
			default:
				return Vector3.zero;
		}
	}


	int GetRandomChoice(int[] choices)
	{
		int idx = Random.Range(0, choices.Length);
		return choices[idx];
	}

	GameObject GetRandomFromList(List<GameObject> list)
	{
		int idx = Random.Range(0, list.Count);
		return list[idx];
	}


}
