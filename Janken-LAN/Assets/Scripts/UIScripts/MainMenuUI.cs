using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
	public GameObject MainMenu;
	public GameObject HostLobby;
	public GameObject JoinLobby;
	public GameObject Lobby;
	public GameObject HeadStart;
	public GameObject LAN;
	public GameObject CharacterLoadingScreen;

	public Button Btn_LobbyToLoadingScreen;
	public Button Btn_StartGame;
	public Button Btn_JoinRocks;
	public Button Btn_JoinPapers;
	public Button Btn_JoinScissors;
	public GameObject lobbySlotHolder;
	public GameObject serverListHolder;

	public GameObject lobbyHolder_TeamPaper;
	public GameObject lobbyHolder_TeamRock;
	public GameObject lobbyHolder_TeamScissors;

	public GameObject loadingScreenHolder_TeamPaper;
	public GameObject loadingScreenHolder_TeamRock;
	public GameObject loadingScreenHolder_TeamScissors;

	public InputField _playerName;

	public string PlayerName {
		get {
			return  _playerName.text;
		}
		set {
			_playerName.text = value;
		}
	}

	public Dropdown _map;

	public string Map {
		get {
			return _map.options [_map.value].text;
		}
	}

	public Dropdown _scoreLimit;

	public int ScoreLimit {
		get {
			return int.Parse (_scoreLimit.options [_scoreLimit.value].text.Substring (8, 3));
		}
	}

	//    public enum _GameType
	//    {
	//        One,
	//        Two,
	//        Three
	//    }
	//
	//    public _GameType GameType;
	public string str_GameType = "1v1v1";

	public int int_GameType {
		get {
			return int.Parse (str_GameType.Substring (0, 1));
		}
	}

	public int MaxPlayersPerTeam {
		get {
			return int.Parse (str_GameType.Substring (0, 1));
		}
	}

	public int MaxPlayers {
		get {
			return MaxPlayersPerTeam * 3;
		}
	}

	public int PlayersInLobby {
		get {
			return lobbySlotHolder.transform.childCount;
		}
	}


	[SerializeField] 
	GameObject ServerList;
	// public InputField ServerName;
	string broadcastData;
	[HideInInspector]
	public string selectedServerIP;
	public string selectedPort;

	public delegate void GameUIHook ();

	public GameUIHook OnExitGame;

	public static MainMenuUI Instance;
	bool loadingScreenCountDown;
	public float loadingScreenTime = 6.0f;
	public bool testing;


	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		MainMenu.SetActive (true);
		HostLobby.SetActive (false);
		JoinLobby.SetActive (false);
		CharacterLoadingScreen.SetActive (false);
		Lobby.SetActive (false);
		HeadStart.SetActive (false);

		if (PlayerPrefs.HasKey ("PlayerName")) {
			PlayerName = PlayerPrefs.GetString ("PlayerName");
		} else {
			PlayerPrefs.SetString ("PlayerName", "Player Name");
			PlayerName = PlayerPrefs.GetString ("PlayerName");
		}
		_playerName.onEndEdit.AddListener ((string str) => {
			PlayerPrefs.SetString ("PlayerName", str);
		});
	}

	public bool AreTeamsFull ()
	{
		if (testing)
			return true;
		if (lobbyHolder_TeamPaper.transform.childCount == MaxPlayersPerTeam && lobbyHolder_TeamRock.transform.childCount == MaxPlayersPerTeam && lobbyHolder_TeamScissors.transform.childCount == MaxPlayersPerTeam)
			return true;
		return false;
	}

	public bool IsLobbyFull ()
	{
		if (PlayersInLobby == MaxPlayers) {
			CustomNetworkManager.s_Singleton.StopListening ();
			return true;
		} else {
			ResumeBroadCast ();
		}
		return false;

	}

	public void EnableLAN_Menu ()
	{
		LAN.SetActive (false);
	}

	public void MainMenuToHostLobby ()
	{
		MainMenu.SetActive (false);
		HostLobby.SetActive (true);
		LAN.SetActive (true);
	}

	public void LobbyToLoadingScreen ()
	{
		Lobby.SetActive (false);
		CharacterLoadingScreen.SetActive (true);
		//Btn_StartGame.interactable = false;
		loadingScreenCountDown = true;
	}

	public void HostLobbyToMainMenu ()
	{
		MainMenu.SetActive (true);
		HostLobby.SetActive (false);
		CustomNetworkDiscovery.s_Singleton.serverList.Clear ();
	}

	public void MainMenuToHeadStart ()
	{
		MainMenu.SetActive (false);
		HeadStart.SetActive (true);
	}

	public void HeadStartToMainMenu ()
	{
		HeadStart.SetActive (false);
		MainMenu.SetActive (true);
	}

	public void MainMenuToJoinLobby ()
	{
		MainMenu.SetActive (false);
		JoinLobby.SetActive (true);
		LAN.SetActive (true);
		GUIFindGame ();
	}

	public void JoinLobbyToMainMenu ()
	{
		MainMenu.SetActive (true);
		JoinLobby.SetActive (false);

		CustomNetworkDiscovery.s_Singleton.serverList.Clear ();
		CustomNetworkManager.s_Singleton.StopListening ();
	}

	public void LobbyToMainMenu ()
	{
		MainMenu.SetActive (true);
		Lobby.SetActive (false);
		CustomNetworkDiscovery.s_Singleton.serverList.Clear ();
		CustomNetworkManager.s_Singleton.LeaveLobby ();
	}

	public void JoinLobbyToLobby ()
	{
		if (!string.IsNullOrEmpty (selectedServerIP)) {
			// Set the networkaddress we want to use to connect to
			CustomNetworkManager.s_Singleton.networkAddress = selectedServerIP;
			CustomNetworkManager.s_Singleton.networkPort = System.Int32.Parse (selectedPort);
			// Call our custom Attempt to connect as client method
			CustomNetworkManager.s_Singleton.TryClient ();

			JoinLobby.SetActive (false);
			Lobby.SetActive (true);
			Btn_LobbyToLoadingScreen.interactable = false;
			CustomNetworkManager.s_Singleton.StopListening ();
		}
	}

	public void ChangeGameType (string t)
	{
		str_GameType = t + "v" + t + "v" + t;
	}

	public void Exit ()
	{
		Application.Quit ();
		//        if (OnExitGame != null)
		//            OnExitGame.Invoke();
	}


	void Update ()
	{
		if (loadingScreenCountDown) {
			loadingScreenTime -= Time.deltaTime;
			Btn_StartGame.transform.FindChild ("Text").GetComponent <Text> ().text = "Starting in " + (int)loadingScreenTime;
			//text.text = "" + (int)timeLeft;
			if (loadingScreenTime < 1) {
				loadingScreenTime = 6;
				loadingScreenCountDown = false;
				CustomNetworkManager.s_Singleton.localPlayer.GetComponent <CustomLobbyPlayer> ().LoadMap ();
			}
		}
	}


	// Called from the Host Game Event Trigger button
	public void GUIHostGame ()
	{
		// check if the network is already active
		if (CustomNetworkManager.s_Singleton.isNetworkActive)
			return;

		// Set the IP the Net Manager is going to use to host a game to OUR IP address and Port 7777
		CustomNetworkManager.s_Singleton.networkAddress = Network.player.ipAddress;
		CustomNetworkManager.s_Singleton.networkPort = Random.Range (1000, 9999);
		CustomNetworkManager.s_Singleton.maxPlayers = int_GameType * 3;

		/*
        // If game name was set...
		if (ServerName.text != "")
			//... get the provided name
			_name = ServerName.text;
		else
		{
			// ELSE set a game name for our user
			_name = "Game:" + Random.Range(0, 10000);
			ServerName.text = _name;
		}
		*/
		broadcastData = PlayerName + "," + str_GameType + "," + Map + "," + ScoreLimit + "," + CustomNetworkManager.s_Singleton.networkPort;

		Debug.Log (broadcastData);
		// This sets the data part of the OnReceivedBroadcast() event 
		CustomNetworkManager.s_Singleton.discovery.broadcastData = broadcastData;
		CustomNetworkManager.s_Singleton.TryHost ();
		CustomNetworkManager.s_Singleton.currentServerInfo = new ServerResponse (broadcastData, Network.player.ipAddress);

		HostLobby.SetActive (false);
		Lobby.SetActive (true);
		Btn_LobbyToLoadingScreen.interactable = false;
	}

	// Called from the Find Game Event Trigger button
	public void GUIFindGame ()
	{
		// check if the network is already active
		if (CustomNetworkManager.s_Singleton.discovery.running)
			return;

		CustomNetworkManager.s_Singleton.StartListening ();
		// Show the serverlist object which will automatically start handling receiving games
		ServerList.SetActive (true);
	}

	void ResumeBroadCast ()
	{
		if (CustomNetworkDiscovery.s_Singleton.isServer) {
			if (!CustomNetworkDiscovery.s_Singleton.running) {
				broadcastData = PlayerName + "," + str_GameType + "," + Map + "," + ScoreLimit + "," + CustomNetworkManager.s_Singleton.networkPort;

				Debug.Log (broadcastData);
				// This sets the data part of the OnReceivedBroadcast() event 
				CustomNetworkManager.s_Singleton.discovery.broadcastData = broadcastData;
				CustomNetworkDiscovery.s_Singleton.Initialize ();
				CustomNetworkDiscovery.s_Singleton.StartAsServer ();
				//Debug.Log(GameUI.Instance.TeamRockListHolder.transform.childCount);
			}
		}
	}
}
