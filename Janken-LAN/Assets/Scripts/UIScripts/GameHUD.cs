using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{

	public Text Kills;
	public Text Deaths;


	public Canvas JoyStick;
	public CountDown countDown;
	public Powers powers;
	public bool PowerActivated;

	public GameObject GameOverScreen;
	public GameObject HUD;
	public GameObject Death_Particle;
	public GameObject KillingPopup;
	public GameObject RampageGUI;
	public GameObject DoubleKill_GUI;

	public GameObject RespawnScreen;

	public static GameHUD Instance;
	public float stayTime = 5.0f;



	void Awake ()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start ()
	{
		RespawnScreen.SetActive (false);
		GameOverScreen.SetActive (false);
		countDown.countDownDone += timerDone;
	}

	public void Start_CountDown ()
	{
		ToggleJoyStick (false);
		countDown.Start_CountDown ();
	}

	public void GameOver (bool b)
	{
		GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = true;
		GameOverScreen.SetActive (b);
		HUD.SetActive (!b);
	}



	// Update is called once per frame
	public void Disconnect ()
	{
		GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = true;
		CustomNetworkManager.s_Singleton.StopHost ();
	}

	public void ActivatePower ()
	{
		PowerActivated = true;

	}

	public void SpawnDeathParticleSystem (Vector3 pos)
	{
		Instantiate (Death_Particle, pos, Quaternion.identity);
	}

	void timerDone ()
	{
		
		GameObject.FindGameObjectWithTag ("Planet").GetComponent<MeshCollider> ().convex = false;

		ToggleJoyStick (true);
		Tutorial.Instance.Tut_JoyStick ();
	}

	public void ActivatePowerButton (PowerType pt)
	{
		powers.ActivatePowerButton (pt);
	}

	public void ToggleJoyStick (bool toggle)
	{
		JoyStick.enabled = toggle;
	}

	public void ToggleRespawnScreen (PlayerInfo killer, bool toggle, System.Action respawn)
	{
		RespawnScreen.SetActive (toggle);
		if (toggle)
			RespawnScreen.GetComponent <RespawnScreen> ().SetKillerDetails (killer);


		if (respawn != null) {
			RespawnScreen.transform.FindChild ("Deploy").GetComponent<Button> ().onClick.AddListener (() => {
				respawn ();
			});
		}
	}

	public void KillingAnim (int killerTeamID, PlayerInfo victim, int kills)
	{

		Kills.text = "" + kills;
		KillingPopup.GetComponent <KillingPopup> ().PlayKillAnim (killerTeamID, victim);

	}

	void Streak_Kills_Helper (GameObject gui, string text)
	{
		//Double Kill
		if (gui.activeSelf) {
			StopCoroutine (close_Popup (gui));
		}
		gui.SetActive (true);
		gui.transform.FindChild ("Text").GetComponent <Text> ().text = text;

		StartCoroutine (close_Popup (gui));
	}

	public void Streak_Kills_GUI (bool isLocal, string PlayerName, int kills)
	{
		if (isLocal) {
			if (kills == 2) {
				//Double Kill
				Streak_Kills_Helper (DoubleKill_GUI, "Double-Kill");
			} else if (kills >= 3) {
				//Rampage
				Streak_Kills_Helper (RampageGUI, "Rampage!");
			}
		} else {
			if (kills == 2) {
				//Double Kill
				Streak_Kills_Helper (DoubleKill_GUI, PlayerName + " Got a Double-Kill");
			} else if (kills >= 3) {
				//Rampage
				Streak_Kills_Helper (RampageGUI, PlayerName + " is on a Rampage!");
			}
		}
	}


	IEnumerator close_Popup (GameObject obj)
	{
		yield return new WaitForSeconds (stayTime);
		obj.SetActive (false);
	}


}

