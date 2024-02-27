using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum Lane
	{
		left,
		right,
		center
	}
	public Lane lane = Lane.center;

	public float jumpForce = 500f;
	public bool isGrounded = true;
	public Transform groundCheck;
	public LayerMask groundMask;
	public float groundDistance = 0.4f;

	private Rigidbody rb;
	public Vector3 displacement;
	private Vector3 targetPosition;
	private bool isMoving = false;

	// Crouching
	public float crouchSpeed = 2f;
	public float normalHeight = 1f;
	public float crouchHeight = 1f;

	private bool isCrouching = false;

	BoxCollider boxCollider;

	// Player Art
	public GameObject playerArt;
	Animator animator;
	

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		transform.localPosition = Vector3.zero;
		targetPosition = transform.localPosition;
		boxCollider = GetComponent<BoxCollider>();
		animator = playerArt.GetComponentInChildren<Animator>();
	}


	// Update is called once per frame
	void Update()
	{
		if (isMoving || !isGrounded) return;

		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			StartCoroutine(MoveTo(Lane.left));
		}

		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			StartCoroutine(MoveTo(Lane.center));
		}

		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			StartCoroutine(MoveTo(Lane.right));
		}

		if (Input.GetKeyDown(KeyCode.Keypad8))
		{
			Jump();
		}

		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			Crouch();
		}
	}

	private void Jump()
	{
		if (isCrouching)
		{
			Crouch();
		}
		rb.AddForce(Vector3.up * jumpForce);
	}

	public void Crouch()
	{
		if (!isCrouching)
		{
			animator.SetBool("crouching", true);
			// Crouch down
			float newSize = boxCollider.size.y / 2;
			boxCollider.size = new Vector3(boxCollider.size.x, newSize, boxCollider.size.z);
			boxCollider.center = Vector3.zero;
			//transform.localScale = new Vector3(1, crouchHeight / normalHeight, 1);
			isCrouching = true;
			//transform.position -= new Vector3(0, .3f, 0);
		}
		else
		{
			animator.SetBool("crouching", false);
			// Stand up
			//transform.localScale = new Vector3(1, 1, 1);
			isCrouching = false;
			//transform.position += new Vector3(0, .3f, 0);
			float newSize = boxCollider.size.y * 2;
			boxCollider.size = new Vector3(boxCollider.size.x, newSize, boxCollider.size.z);
			boxCollider.center = new Vector3(0,.4f,0);
		}
	}

	// Ground check
	private void FixedUpdate()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
	}

	IEnumerator MoveTo(Lane targetLane)
	{
		isMoving = true;
		Vector3 destination = Displacement(targetLane);
		lane = targetLane;
		float currentLerpTime = 0f;
		float lerpTime = 0.8f; // You can adjust this value to make the movement faster or slower

		Vector3 startingPosition = transform.localPosition;
		targetPosition = destination + startingPosition;

		Debug.Log($"StartPos {transform.localPosition}");
		Debug.Log($"EndPos {destination}");

		while (currentLerpTime < lerpTime)
		{
			currentLerpTime += Time.deltaTime;
			float perc = currentLerpTime / lerpTime;
			transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, perc);
			yield return null;
		}

		transform.localPosition = targetPosition;
		isMoving = false;
	}

	Vector3 Displacement(Lane targetLane)
	{
		if (lane == Lane.center)
		{
			if (targetLane == Lane.center) return Vector3.zero;
			if (targetLane == Lane.left) return displacement;
			if (targetLane == Lane.right) return -displacement;
		}

		if (lane == Lane.left)
		{
			if (targetLane == Lane.left) return Vector3.zero;
			if (targetLane == Lane.center) return -displacement;
			if (targetLane == Lane.right) return -displacement * 2;
		}

		if (lane == Lane.right)
		{
			if (targetLane == Lane.right) return Vector3.zero;
			if (targetLane == Lane.center) return displacement;
			if (targetLane == Lane.left) return displacement * 2;
		}

		return Vector3.zero;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Obstacle")) return;
		GetComponentInParent<PathCreation.Examples.PathFollower>().enabled = false;
	}

}
