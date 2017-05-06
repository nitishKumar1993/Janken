using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocalPlayer : MonoBehaviour
{
	public List<Transform> RespawnPoints;
	public GameObject obj_RespawnPoints;

	//Buffs
	public List<GameObject> Launcher_Freeze_Particles;
	public List<GameObject> Launcher_Skull_Particles;
	public GameObject Freeze_Particle;
	public GameObject Speed_Particle;
	public GameObject Shield_Particle;

	public Transform Parent;
	public SyncPlayer syncPlayer;
	public PlayerController playerController;

	public bool shieldActive {
		get {
			return Shield_Particle.activeSelf;
		}
	}


	public int Deaths {
		get {
			return GetComponent <SyncPlayer> ().Deaths;
		}
	}

	public int Kills {
		get {
			return GetComponent <SyncPlayer> ().Kills;
		}
	}

	private delegate IEnumerator CoroutineDelegate (float time, PowerType type);
	//Stop Couroutines
	IEnumerator Buff_Speed;
	IEnumerator Buff_Shield;
	IEnumerator Buff_Freeze;
	IEnumerator Launcher_Freeze;
	IEnumerator Launcher_Skull;

	void Start ()
	{ 
		Parent = this.transform.FindChild ("Parent").transform;
		syncPlayer = GetComponent <SyncPlayer> ();
		playerController = GetComponent <PlayerController> ();

		Launcher_Freeze_Particles = new List<GameObject> ();
		Launcher_Skull_Particles = new List<GameObject> ();

		Buff_Speed = DeActivate_Power (6.0f, PowerType.Buff_Speed);
		Buff_Shield = DeActivate_Power (5.0f, PowerType.Buff_Shield);
		Buff_Freeze = DeActivate_Power (3.0f, PowerType.Buff_Freeze);
		Launcher_Freeze = DeActivate_Power (1.0f, PowerType.Launcher_Freeze);
		Launcher_Skull = DeActivate_Power (1.0f, PowerType.Launcher_Skull);

		Speed_Particle = Parent.FindChild ("Powers").transform.FindChild ("Buffs").transform.FindChild ("Speed").gameObject;
		Shield_Particle = Parent.FindChild ("Powers").transform.FindChild ("Buffs").transform.FindChild ("Shield Mesh").gameObject;
		Freeze_Particle = Parent.FindChild ("Powers").transform.FindChild ("Buffs").transform.FindChild ("Frozen").gameObject;
	}

	// Use this for initialization
	public void LoadRespawnPoints ()
	{
		RespawnPoints.Clear ();
		foreach (Transform t in obj_RespawnPoints.transform) {
			RespawnPoints.Add (t);
		}
	}

	public void UI_Deaths ()
	{

		GameHUD.Instance.Deaths.text = "" + Deaths;

		
	}

	public void UI_TogglePowerButton (PowerType tp)
	{

		GameHUD.Instance.ActivatePowerButton (tp);
	}

	//Called on all clients
	public void TogglePowerParticleSystem (PowerType tp, bool toggle)
	{

		switch (tp) {
		case PowerType.Buff_Speed:
			
			Speed_Particle.SetActive (toggle);
			
			break;

		}

	}

	IEnumerator PowerCastAnim ()
	{
		yield return new WaitForSeconds (0.165f);
		Parent.GetChild (0).GetComponent<Animator> ().SetBool ("Power", false);

	}


	public void ApplyBuff (PowerType type, bool isServer, bool isLocalPlayer)
	{

		Parent.GetChild (0).GetComponent<Animator> ().SetBool ("Power", true);
		StartCoroutine ("PowerCastAnim");

		switch (type) {

		case PowerType.Buff_Speed:
			Speed_Particle.SetActive (true);
			if (isLocalPlayer) {
				playerController.walkSpeed = 8;

			}
			if (isServer) {
				Buff_Speed = DeActivate_Power (6.0f, PowerType.Buff_Speed);
				StartCoroutine (Buff_Speed);
			}
			break;

		case PowerType.Buff_Shield:
			Shield_Particle.SetActive (true);
			if (isServer) {
				Buff_Shield = DeActivate_Power (5.0f, PowerType.Buff_Shield);
				StartCoroutine (Buff_Shield);
			}
			break;

		case PowerType.Launcher_Freeze:
			if (isServer) {
				GameObject particle = (GameObject)Instantiate (PowersManager.Instance.Launcher_Freeze, transform.position + (Parent.forward * 2), Quaternion.identity);

				particle.tag = this.gameObject.tag + "_Launcher_Freeze";
				particle.layer = LayerMask.NameToLayer (this.gameObject.tag + "_Launcher_Freeze");
				particle.transform.rotation = Parent.rotation;

				Launcher l = particle.GetComponent<Launcher> ();
				l.walkSpeed = 15f;
				l.WHO_FIRED = this.gameObject;

				Launcher_Freeze_Particles.Add (particle);

				NetworkServer.Spawn (particle);
				Launcher_Freeze = DeActivate_Power (1.0f, PowerType.Launcher_Freeze);
				StartCoroutine (Launcher_Freeze);
			}

			if (isLocalPlayer) {
				//Set Direction
			}
			break;

		case PowerType.Launcher_Skull:
			if (isServer) {
				GameObject particle = (GameObject)Instantiate (PowersManager.Instance.Launcher_Skull, transform.position + (Parent.forward * 2), Quaternion.identity);

				particle.tag = this.gameObject.tag + "_Launcher_Skull";
				particle.layer = LayerMask.NameToLayer (this.gameObject.tag + "_Launcher_Skull");
				particle.transform.rotation = Parent.rotation;

				Launcher l = particle.GetComponent<Launcher> ();
				l.walkSpeed = 15f;
				l.WHO_FIRED = this.gameObject;

				Launcher_Skull_Particles.Add (particle);

				NetworkServer.Spawn (particle);
				Launcher_Skull = DeActivate_Power (1.0f, PowerType.Launcher_Skull);
				StartCoroutine (Launcher_Skull);
			}

			if (isLocalPlayer) {
				//Set Direction
			}
			break;

		case PowerType.Buff_Freeze:
			Freeze_Particle.SetActive (true);
			if (isLocalPlayer)
				playerController.walkSpeed = 0;
			if (isServer) {
				Buff_Freeze = DeActivate_Power (3.0f, PowerType.Buff_Freeze);
				StartCoroutine (Buff_Freeze);
			}
			break;
		}
	}

	public void RemoveBuff (PowerType type, bool isServer, bool isLocalPlayer)
	{
		switch (type) {

		case PowerType.Buff_Speed:
			//Activate_Speed_Buff (isServer, isLocalPlayer);
			Speed_Particle.SetActive (false);
			if (isLocalPlayer)
				playerController.walkSpeed = playerController.cacheWalkSpeed;
			break;

		case PowerType.Buff_Shield:
			Shield_Particle.SetActive (false);
			break;

		case PowerType.Launcher_Freeze:
			if (isServer) {
				
				if (Launcher_Freeze_Particles [0] != null) {
					GameObject part = Launcher_Freeze_Particles [0];
					Launcher_Freeze_Particles.RemoveAt (0);
					part.GetComponent <Launcher> ().NetworkDestroy ();
				}
			}
			break;

		case PowerType.Launcher_Skull:
			if (isServer) {
				
				if (Launcher_Skull_Particles [0] != null) {
					GameObject part = Launcher_Skull_Particles [0];
					part.GetComponent <Launcher> ().NetworkDestroy ();
					Launcher_Skull_Particles.RemoveAt (0);

				}
			}
			break;
				
		case PowerType.Buff_Freeze:
			Freeze_Particle.SetActive (false);
			if (isLocalPlayer)
				playerController.walkSpeed = playerController.cacheWalkSpeed;
			break;
		}
	}


	public IEnumerator DeActivate_Power (float timer, PowerType type)
	{
		yield return new WaitForSeconds (timer);
		syncPlayer.pending_DeActivationPowerType = type;
	}

	public void Destroy_Shield_Buff ()
	{
		StopCoroutine (Buff_Shield);
		syncPlayer.pending_DeActivationPowerType = PowerType.Buff_Shield;
	}

	public void Destroy_Launcher_Freeze ()
	{
		StopCoroutine (Launcher_Freeze);

		syncPlayer.pending_DeActivationPowerType = PowerType.Launcher_Freeze;
	}

	public void Destroy_Launcher_Skull ()
	{
		StopCoroutine (Launcher_Skull);
		syncPlayer.pending_DeActivationPowerType = PowerType.Launcher_Skull;
	}

	public void UI_Respawn ()
	{
		
		//GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = true;
		//this.gameObject.transform.localPosition = Vector3.zero;


		GameHUD.Instance.ToggleJoyStick (false);

		GameHUD.Instance.ToggleRespawnScreen (syncPlayer.Killer, true, () => {
			Respawn ();
		});

	}


	public void PlayerRespawn (bool isLocal)
	{
		if (isLocal)
			playerController.isDead = false;
		this.GetComponent<CapsuleCollider> ().enabled = true;
		Parent.gameObject.SetActive (true);

		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
		this.GetComponent <Animator> ().Play ("Spawn", -1);
	}

	public void PlayerDead (bool isLocal, bool isServer)
	{
		if (isLocal)
			playerController.isDead = true;
		this.GetComponent<CapsuleCollider> ().enabled = false;
		Parent.gameObject.SetActive (false);
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;

		foreach (PowerType item in syncPlayer.activePowerType) {
			RemoveBuff (item, isServer, isLocal);
		}
		if (isServer) {
			syncPlayer.selectedPowerType = PowerType.None;
			syncPlayer.activePowerType.Clear ();
		}
	}

	public void UI_Kills ()
	{
		
		GameHUD.Instance.KillingAnim (syncPlayer.TeamID, syncPlayer.Victim, Kills);


	}

	public void Respawn ()
	{
		GameHUD.Instance.ToggleJoyStick (true);
		GameHUD.Instance.ToggleRespawnScreen (new PlayerInfo (), false, null);
		GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = false;
	
		if (RespawnPoints != null)
			this.transform.localPosition = RespawnPoints [Random.Range (0, RespawnPoints.Count)].localPosition;

		PlayerRespawn (true);
		syncPlayer.OnRespawn ();
	}

}
