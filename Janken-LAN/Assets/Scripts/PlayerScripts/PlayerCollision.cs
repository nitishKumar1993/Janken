using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	int damage = 100;
	string teamID;
	// Use this for initialization
	void Start ()
	{
		teamID = this.gameObject.tag;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
		I_KILLED_SOMEONE.Victim = v;

		//I_AM_DEAD.GetComponent<SyncPlayer> ().isAlive = false;
		++I_AM_DEAD.Deaths;
		++I_KILLED_SOMEONE.Kills;
		++I_KILLED_SOMEONE.Multi_Kills;

	}


	void OnCollisionEnter (Collision col)
	{
		//Scissor > Paper > Rock > Scissors
	
		if (col.gameObject.tag == "Power") {
			//if (GetComponent<SyncPlayer> ().selectedPowerType == PowerType.None) {
			GetComponent<SyncPlayer> ().selectedPowerType = col.gameObject.GetComponent <PowerInfo> ().powerType;
			col.gameObject.GetComponent <PowerInfo> ().NetworkDestroy (); 
			//}
		}

		switch (teamID) {

		case "Rock":
			if (col.gameObject.tag == "Paper") {
				if (!GetComponent <SyncPlayer> ().isShieldActive) {
					HandleKillDeath (this.gameObject, col.gameObject);

				} else {
					
					GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			}
			break;

		case "Paper":
			Debug.Log ("Paper Collision");
			if (col.gameObject.tag == "Scissor") {
				Debug.Log ("Paper Scissor Collision");
				if (!GetComponent <SyncPlayer> ().isShieldActive) {
					HandleKillDeath (this.gameObject, col.gameObject);

				} else {
					
					GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}
			} 
			break;
	
	
		case "Scissor":
			if (col.gameObject.tag == "Rock") {
				if (!GetComponent <SyncPlayer> ().isShieldActive) {
					HandleKillDeath (this.gameObject, col.gameObject);

				} else {
					
					GetComponent <LocalPlayer> ().Destroy_Shield_Buff ();
				}

			}
			break;
		}

	}
}
