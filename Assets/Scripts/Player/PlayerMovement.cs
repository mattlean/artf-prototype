﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6f; //The speed that the player will move at

	Vector3 movement; //The vector to store the direction of the player's movement
	Animator anim; //Reference to the animator's component
	Rigidbody playerRigidbody; //Reference to the player's rigidbody
	int floorMask; //A layer mask so that a ray can be cast just a game objs on the floor layer
	GameObject hitbox;
	float timer; //A timer to determine when to fire
	float cooldown = 0.3f;
	enum direction {up, down, left, right, upleft, upright, downleft, downright};
	direction playerDirection = direction.up;
	Vector3 directionVector = Vector3.forward;

	void Awake() {
		//Create a layer mask for the floor layer
		floorMask = LayerMask.GetMask ("Floor");

		//Setup references
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate() {
		//Store the input axes
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		//Move the player around the scene
		Move (h, v);

		//Animate the player
		Animating (h, v);

		if (hitbox != null) {
			Destroy (hitbox);
		}
	}

	void Update() {
		//Add the time since Update was last called to the timer
		timer += Time.deltaTime;

		if(Input.GetButton ("Fire1") && timer >= cooldown && Time.timeScale != 0) {
			Attack(new Vector3(6f, 0.97f, 3.38f), 1f);
		}
	}

	void Move(float h, float v) {
		//Set the movement vector based on the axis input
		movement.Set (h, 0f, v);

		//Normalize the movement vector and make it proportional to the speed per second
		movement = movement.normalized * speed * Time.deltaTime;

		//Move the player to its current position plus the movement
		playerRigidbody.MovePosition (transform.position + movement);
		
		if (h == -1) {
			directionVector = Vector3.left;
			playerDirection = direction.left;
		} else if (h == 1) {
			playerDirection = direction.right;
			directionVector = Vector3.right;
		}

		if (v == -1) {
			directionVector = Vector3.back;
			playerDirection = direction.down;
		} else if (v == 1) {
			directionVector = Vector3.forward;
			playerDirection = direction.up;
		}

		if (h == -1 && v == -1) {
			directionVector = new Vector3(-1, 0, -1);
			playerDirection = direction.downleft;
		} else if (h == 1 && v == -1) {
			directionVector = new Vector3(1, 0, -1);
			playerDirection = direction.downright;
		} else if (h == -1 && v == 1) {
			directionVector = new Vector3(-1, 0, 1);
			playerDirection = direction.upleft;
		} else if (h == 1 && v == 1) {
			directionVector = new Vector3(1, 0, 1);
			playerDirection = direction.upright;
		}

		Quaternion newRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		playerRigidbody.MoveRotation (newRotation);
	}

	void Animating(float h, float v) {
		//Create a boolean that is true if either of the input axes is non-zero
		bool walking = h != 0f || v != 0f;

		//Tell the animator whether or not the player is walking
		anim.SetBool ("IsWalking", walking);
	}

	void Attack(Vector3 center, float radius) {
		//Reset the timer
		timer = 0f;

		Collider[] hitColliders = Physics.OverlapSphere(center, radius);

		hitbox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hitbox.transform.position = center;
		hitbox.transform.localScale = new Vector3 (radius, radius, radius);
		hitbox.renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
		hitbox.renderer.collider.enabled = false;

		int i = 0;
		while (i < hitColliders.Length) {
			if(hitColliders[i].gameObject.layer == 9)
				print(hitColliders[i]);
			i++;
		}
	}
}
