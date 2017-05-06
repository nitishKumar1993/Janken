using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


public struct PlayerInfo
{
	public string Name;
	public int TeamID;
	public int ModelID;
}

public class SyncPlayer : NetworkBehaviour
{
	[SyncVar]
	private Vector3 syncPosition;
	[SyncVar]
	private Quaternion syncRotation;
	[SyncVar]
	private Quaternion syncChildRotation;
	//	[SyncVar]
	//	public PlayerInfo PlayerDetails;

	[SyncVar]
	public string Name;
	[SyncVar]
	public int TeamID;
	[SyncVar]
	public int ModelID;

	[SyncVar]
	public PlayerInfo Killer;
	[SyncVar]
	public PlayerInfo Victim;
	[SyncVar (hook = "OnPlayerKill")]
	public int Kills;
	[SyncVar (hook = "OnStreakKills")]
	public int Multi_Kills;
	[SyncVar (hook = "OnPlayerDead")]
	public int Deaths;
	[SyncVar]
	public int Assits;
	[SyncVar (hook = "OnPlayerRespawn")]
	public bool Respawn;
	[SyncVar (hook = "OnPowerActivated")]
	public ArrayList activePowerType;
	[SyncVar (hook = "OnPowerEquip")]
	public PowerType selectedPowerType;
	[SyncVar (hook = "OnPending_Activation")]
	public PowerType pending_ActivationPowerType;
	[SyncVar (hook = "OnPending_DeActivation")]
	public PowerType pending_DeActivationPowerType;

	public bool isShieldActive {
		get {
			return activePowerType.Contains (PowerType.Buff_Shield);
		}
	}

	[SyncVar]
	public int isMoving;


	[SerializeField]public Transform playerTransform;
	[SerializeField]public Transform playerChildTransform;

	public Animator animator;
	float LerpRate = 15;
	public LocalPlayer localPlayer;

	// Use this for initialization
	void Start ()
	{
		activePowerType = new ArrayList ();
		localPlayer = GetComponent<LocalPlayer> ();
		animator = transform.FindChild ("Parent").GetChild (0).GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	
		if (isLocalPlayer) {



			if (GameHUD.Instance.PowerActivated) {
				if (!activePowerType.Contains (selectedPowerType)) {
					GameHUD.Instance.powers.DisableAllButtons ();
					GameHUD.Instance.PowerActivated = false;

					TogglePower ();
				}
			}
		}

		SentDataFromLocalPlayerToServer ();
		LerpPlayerData ();

		//Debug.Log (gameObject.name + " Deaths: " + Deaths);
		//Debug.Log (gameObject.name + " Kills: " + Kills);
	}

	void LerpPlayerData ()
	{
		if (!isLocalPlayer) {
			playerTransform.localPosition = Vector3.Lerp (playerTransform.localPosition, syncPosition, Time.deltaTime * LerpRate);
			playerTransform.localRotation = Quaternion.Lerp (playerTransform.localRotation, syncRotation, Time.deltaTime * LerpRate);
			playerChildTransform.localRotation = Quaternion.Lerp (playerChildTransform.localRotation, syncChildRotation, Time.deltaTime * LerpRate);
			animator.SetInteger ("AnimState", isMoving);
		}
	}

	[Command]
	void CmdSentDataToClientsAcrossNetwork (Vector3 playerPos, Quaternion playerRot, Quaternion childRot, int _isMoving)
	{
		syncPosition = playerPos;
		syncRotation = playerRot;
		syncChildRotation = childRot;
		isMoving = _isMoving;
	}

	[Command]
	void CmdOnRespawn ()
	{
		Respawn = true;
	}

	[Command]
	void CmdTogglePower ()
	{

		pending_ActivationPowerType = selectedPowerType;


		//ActivatePower = toggle;
	}

	[ClientCallback]
	public void TogglePower ()
	{
		if (isLocalPlayer) {
			CmdTogglePower ();
		}
	}

	[ClientCallback]
	public void OnRespawn ()
	{
		if (isLocalPlayer) {
			CmdOnRespawn ();
		}
	}

