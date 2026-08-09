using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelZone : MonoBehaviour
{
	public GameObject pivot;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Got player");
			other.GetComponentInChildren<PlayerController>().enabled = false;
			other.GetComponentInChildren<PlayerTunnel>().enabled = true;
			other.GetComponentInChildren<PlayerTunnel>().pivot = pivot;
			other.transform.SetParent(pivot.transform);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			other.GetComponentInChildren<PlayerController>().enabled = true;
			other.GetComponentInChildren<PlayerTunnel>().enabled = false;
		}
	}
}
