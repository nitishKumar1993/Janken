using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KillingPopup : MonoBehaviour
{


	public GameObject Rock;
	public GameObject Paper;
	public GameObject Scissor;

	public float animationTime = 2.0f;
	// Use this for initialization
	void Start ()
	{
		Rock.SetActive (false);
		Paper.SetActive (false);
		Scissor.SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void PlayKillAnim (int KillerTeamID, PlayerInfo Victim)
	{
		
		switch (KillerTeamID) {
		case 1:
			Rock.SetActive (true);
			Rock.transform.FindChild ("Pandu").GetComponent <Text> ().text = Victim.Name;
			Rock.GetComponent <Animator> ().Play ("" + Victim.TeamID, -1);
			StartCoroutine (close_KillingPopup (Rock));
			break;
		case 2:
			Paper.SetActive (true);
			Paper.transform.FindChild ("Pandu").GetComponent <Text> ().text = Victim.Name;
			Paper.GetComponent <Animator> ().Play ("" + Victim.TeamID, -1);
			StartCoroutine (close_KillingPopup (Paper));
			break;

		case 3:
			Scissor.SetActive (true);
			Scissor.transform.FindChild ("Pandu").GetComponent <Text> ().text = Victim.Name;
			Scissor.GetComponent <Animator> ().Play ("" + Victim.TeamID, -1);
			StartCoroutine (close_KillingPopup (Scissor));
			break;
		}
	}

	IEnumerator close_KillingPopup (GameObject obj)
	{
		yield return new WaitForSeconds (animationTime);
		obj.SetActive (false);
	}
}
