using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : SpawneableObject
{

	public bool[] GetValidLanes()
	{
		bool[] all = { true, true, true };
		bool[] sides = { true, false, true };
		bool[] center = { false, true, false };
		switch (placement)
		{
			case ItemPlacement.center:
				return center;
			case ItemPlacement.side:
				return sides;
			default:
				return all;
		}
	}
}
