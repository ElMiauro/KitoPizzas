using System;
using UnityEngine;

namespace KitoPizza.Spawner
{
    [Serializable]
    public class SpawnerData
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnRate;
        public Vector3 positionOffset;
        public Vector3 rotationOverride;
    }
}