using UnityEngine;
using System.Collections;
using CnControls;

public class PlayerController : MonoBehaviour
{
	
	// public vars
	public float cacheWalkSpeed;
	public float walkSpeed = 6;
	public LayerMask groundedMask;
	public  Transform planet;
	public float gravity = -9.8f;
	
	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	public Transform cameraTransform;
	Rigidbody rigidbody;
	Vector3 gravityUp;
	public Vector3 direction;
	public Vector3 rotationDir;

	public int isMoving;
	public Animator animator;
	public bool isDead;

	void Awake ()
	{
		//Initialize vars
		//cameraTransform = Camera.main.transform;
		planet = GameObject.FindGameObjectWithTag ("Planet").GetComponent<Transform> ();
		rigidbody = GetComponent<Rigidbody> ();
		cacheWalkSpeed = walkSpeed;

		// Disable rigidbody gravity and rotation as this is simulated manually
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		animator = transform.FindChild ("Parent").GetChild (0).GetComponent<Animator> ();
		//rigidbody.isKinematic = false;
	}


	void Update ()
	{
		float inputX = 0;
		float inputY = 0;
		if (walkSpeed != 0) {
			//Joystick Input
			inputX = CnInputManager.GetAxisRaw ("Horizontal");
			inputY = CnInputManager.GetAxisRaw ("Vertical");
		}
	
		if (inputX != 0 && inputY != 0) {
			direction.x = inputX;
			direction.y = 0;
			direction.z = inputY;
		}

		//Animation
		if (inputX == 0 && inputY == 0) {
			isMoving = 0;

		} else {
			isMoving = 1;
		}
		animator.SetInteger ("AnimState", isMoving);


		//Calculate gravity direction
		gravityUp = (transform.position - planet.position).normalized;
		Vector3 localUp = transform.up;
        
       
       
		// Align player's up axis with the centre of planet
		transform.rotation = Quaternion.FromToRotation (localUp, gravityUp) * transform.rotation;
		//transform.rotation = Quaternion.FromToRotation(transform.forward, cameraTransform.TransformDirection(new Vector3(inputX, 0, inputY).normalized)) * transform.rotation;

      

		// Calculate movement
		Vector3 moveDir = new Vector3 (inputX, 0, inputY).normalized;
		Vector3 targetMoveAmount = Vector3.zero;
		//   if (moveDir.sqrMagnitude > 0.001f)
		targetMoveAmount = moveDir * walkSpeed;
		moveAmount = Vector3.SmoothDamp (moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
		
		
		// Grounded check
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, 1 + .1f, groundedMask)) {
			grounded = true;
		} else {
			grounded = false;
		}
		rotationDir = new Vector3 (inputX, 0, inputY);
	}

	void FixedUpdate ()
	{
		if (!isDead) {
			// Apply downwards gravity to body
			rigidbody.AddForce (gravityUp * gravity);

			// Apply movement to rigidbody
			Vector3 localMove = transform.TransformDirection (moveAmount) * Time.fixedDeltaTime;
			rigidbody.MovePosition (rigidbody.position + localMove);
		}
//		if (rigidbody.velocity.x == 0.0f && rigidbody.velocity.y == 0.0f && rigidbody.velocity.z == 0.0f)
//			Debug.Log ("IDLE");
//		else
//			Debug.Log ("MOVING");

		//Debug.Log ("RigidBody Velocity: " + rigidbody.velocity);

	}
}
