/***********************************************************************************************
 * CustomNetworkManager.cs
 * 	-This script extends the functionality of the basic NetworkLobbyManager
 * 	-Handles the network discovery calls
 * 	-Handles User interactions with the net manager
 *  -Handles transferring data from lobby to game
 * *********************************************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CustomNetworkManager : NetworkLobbyManager
{
	
	#region Variables

	public static CustomNetworkManager s_Singleton;
	public CustomNetworkDiscovery discovery;
	public GameObject localPlayer;

	public GameObject lobbySlotHolder {
		get {
			return MainMenuUI.Instance.lobbySlotHolder;
		}
	}

	public GameObject serverListHolder {
		get {
			return MainMenuUI.Instance.serverListHolder;
		}
	}

 
	public GameObject lobbyHolder_TeamPaper {
		get {
			return MainMenuUI.Instance.lobbyHolder_TeamPaper;
		}
	}

	public GameObject lobbyHolder_TeamRock {
		get {
			return MainMenuUI.Instance.lobbyHolder_TeamRock;
		}
	}

	public GameObject lobbyHolder_TeamScissors {
		get {
			return MainMenuUI.Instance.lobbyHolder_TeamScissors;
		}
	}

	public Dictionary<int, GameObject> currentPlayers = new Dictionary<int, GameObject> ();
	public Dictionary<int, CustomLobbyPlayer> currentLobbyPlayers = new Dictionary<int, CustomLobbyPlayer> ();
	public Dictionary<int, List<Transform>> TeamSpawnPoints = new Dictionary<int, List<Transform>> ();
	public ServerResponse currentServerInfo;

	#endregion

	#region Initialization

	// Use this for initialization
	void Start ()
	{
		if (s_Singleton != null) {
			Destroy (gameObject);
			return;
		} else
			s_Singleton = this;

		//GetRefs();
	}

	void OnLevelWasLoaded (int level)
	{
		if (level == 0) {
			GetRefs ();
		}
	}

	#endregion

	#region Discovery

	public override void OnStartHost ()
	{

		base.OnStartHost ();
		// Use this override to initialize and broadcast your game through NetworkDiscovery
		discovery.Initialize ();
		discovery.StartAsServer ();
	}

	public void StartListening ()
	{
		if (!discovery.running) {
			// Use this method to start listening for a local game
			discovery.Initialize ();
			discovery.StartAsClient ();
		}
	}

	public void StopListening ()
	{
		if (discovery.running)
			// Use this method to stop listening for a local game
			discovery.StopBroadcast ();
	}

	#endregion

	#region Overrides to handle lobby -> game data hand-off

	public void LoadMap ()
	{
		
		ServerChangeScene (playScene);
	}


	public override GameObject OnLobbyServerCreateLobbyPlayer (NetworkConnection conn, short playerControllerId)
	{
		//original script
		GameObject obj = Instantiate (lobbyPlayerPrefab.gameObject) as GameObject;
		//original script
		CustomLobbyPlayer newPlayer = obj.GetComponent<CustomLobbyPlayer> (); 
		//original script
           
 
		//my added code
		if (!currentLobbyPlayers.ContainsKey (conn.connectionId)) {    /*takes connectionsID into the Dictionary and the LobbyPlayer 
                 to have correspondence between the 2 and be able to distinguish which player has selected which character*/
			currentLobbyPlayers.Add (conn.connectionId, newPlayer);
                   
		}
		return obj;
		//original code goes on
	}


	public override GameObject OnLobbyServerCreateGamePlayer (NetworkConnection conn, short playerControllerId)
	{
		CustomLobbyPlayer newPlayer = currentLobbyPlayers [conn.connectionId];
		GameObject temp = null;
	
		if (TeamSpawnPoints.Count <= 0) {
			foreach (Transform t in startPositions) {
				if (t.gameObject.tag == "RockSpawn") {
			
					if (TeamSpawnPoints.ContainsKey (1)) {
						List<Transform> points = TeamSpawnPoints [1];
						points.Add (t);
						TeamSpawnPoints [1] = points;
					} else {
						List<Transform> points = new List<Transform> ();
						points.Add (t);
						TeamSpawnPoints.Add (1, points);

					}
				}

				if (t.gameObject.tag == "PaperSpawn") {
					if (TeamSpawnPoints.ContainsKey (2)) {
						List<Transform> points = TeamSpawnPoints [2];
						points.Add (t);
						TeamSpawnPoints [2] = points;
					} else {
						List<Transform> points = new List<Transform> ();
						points.Add (t);
						TeamSpawnPoints.Add (2, points);
					
					}
				}

				if (t.gameObject.tag == "ScissorSpawn") {
					if (TeamSpawnPoints.ContainsKey (3)) {
						List<Transform> points = TeamSpawnPoints [3];
						points.Add (t);
						TeamSpawnPoints [3] = points;
					} else {
						List<Transform> points = new List<Transform> ();
						points.Add (t);
						TeamSpawnPoints.Add (3, points);

					}
				}
			}
		}

		/*I probably don't need this second dictionary but before this try the code was in a different function therefore I needed it and for simplicity I added as it was, I'm pretty sure this is not a problem thou*/
		if (!currentPlayers.ContainsKey (conn.connectionId)) {    //takes connectionsID into the Dictionary and the chosen PC from LobbyPlayer
			
			GameObject g = CharacterManager.Instance.ReturnCharacter (newPlayer.lobbyHelper.TeamID, newPlayer.lobbyHelper.ModelID);


			Vector3 pos = TeamSpawnPoints [newPlayer.lobbyHelper.TeamID] [newPlayer.lobbyHelper.ModelID].position;

			//ClientScene.RegisterPrefab(g);
			temp = (GameObject)GameObject.Instantiate (g,
				pos,
				Quaternion.identity);
			if (temp != null) {
				currentPlayers.Add (conn.connectionId, temp);
			}
			temp.GetComponent<SyncPlayer> ().Name = newPlayer.lobbyHelper.playerName;
			temp.GetComponent<SyncPlayer> ().TeamID = newPlayer.lobbyHelper.TeamID;
			temp.GetComponent<SyncPlayer> ().ModelID = newPlayer.lobbyHelper.ModelID;
			temp.name = newPlayer.lobbyHelper.playerName;
                 

		}
 
		NetworkServer.AddPlayerForConnection (conn, temp, playerControllerId);
 
		return temp;
	}



	public override void OnStopHost ()
	{

		Debug.Log ("The host was disconnected");
		Network.Disconnect ();
	}

	public override void OnLobbyClientDisconnect (NetworkConnection conn)
	{
		if (GameObject.FindGameObjectWithTag ("SceneCamera") != null)
			GameObject.FindGameObjectWithTag ("SceneCamera").GetComponent<Camera> ().enabled = true;
	}

	#endregion




	#region GUI Hooks

	void GetRefs ()
	{
		// lobbySlotHolder = GameObject.Find("holder_Lobby");
		//   serverListHolder = GameObject.Find("holder_ServerList");	
	}

	public void TryHost ()
	{
		networkSceneName = "";
		NetworkServer.SetAllClientsNotReady ();
		ClientScene.DestroyAllClientObjects ();
		CustomNetworkManager.s_Singleton.StartHost ();
	}

	public void TryClient ()
	{		
		
		serverListHolder.SetActive (false);
		networkSceneName = "";
		NetworkServer.SetAllClientsNotReady ();
		ClientScene.DestroyAllClientObjects ();
		CustomNetworkManager.s_Singleton.StartClient ();
	}

	public void LeaveLobby ()
	{
		networkSceneName = "";
		discovery.StopBroadcast ();
		CustomNetworkManager.s_Singleton.StopClient ();
		CustomNetworkManager.s_Singleton.StopHost ();
	}

	public void LeaveGame ()
	{
		networkSceneName = "";
		CustomNetworkManager.s_Singleton.StopClient ();
		CustomNetworkManager.s_Singleton.StopHost ();
	}

	public void DestroyList ()
	{
		foreach (Transform t in lobbySlotHolder.transform) {
			Destroy (t.gameObject);
		}
		foreach (Transform t in serverListHolder.transform) {
			Destroy (t.gameObject);
		}
	}

	#endregion


	void Update ()
	{
      
	}
}
