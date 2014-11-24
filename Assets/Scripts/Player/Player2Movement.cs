using UnityEngine;
using System.Collections.Generic;

public class Player2Movement : MonoBehaviour
{
	public float speed = 6f; //The speed that the player will move at
	
	Vector3 movement; //The vector to store the direction of the player's movement
	Animator anim; //Reference to the animator's component
	Rigidbody playerRigidbody; //Reference to the player's rigidbody
	GameObject hitbox;
	float timer; //A timer to determine when to fire
	float cooldown = 0.3f;
	enum direction {up, down, left, right, upleft, upright, downleft, downright};
	direction playerDirection = direction.up;
	bool isCasting = false;
	bool psyMove = false;
	List<Collider> psyObjs = new List<Collider>();
	bool isLifting = false;
	Collider pillar;
	
	void Awake() {
		//Setup references
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate() {
		//Store the input axes
		float h = Input.GetAxisRaw ("MoveHorizontal");
		float v = Input.GetAxisRaw ("MoveVertical");
		
		//Move the player arwound the scene
		Move (h, v);
		
		//Animate the player
		Animating (h, v);
	}
	
	void Move(float h, float v) {
		if (isCasting == false) {
			//Set the movement vector based on the axis input
			movement.Set (h, 0f, v);
			
			//Normalize the movement vector and make it proportional to the speed per second
			movement = movement.normalized * speed * Time.deltaTime;
			
			//Move the player to its current position plus the movement
			playerRigidbody.MovePosition (transform.position + movement);
			
			if (h == -1) {
				playerDirection = direction.left;
			} else if (h == 1) {
				playerDirection = direction.right;
			}
			
			if (v == -1) {
				playerDirection = direction.down;
			} else if (v == 1) {
				playerDirection = direction.up;
			}
			
			if (h == -1 && v == -1) {
				playerDirection = direction.downleft;
			} else if (h == 1 && v == -1) {
				playerDirection = direction.downright;
			} else if (h == -1 && v == 1) {
				playerDirection = direction.upleft;
			} else if (h == 1 && v == 1) {
				playerDirection = direction.upright;
			}
			
			Vector3 tempVector = Vector3.forward;
			switch ((int)playerDirection) {
			case 1:
				tempVector = Vector3.back;
				break;
			case 2:
				tempVector = Vector3.left;
				break;
			case 3:
				tempVector = Vector3.right;
				break;
			case 4:
				tempVector = new Vector3 (-1, 0, 1);
				break;
			case 5:
				tempVector = new Vector3 (1, 0, 1);
				break;
			case 6:
				tempVector = new Vector3 (-1, 0, -1);
				break;
			case 7:
				tempVector = new Vector3 (1, 0, -1);
				break;
			default:
				tempVector = Vector3.forward;
				break;
			}
			Quaternion newDirection = Quaternion.LookRotation (tempVector, Vector3.up);
			playerRigidbody.MoveRotation (newDirection);
		} else if(psyMove == false) {
			float offsetX = 0;
			float offsetZ = 0;
			
			if (h == -1) {
				offsetX = -0.1f;
			} else if (h == 1) {
				offsetX = 0.1f;
			}
			
			if (v == -1) {
				offsetZ = -0.1f;
			} else if (v == 1) {
				offsetZ = 0.1f;
			}
		}
	}
	
	void Animating(float h, float v) {
		if (isCasting == false) {
			//Create a boolean that is true if either of the input axes is non-zero
			bool walking = h != 0f || v != 0f;
			
			//Tell the animator whether or not the player is walking
			anim.SetBool ("IsWalking", walking);
		} else {
			anim.SetBool ("IsWalking", false);
		}
	}
}
