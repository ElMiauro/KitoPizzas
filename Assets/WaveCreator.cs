using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCreator : MonoBehaviour
{

    public GameObject cube;
    public List<GameObject> cubes = new List<GameObject>();
    // Example usage:
    public float amplitude = 10; // Peak height of the wave
    public float frequency = 0.1f; // Number of cycles in a given unit of x
    public float phaseShift = 0; // No horizontal shift
    public float verticalShift = 0; // Centered vertically
    public int numPoints = 100; // Number of points to generate
    public float xStep = 0.1f; // Increment in x for each point
    void GenerateWavePoints(float amplitude, float frequency, float phaseShift, float verticalShift, int numPoints, float xStep)
    {
        for (int i = 0; i < numPoints; i++)
        {
            float x = i * xStep;
            float y = amplitude * Mathf.Sin(frequency * x + phaseShift) + verticalShift;

            float dy_dx = amplitude * frequency * Mathf.Cos(frequency * x + phaseShift);

            // Calculate the rotation angle in radians
            float rotation = Mathf.Atan(dy_dx);

            cubes[i].transform.position = new Vector3(x, y);
            cubes[i].transform.rotation = Quaternion.Euler(new Vector3(0,0, rotation * Mathf.Rad2Deg));
    }
}



	private void Start()
	{
        for (int i = 0; i < numPoints; i++)
        {
            GameObject _ = Instantiate(cube);
            cubes.Add(_);
        }
    }

	private void Update()
	{
        GenerateWavePoints(amplitude, frequency, phaseShift, verticalShift, numPoints, xStep);
    }

}
