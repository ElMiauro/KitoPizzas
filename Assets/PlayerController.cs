using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public bool canTakeDamage = true;
	public enum Lane
	{
		left,
		right,
		center
	}
	public Lane lane = Lane.center;

	public enum PlayerState
	{
		idle,
		moving,
		crouch,
		jump,
		knocked
	}
	public PlayerState state = PlayerState.idle;

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

	// Managers
	GameController controller;
	

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		transform.localPosition = Vector3.zero;
		targetPosition = transform.localPosition;
		boxCollider = GetComponent<BoxCollider>();
		animator = playerArt.GetComponentInChildren<Animator>();
		controller = GameController.control;
	}


	// Update is called once per frame
	void Update()
	{
		if (isMoving || !isGrounded) return;
		if (state == PlayerState.knocked) return;

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

		if (Input.GetKeyDown(KeyCode.M))
		{
			playerArt.GetComponentInChildren<CharacterFlash>().StartFlashing(5);
		}
	}



	private void Jump()
	{
		if (isMoving) return;
		if (isCrouching)
		{
			Crouch();
		}
		state = PlayerState.jump;
		rb.AddForce(Vector3.up * jumpForce);
	}

	public void Crouch()
	{
		if (!isCrouching)
		{
			state = PlayerState.crouch;
			animator.SetBool("crouching", true);
			// Crouch down
			float newSize = boxCollider.size.y / 2;
			boxCollider.size = new Vector3(boxCollider.size.x, newSize, boxCollider.size.z);
			boxCollider.center = Vector3.zero;
			isCrouching = true;
		}
		else
		{
			state = PlayerState.moving;
			animator.SetBool("crouching", false);
			// Stand up
			isCrouching = false;
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
		if (other.CompareTag("Topping"))
		{
			Topping topping = other.GetComponent<Topping>();
			controller.GetTopping(topping.type, topping.score);
			topping.Unload();
			return;
		}
		if (!canTakeDamage) return;
		if (!other.CompareTag("Obstacle")) return;
		if (state == PlayerState.knocked) return;
		state = PlayerState.knocked;
		animator.SetTrigger("Knocked");
		GetComponentInParent<PathCreation.Examples.PathFollower>().enabled = false;
		controller.GetHit();
	}

}
