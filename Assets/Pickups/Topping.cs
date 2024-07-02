using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToppingType
{
    clock,
    olive,
    pepperoni,
    tomato,
}
public class Topping : MonoBehaviour
{
    public ToppingType type;
    public int score = 1;
    public Vector3 speed = new Vector3(0, 0, 0);

	private void Update()
	{
        transform.Rotate(speed * Time.deltaTime);
	}
	public void Unload()
    {
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
        Destroy(gameObject, 5);
    }
}
