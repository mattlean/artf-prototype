using UnityEngine;

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
	public GameObject cursor;
	private Transform cursorLocation;

	void Awake() {
		//Create a layer mask for the floor layer
		floorMask = LayerMask.GetMask ("Floor");

		//Setup references
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
		Instantiate (cursor, new Vector3(7.71f, 3f, 2.75f), Quaternion.Euler(new Vector3(90, 0, 0)));
		cursor.name = "P1Cursor";
		cursorLocation = GameObject.Find ("P1Cursor(Clone)").GetComponent<Transform>();
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
			Attack(1f);
		}

		if (Input.GetButton ("Fire2") && timer >= cooldown && Time.timeScale != 0) {
			Psynergy (5f);
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
			tempVector = new Vector3(-1, 0, 1);
			break;
		case 5:
			tempVector = new Vector3(1, 0, 1);
			break;
		case 6:
			tempVector = new Vector3(-1, 0, -1);
			break;
		case 7:
			tempVector = new Vector3(1, 0, -1);
			break;
		default:
			tempVector = Vector3.forward;
			break;
		}
		Quaternion newDirection = Quaternion.LookRotation(tempVector, Vector3.up);
		playerRigidbody.MoveRotation (newDirection);
	}

	void Animating(float h, float v) {
		//Create a boolean that is true if either of the input axes is non-zero
		bool walking = h != 0f || v != 0f;

		//Tell the animator whether or not the player is walking
		anim.SetBool ("IsWalking", walking);
	}

	void Attack(float radius) {
		//Reset the timer
		timer = 0f;

		Vector3 tempVector = transform.position;
		switch ((int)playerDirection) {
		case 1:
			tempVector = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		case 2:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z);
			break;
		case 3:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z);
			break;
		case 4:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		case 5:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		case 6:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		case 7:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		default:
			tempVector = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		}

		Collider[] hitColliders = Physics.OverlapSphere(tempVector, radius);
		hitbox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hitbox.transform.position = tempVector;
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

	void Psynergy(float radius) {
		/*//Reset the timer
		timer = 0f;
		
		Vector3 tempVector = transform.position;
		switch ((int)playerDirection) {
		case 1:
			tempVector = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		case 2:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z);
			break;
		case 3:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z);
			break;
		case 4:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		case 5:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		case 6:
			tempVector = new Vector3(transform.position.x - 1, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		case 7:
			tempVector = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, transform.position.z - 1);
			break;
		default:
			tempVector = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 1);
			break;
		}
		
		Collider[] hitColliders = Physics.OverlapSphere(tempVector, radius);
		hitbox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hitbox.transform.position = tempVector;
		hitbox.transform.localScale = new Vector3 (radius, radius, radius);
		hitbox.renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
		hitbox.renderer.collider.enabled = false;
		
		int i = 0;
		while (i < hitColliders.Length) {
			if(hitColliders[i].gameObject.layer == 9)
				print(hitColliders[i]);
			i++;
		}*/
		print ("psynergy called");

		print (cursor.transform.position);
		cursorLocation.position = new Vector3(cursorLocation.position.x - 0.1f, cursorLocation.position.y, cursorLocation.position.z);
		//cursor.transform.position = new Vector3(cursor.transform.position.x - 0.1f, cursor.transform.position.y, cursor.transform.position.z);
	}
}