	[ClientCallback]
	void SentDataFromLocalPlayerToServer ()
	{
		if (isLocalPlayer) {
			CmdSentDataToClientsAcrossNetwork (playerTransform.localPosition, playerTransform.localRotation, playerChildTransform.localRotation, GetComponent<PlayerController> ().isMoving);
		}
	}

	void OnKiller (PlayerInfo p)
	{
		Killer = p;
	}

	void OnVictim (PlayerInfo p)
	{
		Victim = p;
	}

	void OnPlayerDead (int d)
	{
		Deaths = d;
		GameHUD.Instance.SpawnDeathParticleSystem (transform.position);
		localPlayer.PlayerDead (isLocalPlayer, isServer);
		Multi_Kills = 0;
		//Update Score

		if (isLocalPlayer) {
			Deaths = d;
			localPlayer.UI_Deaths ();
			localPlayer.UI_Respawn ();
		}
	}

	void OnPlayerKill (int d)
	{
		Kills = d;
		if (isLocalPlayer) {
			Kills = d;
		
			localPlayer.UI_Kills ();
		}
	}

	void OnStreakKills (int kills)
	{
		Multi_Kills = kills;
		if (kills > 0) {
			if (isServer) {
				StopCoroutine ("StreakCoolDown");
				StartCoroutine ("StreakCoolDown");
			}
			GameHUD.Instance.Streak_Kills_GUI (isLocalPlayer, Name, kills);
		}
	}

	void OnPlayerRespawn (bool d)
	{
		Respawn = d;

		if (d) {
			localPlayer.PlayerRespawn (false);
			StartCoroutine ("ResetRespawn");
		}
	}

	void OnPowerEquip (PowerType pt)
	{
		selectedPowerType = pt;
		if (isLocalPlayer) {
			selectedPowerType = pt;
			localPlayer.UI_TogglePowerButton (pt);
		}
	}

	void OnPowerActivated (ArrayList pt)
	{
		activePowerType = pt;
	}

	void OnPending_Activation (PowerType type)
	{
		

		pending_ActivationPowerType = type;
		if (type != PowerType.None) {
			Debug.Log ("Acticating Pending Power: " + type);
			//if (act) {


			localPlayer.ApplyBuff (type, isServer, isLocalPlayer);
			if (type != PowerType.Launcher_Freeze && type != PowerType.Launcher_Skull) {

				activePowerType.Add (type);
			}
			//localPlayer.ApplyBuff (powerType, isLocalPlayer);

			StartCoroutine ("ResetPowerType");
		}
	}

	void OnPending_DeActivation (PowerType type)
	{




		pending_DeActivationPowerType = type;
		if (type != PowerType.None) {
			Debug.Log ("De Acticating Pending Power: " + type);
			//if (act) {


			localPlayer.RemoveBuff (type, isServer, isLocalPlayer);
			if (type != PowerType.Launcher_Freeze || type != PowerType.Launcher_Skull) {

				activePowerType.Remove (type);
			}
			//localPlayer.ApplyBuff (powerType, isLocalPlayer);

			StartCoroutine ("ResetPendingPowerType");
		}
	}

	IEnumerator ResetPendingPowerType ()
	{
		yield return new WaitForSeconds (0.1f);

		pending_DeActivationPowerType = PowerType.None;
	}

	IEnumerator ResetRespawn ()
	{
		yield return new WaitForSeconds (0.1f);
		Respawn = false;
	}

	IEnumerator ResetPowerType ()
	{
		yield return new WaitForSeconds (0.1f);
		selectedPowerType = PowerType.None;
		pending_ActivationPowerType = PowerType.None;
	}

	IEnumerator StreakCoolDown ()
	{
		yield return new WaitForSeconds (20.0f);
		Multi_Kills = 0;
	}

	//	void OnPowerActivate (bool act)
	//	{
	//		isPowerActive = act;
	//		localPlayer.TogglePowerParticleSystem (powerType, act);
	//		if (isLocalPlayer) {
	//			isPowerActive = act;
	//			localPlayer.TogglePowerBuff (powerType, act);
	//		}
	//		if (!act)
	//			GetComponent<SyncPlayer> ().powerType = PowerType.None;
	//	}
}
