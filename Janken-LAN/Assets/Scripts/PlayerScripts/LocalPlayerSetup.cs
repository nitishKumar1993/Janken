using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LocalPlayerSetup : NetworkBehaviour
{


	void Start ()
	{
		GameHUD.Instance.Start_CountDown ();
		GetComponent<Rigidbody> ().isKinematic = true;

		SetUp ();
		//StartCoroutine ("SetUp");
	}

	// Use this for initialization
	public void SetUp ()
	{
		GetComponent<SyncPlayer> ().enabled = true;
		if (isServer) {
			if (isLocalPlayer) { 

				GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = false;
				transform.FindChild ("Main Camera").GetComponent <Camera> ().enabled = true;

				GetComponent<PlayerController> ().enabled = true;
				if (GetComponentInChildren<PlayerRotation> ())
					GetComponentInChildren<PlayerRotation> ().enabled = true;
				GetComponent<PlayerCollision> ().enabled = true;
				GetComponent<Rigidbody> ().isKinematic = false;
				GetComponent<LocalPlayer> ().enabled = true;
				GetComponent<LocalPlayer> ().obj_RespawnPoints = GameObject.FindGameObjectWithTag ("Respawn");
				GetComponent<LocalPlayer> ().LoadRespawnPoints ();
				transform.FindChild ("Main Camera").GetComponent <AudioListener> ().enabled = true;
			} else {
				transform.FindChild ("Main Camera").GetComponent <Camera> ().enabled = false;
				GetComponent<PlayerController> ().enabled = false;
				GetComponent<PlayerCollision> ().enabled = true;
				//GetComponent<Rigidbody> ().isKinematic = true;
				GetComponent<Rigidbody> ().isKinematic = false;
				GetComponent<LocalPlayer> ().enabled = true;
				transform.FindChild ("Main Camera").GetComponent <AudioListener> ().enabled = false;
			}
		} else {
			if (isLocalPlayer) {

				GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = false;
				transform.FindChild ("Main Camera").GetComponent <Camera> ().enabled = true;

				GetComponent<PlayerController> ().enabled = true;
				if (GetComponentInChildren<PlayerRotation> ())
					GetComponentInChildren<PlayerRotation> ().enabled = true;
				GetComponent<PlayerCollision> ().enabled = false;
				GetComponent<Rigidbody> ().isKinematic = false;
				GetComponent<LocalPlayer> ().enabled = true;
				GetComponent<LocalPlayer> ().obj_RespawnPoints = GameObject.FindGameObjectWithTag ("Respawn");
				GetComponent<LocalPlayer> ().LoadRespawnPoints ();
				transform.FindChild ("Main Camera").GetComponent <AudioListener> ().enabled = true;
			} else {
				transform.FindChild ("Main Camera").GetComponent <Camera> ().enabled = false;
				GetComponent<PlayerCollision> ().enabled = false;
				//GetComponent<Rigidbody> ().isKinematic = true;
				GetComponent<Rigidbody> ().isKinematic = false;
				GetComponent<PlayerController> ().enabled = false;
				GetComponent<LocalPlayer> ().enabled = true;
				transform.FindChild ("Main Camera").GetComponent <AudioListener> ().enabled = false;
			}
		}
//		if (isLocalPlayer)
//			Scoreboard.Instance.SetMyTeamID (this.tag);
		this.GetComponent <Animator> ().Play ("Spawn", -1);
		//transform.FindChild ("Parent").GetChild (0).GetComponent<Animator> ().enabled = false;

	}

	// Update is called once per frame
	void Update ()
	{

	}
}
