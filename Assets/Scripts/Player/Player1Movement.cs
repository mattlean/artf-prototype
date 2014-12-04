using UnityEngine;
using System.Collections;

public class Player1Movement : MonoBehaviour {
	enum direction {up, down, left, right, upleft, upright, downleft, downright};
	direction playerDirection = direction.up;
	Rigidbody playerRigidbody; //Reference to the player's rigidbody

	void Awake() {
		//Setup references
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void Move() {
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		//determine facing direction
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

		float offsetX = 0;
		float offsetZ = 0;
		float moveSpeed = 0.08f;
		
		//determine movement
		if (h == -1) {
			offsetX = -moveSpeed;
		} else if (h == 1) {
			offsetX = moveSpeed;
		}
		
		if (v == -1) {
			offsetZ = -moveSpeed;
		} else if (v == 1) {
			offsetZ = moveSpeed;
		}

		transform.position = new Vector3 (transform.position.x + offsetX, transform.position.y, transform.position.z + offsetZ);
	}

	// Update is called once per frame
	void Update() {
		Move ();
	}
}
