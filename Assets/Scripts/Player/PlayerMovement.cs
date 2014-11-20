using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6f; //The speed that the player will move at

	Vector3 movement; //The vector to store the direction of the player's movement
	Animator anim; //Reference to the animator's component
	Rigidbody playerRigidbody; //Reference to the player's rigidbody
	int floorMask; //A layer mask so that a ray can be cast just a game objs on the floor layer
	Vector3 playerDirection = Vector3.forward; //Determines the direction the player is facing

	void Awake() {
		//Create a layer mask for the floor layer
		floorMask = LayerMask.GetMask ("Floor");

		//Setup references
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate() {
		//Store the input axes
		int h = (int)Input.GetAxisRaw ("Horizontal");
		int v = (int)Input.GetAxisRaw ("Vertical");

		//Move the player around the scene
		Move (h, v);

		//Animate the player
		Animating (h, v);
	}

	void Move(float h, float v) {
		//Set the movement vector based on the axis input
		movement.Set (h, 0f, v);

		//Normalize the movement vector and make it proportional to the speed per second
		movement = movement.normalized * speed * Time.deltaTime;

		//Move the player to its current position plus the movement
		playerRigidbody.MovePosition (transform.position + movement);
		
		if (h == -1) {
			playerDirection = Vector3.left;
		} else if (h == 1) {
			playerDirection = Vector3.right;
		}

		if (v == -1) {
			playerDirection = Vector3.back;
		} else if (v == 1) {
			playerDirection = Vector3.forward;
		}

		if (h == -1 && v == -1) {
			playerDirection = new Vector3(-1, 0, -1);
		} else if (h == 1 && v == -1) {
			playerDirection = new Vector3(1, 0, -1);
		} else if (h == -1 && v == 1) {
			playerDirection = new Vector3(-1, 0, 1);
		} else if (h == 1 && v == 1) {
			playerDirection = new Vector3(1, 0, 1);
		}

		Quaternion newDirection = Quaternion.LookRotation(playerDirection, Vector3.up);
		playerRigidbody.MoveRotation (newDirection);
	}

	void Animating(float h, float v) {
		//Create a boolean that is true if either of the input axes is non-zero
		bool walking = h != 0f || v != 0f;

		//Tell the animator whether or not the player is walking
		anim.SetBool ("IsWalking", walking);
	}
}
