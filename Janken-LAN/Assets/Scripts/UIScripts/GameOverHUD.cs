using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverHUD : MonoBehaviour
{

	public Transform First;
	public Transform Second;
	public Transform Third;

	public Text score_first;
	public Text score_second;
	public Text score_third;

	public Image victory_Rock;
	public Image victory_Paper;
	public Image victory_Scissor;

	public static GameOverHUD Instance;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
//		FillData (First);
//		FillData (Second);
//		FillData (Third);
	}

	public void FillData (Transform t, List<SyncPlayer> players = null)
	{
		if (players != null) {
			for (int i = 0; i < players.Count; i++) {
				t.GetChild (i).FindChild ("Player name").GetComponent <Text> ().text = players [i].Name;
				t.GetChild (i).FindChild ("Kills").GetComponent <Text> ().text = "" + players [i].Kills;
				t.GetChild (i).FindChild ("Deaths").GetComponent <Text> ().text = "" + players [i].Deaths;
			} 
		} else {
			for (int i = 0; i < 3; i++) {
				t.GetChild (i).FindChild ("Player name").GetComponent <Text> ().text = "";
				t.GetChild (i).FindChild ("Kills").GetComponent <Text> ().text = "";
				t.GetChild (i).FindChild ("Deaths").GetComponent <Text> ().text = "";
			} 
		}
	}

	public void Init (List<SyncPlayer> first, List<SyncPlayer> second, List<SyncPlayer> third)
	{
		FillData (First, first);
		FillData (Second, second);
		FillData (Third, third);

	}

	public void Init (int a, int b, int c)
	{
		score_first.text = "" + a;
		score_second.text = "" + b;
		score_third.text = "" + c;
	
	}

	public void VictoryImage (int teamID)
	{
		switch (teamID) {
		case 1:
			victory_Rock.enabled = true;
			break;
		case 2:
			victory_Paper.enabled = true;
			break;

		case 3:
			victory_Scissor.enabled = true;
			break;
		}
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}
