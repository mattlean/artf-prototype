using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
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
	public GameObject cursor;
	private Transform cursorLocation;
	bool isCasting = false;
	bool psyMove = false;
	List<Collider> psyObjs = new List<Collider>();
	bool isLifting = false;
	Collider pillar;

	void Awake() {
		//Setup references
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();

		Instantiate (cursor, new Vector3(transform.position.x, -1f, transform.position.y), Quaternion.Euler(new Vector3(90, 0, 0)));
		cursor.name = "P1Cursor";
		cursorLocation = GameObject.Find ("P1Cursor(Clone)").GetComponent<Transform>();
	}

	void FixedUpdate() {
		//Store the input axes
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		//Move the player arwound the scene
		Move (h, v);

		//Animate the player
		Animating (h, v);

		if (hitbox != null) {
			Destroy (hitbox);
		}

		if (isLifting) {
			if(psyObjs[0].transform.position.y <= 0.884) {
				pillar.transform.position = new Vector3(psyObjs[0].transform.position.x, psyObjs[0].transform.position.y + 0.01f, psyObjs[0].transform.position.z);
			} else {
				isLifting = false;
			}
		}
	}

	void Update() {
		//Add the time since Update was last called to the timer
		timer += Time.deltaTime;

		if(Input.GetButton ("Fire1") && timer >= cooldown && Time.timeScale != 0) {
			Attack(1f);
		}

		if (Input.GetButtonDown ("Fire2") && psyMove == false) {
			PsynergyActivate ();
		} else if (Input.GetButtonUp ("Fire2") && psyMove == false) {
			PsynergyRelease ();
		}else if(Input.GetButtonDown ("Lift") && psyMove == false) {
			LiftActivate();
		} else if(Input.GetButtonUp ("Lift") && psyMove == false){
			LiftRelease ();
		} else if (psyMove) {
			psyMoveControl();
		}
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

			cursorLocation.position = new Vector3(cursorLocation.position.x + offsetX, 3f, cursorLocation.position.z + offsetZ);
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

	void Attack(float radius) {
		//Reset the timer
		timer = 0f;

		float hsPosX = transform.position.x;
		float hsPosY = transform.position.y + 0.5f;
		float hsPosZ = transform.position.z;

		switch ((int)playerDirection) {
		case 1:
			hsPosZ -= 1;
			break;
		case 2:
			hsPosX -= 1;
			break;
		case 3:
			hsPosX += 1;
			break;
		case 4:
			hsPosX -= 1;
			hsPosZ += 1;
			break;
		case 5:
			hsPosX += 1;
			hsPosZ += 1;
			break;
		case 6:
			hsPosX -= 1;
			hsPosZ -= 1;
			break;
		case 7:
			hsPosX += 1;
			hsPosZ -= 1;
			break;
		default:
			hsPosZ += 1;
			break;
		}

		Vector3 hsVector = new Vector3(hsPosX, hsPosY, hsPosZ);

		createHitsphere (hsVector, 1f, 9);
	}

	List<Collider> createHitsphere(Vector3 hsVector, float radius, int layerID) {
		Collider[] hitColliders = Physics.OverlapSphere(hsVector, radius);
		hitbox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hitbox.transform.position = hsVector;
		hitbox.transform.localScale = new Vector3 (radius, radius, radius);
		hitbox.renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
		hitbox.renderer.collider.enabled = false;
		
		int i = 0;
		List<Collider> affectedObjs = new List<Collider>();
		while (i < hitColliders.Length) {
			if(hitColliders[i].gameObject.layer == layerID) {
				print (hitColliders[i]);
				affectedObjs.Add(hitColliders[i]);
			}
			i++;
		}

		return affectedObjs;
	}

	void PsynergyActivate() {
		print ("psynergy activated");
		isCasting = true;
		cursorLocation.position = new Vector3(transform.position.x, 3f, transform.position.z);
	}

	void PsynergyRelease() {
		print ("psynergy released");
		List<Collider> affectedObjs = createHitsphere(new Vector3(cursorLocation.position.x, 0f, cursorLocation.position.z), 3, 9);
		if (affectedObjs.Count != 0) {
			psyMove = true;
			psyObjs = affectedObjs;
			print (psyObjs);
		} else {
			PsynergyStop();	
		}

		cursorLocation.position = new Vector3(0, -1f, 0);
	}

	void psyMoveControl() {
		if (Input.GetButtonUp ("Fire2")) {
			PsynergyStop ();
		}

		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		float offsetX = 0;
		float offsetZ = 0;
		float moveSpeed = 0.02f;

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

		if (psyObjs [0].name == "Boulder") {
			psyObjs[0].rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			PsynergyStop();
		} else {
			psyObjs [0].transform.position = new Vector3 (psyObjs [0].transform.position.x + offsetX, psyObjs [0].transform.position.y, psyObjs [0].transform.position.z + offsetZ);
		}
	}

	void LiftActivate() {
		print ("lift activated");
		isCasting = true;
		cursorLocation.position = new Vector3(transform.position.x, 3f, transform.position.z);
	}

	void LiftRelease() {
		print ("lift deactivated");
		List<Collider> affectedObjs = createHitsphere(new Vector3(cursorLocation.position.x, 0f, cursorLocation.position.z), 2, 10);
		if (affectedObjs.Count != 0) {
			psyMove = true;
			isLifting = true;
			psyObjs = affectedObjs;
			pillar = psyObjs[0];
		}
		PsynergyStop();	
	}

	void PsynergyStop() {
		isCasting = false;
		psyMove = false;
		cursorLocation.position = new Vector3(0, -1f, 0);
	}
}
