using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using CnControls;

public class Launcher : NetworkBehaviour
{

	[SyncVar]
	private Vector3 syncPosition;
	[SyncVar]
	private Quaternion syncRotation;

	[SerializeField]public Transform particleTransform;



	// public vars

	public float walkSpeed = 6;
	public LayerMask groundedMask;
	public  Transform planet;
	public float gravity = -9.8f;

	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	Rigidbody rigidbody;
	Vector3 gravityUp;

	public Vector3 rotationDir;

	public bool selfDestroy = true;
	public float selfDestroyTime;
	public Vector3 direction;
	string teamID;
	Transform forward;
	public GameObject WHO_FIRED;

	float LerpRate = 15;
	// Use this for initialization
	void Start ()
	{
		if (isServer) {
			teamID = this.gameObject.tag;

			planet = GameObject.FindGameObjectWithTag ("Planet").GetComponent<Transform> ();
			rigidbody = GetComponent<Rigidbody> ();

			// Disable rigidbody gravity and rotation as this is simulated manually
			//rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			//direction = transform.TransformDirection (transform.right);
		}
	}

	public void NetworkDestroy ()
	{
		NetworkServer.Destroy (this.gameObject);
	}


	// Update is called once per frame
	void Update ()
	{


		//	if (selfDestroy)
		//Destroy (this.gameObject, selfDestroyTime);
		
		if (isServer) {
			//Joystick Input
//			float inputX = direction.x;
//			float inputY = direction.y;



			//Calculate gravity direction
			gravityUp = (transform.position - GameObject.FindGameObjectWithTag ("Planet").GetComponent<Transform> ().position).normalized;
			Vector3 localUp = transform.up;


			// Align player's up axis with the centre of planet
			transform.rotation = Quaternion.FromToRotation (localUp, gravityUp) * transform.rotation;



			//Roation
//			Vector3 rotDir = transform.forward;
//			if (rotDir != Vector3.zero) {
//				//Calculate amount of rotation in degrees
//				Quaternion newRot = Quaternion.LookRotation (rotDir);
//
//				//Apply Rotation and lock rotation along X and Z axis
//				transform.localRotation = Quaternion.Euler (0, newRot.eulerAngles.y, 0);
//			}

			// Calculate movement
			//Vector3 moveDir = new Vector3 (inputX, 0, inputY).normalized;
		

//			Debug.Log (moveDir);
//
//			Vector3 targetMoveAmount = Vector3.zero;
//			//   if (moveDir.sqrMagnitude > 0.001f)
//			targetMoveAmount = moveDir * walkSpeed;
//			moveAmount = Vector3.SmoothDamp (moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

			transform.Translate (Vector3.forward * Time.deltaTime * walkSpeed, Space.Self);

		}

		SentDataFromLocalPlayerToServer ();
		LerpPlayerData ();
	}

	void FixedUpdate ()
	{
		if (isServer) {
//			// Apply downwards gravity to body
			GetComponent<Rigidbody> ().AddForce (gravityUp * gravity);
//
//			// Apply movement to rigidbody
//			Vector3 localMove = transform.TransformDirection (moveAmount) * Time.fixedDeltaTime;
//
//			GetComponent<Rigidbody> ().MovePosition (GetComponent<Rigidbody> ().position + localMove);
		}
	}

	void LerpPlayerData ()
	{
		if (!isServer) {
			particleTransform.localPosition = Vector3.Lerp (particleTransform.localPosition, syncPosition, Time.deltaTime * LerpRate);
			particleTransform.localRotation = Quaternion.Lerp (particleTransform.localRotation, syncRotation, Time.deltaTime * LerpRate);

		}
	}

	[Command]
	void CmdSentDataToClientsAcrossNetwork (Vector3 playerPos, Quaternion playerRot)
	{
		syncPosition = playerPos;
		syncRotation = playerRot;

	}

