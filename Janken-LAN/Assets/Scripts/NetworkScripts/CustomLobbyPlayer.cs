/***********************************************************************************************
 * CustomLobbyPlayer.cs
 * 	-This script extends the functionality of the basic NetworkLobbyPlayer
 * 	-Handles the creation of the visual lobby player and facilitates changes made by the user to self and other clients
 * 	-Handles custom code for joining / leaving lobby
 * *********************************************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{

	#region Variables

	// This is the prefab we instantiate in the lobby
	[SerializeField] GameObject lobbySlotPrefab;
	// This is the prefab we instantiate in the loading Screen
	[SerializeField] GameObject loadingScreenSlotPrefab;

	// This is the network lobby player object
	NetworkLobbyPlayer lobbyPlayer;
	// This is a class to help communicate changes made in lobby to all clients
	public PlayerLobbyHelper lobbyHelper;
	// This is the object we visually represent a lobby player with
	public GameObject lobbyObject;
	public GameObject teamObject;
	public GameObject loadingScreenObject;

	#endregion

	#region Initialization

	// Use this for initialization
	void Awake ()
	{
		// Cache reference to our lobby player
		lobbyPlayer = GetComponent<NetworkLobbyPlayer> ();
		// Cache reference to our Lobby slot helper
		lobbyHelper = GetComponent<PlayerLobbyHelper> ();
	}

	#endregion

	void Update ()
	{
        
      
	}

	public void Lobby_JoinTeam (Transform teamHolder)
	{

		if (teamObject == null) {
			teamObject = (GameObject)Instantiate (lobbySlotPrefab, Vector3.zero, Quaternion.identity);
		}
		// Instantiate the UI Lobby Object

        
		teamObject.transform.SetParent (teamHolder, false); 
		teamObject.GetComponent<LobbySlot> ().playerName.text = lobbyHelper.playerName;

	}

	public void LoadingScreen_JoinTeam (Transform teamHolder)
	{

		if (loadingScreenObject == null) {
			loadingScreenObject = (GameObject)Instantiate (loadingScreenSlotPrefab, Vector3.zero, Quaternion.identity);
		}
		// Instantiate the UI Lobby Object

        
		loadingScreenObject.transform.SetParent (teamHolder, false); 
		loadingScreenObject.GetComponent<LobbySlot> ().playerName.text = lobbyHelper.playerName;

	}

	#region Overrides for Setting Lobby object

	/// <summary>
	/// Raises the client enter lobby event.
	///  - This event will be called on every object in every client Scene
	///  - For example, if a client joins a Lobby, this will be called once for the joining clients object, and once for every other client object in the lobby
	///  - Use this to setup the other clients lobby objects
	/// </summary>
	public override void OnClientEnterLobby ()
	{
		base.OnClientEnterLobby ();
       


		if (lobbyObject == null) {
           
			// Instantiate the UI Lobby Object
			lobbyObject = (GameObject)Instantiate (lobbySlotPrefab, Vector3.zero, Quaternion.identity);
			// Parent the object within the lobby holder
           
			lobbyObject.transform.SetParent (CustomNetworkManager.s_Singleton.lobbySlotHolder.transform, false);	
			Debug.Log ("OnClientEnterLobby");
		}

      
		// Set the name of the object to the players unique name
		lobbyObject.GetComponent<LobbySlot> ().playerName.text = lobbyHelper.playerName;
   
		// Set the lobby status to not ready
		// lobbyObject.GetComponent<LobbySlot>().SetLobbyStatus("GO");
		// Set the team of the object
		SetTeam ();


	}

	/// <summary>
	/// Raises the start local player event.
	///  - This event will only be called on the local players object
	///  - This is where you would turn on/off things you want to display/hide for local users
	///  - I also direct button clicks to the correct method here
	/// </summary>
	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		Debug.Log ("OnStartLocalPlayer");

		// Set the local user in the NetworkManager
		CustomNetworkManager.s_Singleton.localPlayer = gameObject;
		// Compute name for user
		lobbyHelper.UpdateLocalPlayerName ();
     

		if (lobbyObject == null) {
			//Debug.Log("OnStartLocalPlayer");
			// Instantiate the UI Lobby Object
			lobbyObject = (GameObject)Instantiate (lobbySlotPrefab, Vector3.zero, Quaternion.identity);
			// Parent the object within the lobby holder
			lobbyObject.transform.SetParent (CustomNetworkManager.s_Singleton.lobbySlotHolder.transform, false);	
		}
		lobbyObject.GetComponent<LobbySlot> ().playerName.text = lobbyHelper.playerName;

		// Setup the initial value for the teamID
		//  lobbyHelper.ClientChangedTeam();

		if (isServer) {
			// GameUI.Instance.IsLobbyFull();
			MainMenuUI.Instance.Btn_LobbyToLoadingScreen.transform.FindChild ("Text").GetComponent <Text> ().text = "START";

			MainMenuUI.Instance.Btn_LobbyToLoadingScreen.onClick.AddListener (() => {
               
				foreach (var item in  CustomNetworkManager.s_Singleton.lobbySlots) {
					if (item != null) {
						int modelID = CharacterManager.Instance.SelectUniqueModelID (item.GetComponent<PlayerLobbyHelper> ().TeamID);
						item.GetComponent<PlayerLobbyHelper> ().ClientChangedModel (modelID);
						item.GetComponent<PlayerLobbyHelper> ().ClientStartLoading (true);
					}
				}
				MainMenuUI.Instance.Btn_StartGame.interactable = true;

				CustomNetworkManager.s_Singleton.StopListening ();
    
				//  lobbyObject.GetComponent<LobbySlot>().Lock();
                
			}
			);

			MainMenuUI.Instance.Btn_StartGame.onClick.AddListener (() => {
             
				OnGUIReady (true);
				CustomNetworkManager.s_Singleton.LoadMap ();
			}
			);
		} else {
			MainMenuUI.Instance.Btn_LobbyToLoadingScreen.transform.FindChild ("Text").GetComponent <Text> ().text = "Waiting for Host";
		}

		MainMenuUI.Instance.Btn_JoinRocks.onClick.AddListener (() => {

			//Control Max Players per team
			if (CustomNetworkManager.s_Singleton.lobbyHolder_TeamRock.transform.childCount < MainMenuUI.Instance.MaxPlayersPerTeam) {
           
				OnGUIChangeTeam (1);
			}
		}
		);
		MainMenuUI.Instance.Btn_JoinPapers.onClick.AddListener (() => {
			if (CustomNetworkManager.s_Singleton.lobbyHolder_TeamPaper.transform.childCount < MainMenuUI.Instance.MaxPlayersPerTeam)
				OnGUIChangeTeam (2);
		}
		);
		MainMenuUI.Instance.Btn_JoinScissors.onClick.AddListener (() => {
			if (CustomNetworkManager.s_Singleton.lobbyHolder_TeamScissors.transform.childCount < MainMenuUI.Instance.MaxPlayersPerTeam)
				OnGUIChangeTeam (3);
		}
		);

       
	}

	#endregion

	#region Overrides to handle Lobby closing

	public override void OnClientExitLobby ()
	{

		if (lobbyObject != null) {
			Destroy (lobbyObject.gameObject);
		}
		if (teamObject != null) {
			Destroy (teamObject);
		}
		if (loadingScreenObject != null) {
			Destroy (loadingScreenObject);
		}
		base.OnClientExitLobby ();
	}

	public override void OnNetworkDestroy ()
	{
		if (lobbyObject != null) {
			Destroy (lobbyObject);
		}
		if (teamObject != null) {
			Destroy (teamObject);
		}
		if (loadingScreenObject != null) {
			Destroy (loadingScreenObject);
		}
		base.OnNetworkDestroy ();
	}

	#endregion

	#region LocalUser Lobby Functions

	/// <summary>
	/// Raises the GUI ready event.
	/// </summary>
	public void OnGUIReady (bool toggle)
	{
		// Tell our helper class we're ready
		lobbyHelper.ClientReady (toggle);
	}

	/// <summary>
	/// Raises the GUI leave event.
	/// </summary>
	void OnGUILeave ()
	{
		// Set ready to false before leaving
		lobbyHelper.ClientReady (false);
		// Leave the lobby
		CustomNetworkManager.s_Singleton.LeaveLobby ();
	}

	/// <summary>
	/// Raises the GUI change team event.
	/// </summary>
	void OnGUIChangeTeam (int teamID)
	{
		// spin that ish to our special script yo
		lobbyHelper.ClientChangedTeam (teamID);
	}

	#endregion

	#region Helper

	/// <summary>
	/// Gets the status.
	/// </summary>
	static string GetStatus (bool _val)
	{
		return _val ? "GO" : "GO";
	}

	/// <summary>
	/// Sets the ready.
	/// </summary>
	public void SetReady (bool ready)
	{
		if (isLocalPlayer) {
			// Set this player to ready and send ready message to lobby manager
			readyToBegin = ready;
			// Set bool for our slot object as well
			lobbyObject.GetComponent<LobbySlot> ().ready = readyToBegin;
			//lobbyObject.GetComponent<LobbySlot>().SetLobbyStatus(GetStatus(readyToBegin));
			if (readyToBegin) {
			
				SendReadyToBeginMessage ();
			} else {
				SendNotReadyToBeginMessage ();
			}
		}
	}

	public void StartLoading ()
	{
		if (isLocalPlayer)
			MainMenuUI.Instance.LobbyToLoadingScreen ();
	}

	public void LoadMap ()
	{
		if (isServer) {
			OnGUIReady (true);
			CustomNetworkManager.s_Singleton.LoadMap ();
		}
	}

	public void SetPlayerPrefab ()
	{
		StartCoroutine ("PlayerPortrait");
		//loadingScreenObject.GetComponent<LobbySlot> ().image.sprite = CharacterManager.Instance.ReturnSprite (lobbyHelper.TeamID, lobbyHelper.ModelID);
		// CustomNetworkManager.s_Singleton.gamePlayerPrefab.GetComponent<LocalPlayerSetup>().TeamID = lobbyHelper.TeamID;
	}

	IEnumerator PlayerPortrait ()
	{
		yield return new WaitForSeconds (0.1f);
		loadingScreenObject.GetComponent<LobbySlot> ().image.sprite = CharacterManager.Instance.ReturnSprite_LoadingScreen (lobbyHelper.TeamID, lobbyHelper.ModelID);
	}

	/// <summary>
	/// Sets the team.
	/// </summary>
	/// <param name="_teamID">Team I.</param>
	public void SetTeam ()
	{
       
		//  Set the Team Color based on ID
		switch (lobbyHelper.TeamID) {
		case 1: 
			Lobby_JoinTeam (CustomNetworkManager.s_Singleton.lobbyHolder_TeamRock.transform);
			LoadingScreen_JoinTeam (MainMenuUI.Instance.loadingScreenHolder_TeamRock.transform);
			break;
		case 2: 
			Lobby_JoinTeam (CustomNetworkManager.s_Singleton.lobbyHolder_TeamPaper.transform);
			LoadingScreen_JoinTeam (MainMenuUI.Instance.loadingScreenHolder_TeamPaper.transform);
			break;
		case 3: 
			Lobby_JoinTeam (CustomNetworkManager.s_Singleton.lobbyHolder_TeamScissors.transform);
			LoadingScreen_JoinTeam (MainMenuUI.Instance.loadingScreenHolder_TeamScissors.transform);
			break;
		}
		if (isServer) {

			if (MainMenuUI.Instance.AreTeamsFull ()) {
				MainMenuUI.Instance.Btn_LobbyToLoadingScreen.interactable = true;
			
			} else {
				MainMenuUI.Instance.Btn_LobbyToLoadingScreen.interactable = false;

			}

		}

	}


	#endregion

}
