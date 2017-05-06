using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class Scoreboard : NetworkBehaviour
{
	public int IncrementValue;
	public int DecrementValue;

	[SyncVar (hook = "OnRockScoreChange")]
	private int Rock;
	[SyncVar (hook = "OnPaperScoreChange")]
	private int Paper;
	[SyncVar (hook = "OnScissorScoreChange")]
	private int Scissor;
	[SyncVar (hook = "OnGameOver")]
	private bool isGameOver;

	public int MyTeamID;
	int ScoreLimit;

	public GameObject MyTeam;
	public GameObject Team2;
	public GameObject Team3;

	public Sprite sprite_Rock;
	public Sprite sprite_Paper;
	public Sprite sprite_Scissor;

	public Text MyTeam_ScoreText;
	public Text Team2_ScoreText;
	public Text Team3_ScoreText;

	public Image MyTeam_Portrait;
	public Image Team2_Portrait;
	public 	Image Team3_Portrait;

	public 	RectTransform MyTeam_ProgressBar;
	public 	RectTransform Team2_ProgressBar;
	public 	RectTransform Team3_ProgressBar;

	public Animator MyTeam_Animator;
	public Animator Team2_Animator;
	public Animator Team3_Animator;

	public int progressBar_Full;


	public static Scoreboard Instance;
	public float gameTime = 600;
	public Text Timer;
	float widthMultiplier;
	SyncPlayer[] AllPlayers;
	public List<SyncPlayer> Team_Rock = new List<SyncPlayer> ();
	public List<SyncPlayer> Team_Paper = new List<SyncPlayer> ();
	public List<SyncPlayer> Team_Scissor = new List<SyncPlayer> ();

	float height = 28.9f;
	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		ScoreLimit = CustomNetworkManager.s_Singleton.currentServerInfo.ScoreLimit;
		widthMultiplier = 391.0f / ScoreLimit;
		MyTeamID = CustomNetworkManager.s_Singleton.localPlayer.GetComponent <PlayerLobbyHelper> ().TeamID;
		SetupHUD ();
		SetTeamPortrait ();
		AllPlayers = FindObjectsOfType (typeof(SyncPlayer)) as SyncPlayer[];

		foreach (SyncPlayer p in AllPlayers) {
			if (p.TeamID == 1)
				Team_Rock.Add (p);
			if (p.TeamID == 2)
				Team_Paper.Add (p);
			if (p.TeamID == 3)
				Team_Scissor.Add (p);
		}

	}

	void SetupHUD ()
	{
//		MyTeam_ScoreText = MyTeam.FindChild ("Score/Limit").transform.GetComponent <Text> ();
//		MyTeam_Portrait = MyTeam.FindChild ("team").transform.GetComponent <Image> ();
//		MyTeam_ProgressBar = MyTeam.FindChild ("Progress Bar").transform.GetComponent <RectTransform> ();
//		MyTeam_ScoreText.text = str_Score (0);
//		MyTeam_ProgressBar.sizeDelta = new Vector2 (0, 28.9f);


		InitHUD (MyTeam.transform, MyTeam_ScoreText, MyTeam_Portrait, MyTeam_ProgressBar);
		InitHUD (Team2.transform, Team2_ScoreText, Team2_Portrait, Team2_ProgressBar);
		InitHUD (Team3.transform, Team3_ScoreText, Team3_Portrait, Team3_ProgressBar);
	}

	void InitHUD (Transform team, Text score, Image portrait, RectTransform progress)
	{
		//score = team.FindChild ("Score/Limit").transform.GetComponent <Text> ();
		//portrait = team.FindChild ("team").transform.GetComponent <Image> ();
		//progress = team.FindChild ("Progress Bar").transform.GetComponent <RectTransform> ();
		score.text = str_Score (0);
		progress.sizeDelta = new Vector2 (0, 28.9f);
	}

	public void SetTeamPortrait ()
	{
		if (MyTeamID == 1) {
			MyTeam_Portrait.sprite = sprite_Rock;
			Team2_Portrait.sprite = sprite_Scissor;
			Team3_Portrait.sprite = sprite_Paper;
		}
		if (MyTeamID == 2) {
			

			MyTeam_Portrait.sprite = sprite_Paper;
			Team3_Portrait.sprite = sprite_Scissor;
			Team2_Portrait.sprite = sprite_Rock;
		}
		if (MyTeamID == 3) {
			

			MyTeam_Portrait.sprite = sprite_Scissor;
			Team2_Portrait.sprite = sprite_Paper;
			Team3_Portrait.sprite = sprite_Rock;
		}

	
	}

	public string str_Score (int score)
	{
		int sco = score < 0 ? 0 : score;
		return  sco + "/" + ScoreLimit;
	}

	void ScoreChangeResponse (Text t, int score, RectTransform progress, Animator anim)
	{
		t.text = str_Score (score);
		float currentWidth = progress.sizeDelta.x;
		float resultWidth = score * widthMultiplier;
		if (currentWidth < resultWidth) {
			//Increase
			anim.Play ("Increase", -1);
		} else {
			//Decrease
			anim.Play ("Decrease", -1);
		}
		progress.sizeDelta = new Vector2 (score * widthMultiplier, height);
	}

	public void OnRockScoreChange (int score)
	{
		Debug.Log ("Rock Score Changed");
		Rock = score;
		if (MyTeamID == 1) {
			ScoreChangeResponse (MyTeam_ScoreText, score, MyTeam_ProgressBar, MyTeam_Animator);		
		} else if (MyTeamID == 2) {
			ScoreChangeResponse (Team2_ScoreText, score, Team2_ProgressBar, Team2_Animator);
		} else if (MyTeamID == 3) {
			ScoreChangeResponse (Team3_ScoreText, score, Team3_ProgressBar, Team3_Animator);
		}

	}

	public void OnPaperScoreChange (int score)
	{
		Paper = score;
		if (MyTeamID == 2) {
			ScoreChangeResponse (MyTeam_ScoreText, score, MyTeam_ProgressBar, MyTeam_Animator);
		} else if (MyTeamID == 3) {
			ScoreChangeResponse (Team2_ScoreText, score, Team2_ProgressBar, Team2_Animator);
		} else if (MyTeamID == 1) {
			ScoreChangeResponse (Team3_ScoreText, score, Team3_ProgressBar, Team3_Animator);
		}
	}

	public void OnScissorScoreChange (int score)
	{
		Scissor = score;
		if (MyTeamID == 3) {
			ScoreChangeResponse (MyTeam_ScoreText, score, MyTeam_ProgressBar, MyTeam_Animator);
		} else if (MyTeamID == 1) {
			ScoreChangeResponse (Team2_ScoreText, score, Team2_ProgressBar, Team2_Animator);
		} else if (MyTeamID == 2) {
			ScoreChangeResponse (Team3_ScoreText, score, Team3_ProgressBar, Team3_Animator);
		}
	}

	public void OnGameOver (bool over)
	{
		isGameOver = over;
		GameHUD.Instance.GameOver (over);
		if (over) {
			
			AllPlayers = FindObjectsOfType (typeof(SyncPlayer)) as SyncPlayer[];
			List<SyncPlayer> firstTeam = new List<SyncPlayer> ();
			List<SyncPlayer> secondTeam = new List<SyncPlayer> ();
			List<SyncPlayer> thirdTeam = new List<SyncPlayer> ();

			int firstTeamID = 0;
			int secondTeamID = 0;
			int thirdTeamID = 0;
			int firstTeamScore = 0;
			int secondTeamScore = 0;
			int thirdTeamScore = 0;

			Dictionary<int, int> scores = new Dictionary<int, int> ();
			scores.Add (1, Rock);
			scores.Add (2, Paper);
			scores.Add (3, Scissor);

			Dictionary<int, int> top3 = scores.OrderByDescending (pair => pair.Value).Take (3)
				.ToDictionary (pair => pair.Key, pair => pair.Value);
			
			foreach (KeyValuePair<int, int> a in top3) {
				if (firstTeamID == 0) {
					firstTeamID = a.Key;
					firstTeamScore = a.Value;
					continue;
				}
				if (secondTeamID == 0) {
					secondTeamID = a.Key;
					secondTeamScore = a.Value;
					continue;
				}
				if (thirdTeamID == 0) {
					thirdTeamID = a.Key;
					thirdTeamScore = a.Value;
					continue;
				}
			}

			foreach (SyncPlayer p in AllPlayers) {
				if (p.TeamID == firstTeamID)
					firstTeam.Add (p);
				if (p.TeamID == secondTeamID)
					secondTeam.Add (p);
				if (p.TeamID == thirdTeamID)
					thirdTeam.Add (p);
			}

			Debug.Log ("First Team ID" + firstTeamID);
			GameOverHUD.Instance.Init (firstTeam, secondTeam, thirdTeam);
			GameOverHUD.Instance.Init (firstTeamScore, secondTeamScore, thirdTeamScore);
			GameOverHUD.Instance.VictoryImage (firstTeamID);
			    
		}
	}




	public void Scoreboard_Update (int inc_teamID, int dec_teamID)
	{
		Scoreboard_Update (inc_teamID, true);
		Scoreboard_Update (dec_teamID, false);
	}

	public void Scoreboard_Update (int teamID, bool increment)
	{
		switch (teamID) {
		case 1:
			int x = Rock;
			x += increment ? IncrementValue : DecrementValue;
			Rock = x < 0 ? 0 : x;
			break;

		case 2:
			int y = Paper;
			y += increment ? IncrementValue : DecrementValue;
			Paper = y < 0 ? 0 : y;
			break;

		case 3:
			int z = Scissor;
			z += increment ? IncrementValue : DecrementValue;
			Scissor = z < 0 ? 0 : z;
			break;
		}
		if (Rock >= ScoreLimit || Paper >= ScoreLimit || Scissor >= ScoreLimit) {
			StartCoroutine ("GameOver");
		}

	}

	IEnumerator GameOver ()
	{
		yield return new WaitForSeconds (0.5f);
		isGameOver = true;
	}

	//	void Update ()
	//	{
	//		if (!isGameOver) {
	//			//Scoreboard_Update ();
	//
	//			gameTime -= Time.deltaTime;
	//			TimeSpan t = TimeSpan.FromSeconds (gameTime);
	//			string mins = t.Minutes + "";
	//			mins = mins.Length == 1 ? "0" + mins : mins;
	//
	//			string seconds = t.Seconds + "";
	//			seconds = seconds.Length == 1 ? "0" + seconds : seconds;
	//
	//
	//			Timer.text = mins + ":" + seconds;
	//
	//
	//			if (gameTime <= 0) {
	//				if (isServer)
	//					StartCoroutine ("GameOver");
	//			}
	//		}
	//
	//	}

	// Update is called once per frame
	void Update ()
	{
		if (!isGameOver) {
			//Scoreboard_Update ();

			gameTime -= Time.deltaTime;
			TimeSpan t = TimeSpan.FromSeconds (gameTime);
			string mins = t.Minutes + "";
			mins = mins.Length == 1 ? "0" + mins : mins;

			string seconds = t.Seconds + "";
			seconds = seconds.Length == 1 ? "0" + seconds : seconds;


			Timer.text = mins + ":" + seconds;


			if (gameTime <= 0) {
				if (isServer)
					StartCoroutine ("GameOver");
			}
		} 
			
	}
}

//public class PlayerInfo
//{
//	public string Name;
//	public
//}