	[ClientCallback]
	void SentDataFromLocalPlayerToServer ()
	{
		if (isServer) {
			CmdSentDataToClientsAcrossNetwork (particleTransform.localPosition, particleTransform.localRotation);
		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (isServer) {
			//Scissor > Paper > Rock > Scissors
			Launcher_Freeze (col);
			Launcher_Skull (col);
		}
	}

	void HandleKillDeath (GameObject _I_AM_DEAD, GameObject _I_KILLED_SOMEONE)
	{
		SyncPlayer I_AM_DEAD = _I_AM_DEAD.GetComponent <SyncPlayer> ();
		SyncPlayer I_KILLED_SOMEONE = _I_KILLED_SOMEONE.GetComponent <SyncPlayer> ();
		Scoreboard.Instance.Scoreboard_Update (I_KILLED_SOMEONE.TeamID, I_AM_DEAD.TeamID);

		//	I_AM_DEAD.Killer = null;
		PlayerInfo k;
		k.Name = I_KILLED_SOMEONE.Name;
		k.TeamID = I_KILLED_SOMEONE.TeamID;
		k.ModelID = I_KILLED_SOMEONE.ModelID;
		I_AM_DEAD.Killer = k;

		//I_KILLED_SOMEONE.Victim = null;
		PlayerInfo v;
		v.Name = I_AM_DEAD.Name;
		v.TeamID = I_AM_DEAD.TeamID;
		v.ModelID = I_AM_DEAD.ModelID;
		I_AM_DEAD.Victim = v;

		//I_AM_DEAD.GetComponent<SyncPlayer> ().isAlive = false;
		++I_AM_DEAD.Deaths;
		++I_KILLED_SOMEONE.Kills;
		++I_KILLED_SOMEONE.Multi_Kills;



	}

	void Launcher_Skull (Collision col)
	{
		switch (teamID) {

		case "Rock_Launcher_Skull":
			if (col.gameObject.tag == "Scissor" || col.gameObject.tag == "Paper") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {

					HandleKillDeath (col.gameObject, WHO_FIRED);

					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
				} else {

					col.gameObject.GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
//			}
			break;

		case "Paper_Launcher_Skull":
			if (col.gameObject.tag == "Rock" || col.gameObject.tag == "Scissor") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {
					HandleKillDeath (col.gameObject, WHO_FIRED);
					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
				} else {

					col.gameObject.GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
//			}
			break;


		case "Scissor_Launcher_Skull":
			if (col.gameObject.tag == "Paper" || col.gameObject.tag == "Rock") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {

					HandleKillDeath (col.gameObject, WHO_FIRED);
					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
				} else {

					col.gameObject.GetComponent<LocalPlayer> ().Destroy_Shield_Buff ();
				}

			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Skull ();
//			}
			break;
		}
	}


	void Launcher_Freeze (Collision col)
	{
		switch (teamID) {

		case "Rock_Launcher_Freeze":
			if (col.gameObject.tag == "Scissor" || col.gameObject.tag == "Paper") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {

					col.gameObject.GetComponent<SyncPlayer> ().pending_ActivationPowerType = PowerType.Buff_Freeze;
					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
				} else {

					col.gameObject.GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
//			}
			break;

		case "Paper_Launcher_Freeze":
			if (col.gameObject.tag == "Rock" || col.gameObject.tag == "Scissor") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {
					col.gameObject.GetComponent<SyncPlayer> ().pending_ActivationPowerType = PowerType.Buff_Freeze;
					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
				} else {

					col.gameObject.GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
//			}
			break;


		case "Scissor_Launcher_Freeze":
			if (col.gameObject.tag == "Paper" || col.gameObject.tag == "Rock") {
				if (!col.gameObject.GetComponent <SyncPlayer> ().isShieldActive) {

					col.gameObject.GetComponent<SyncPlayer> ().pending_ActivationPowerType = PowerType.Buff_Freeze;
					WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
				} else {

					col.gameObject.GetComponent<LocalPlayer> ().Destroy_Shield_Buff ();
				}

			}
//			if (col.gameObject.tag == "Environment") {
//				WHO_FIRED.GetComponent <LocalPlayer> ().Destroy_Launcher_Freeze ();
//			}
			break;
		}
	}
}
