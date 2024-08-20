using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawneableType
{
    topping,
    obstacle
}

public enum ItemPlacement
{
    all,
    center,
    side
}

public class SpawneableObject : MonoBehaviour
{
    public SpawneableType spawneableType;
    public ItemPlacement placement;
    public Vector3 posDisplacement;

}
