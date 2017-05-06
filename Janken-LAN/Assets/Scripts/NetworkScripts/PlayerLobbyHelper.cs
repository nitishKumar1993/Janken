/***********************************************************************************************
 * PlayerLobbyHelper.cs
 * 	-This script extends the functionality of our CustomLobbyPlayer
 * 	- Handles communication from the client to server and back to the client
 *  - I use this more for organization
 * *********************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerLobbyHelper : NetworkBehaviour
{

	#region Variables

	[SyncVar (hook = "OnMyTeam")]
	public int TeamID;
	[SyncVar (hook = "OnReady")]
	public bool ReadyStatus;
	[SyncVar (hook = "OnNameChange")] 
	public string playerName;
	[SyncVar (hook = "OnModelChange")] 
	public int ModelID = -1;
	[SyncVar (hook = "StartLoading")] 
	public bool isLoading;

	NetworkLobbyPlayer lobbyPlayer;
	CustomLobbyPlayer playerUI;

	#endregion

	#region Initialization

	// Use this for initialization
	void Awake ()
	{
		ModelID = -1;
		//TeamID = 0;
		lobbyPlayer = GetComponent<NetworkLobbyPlayer> ();
		playerUI = GetComponent<CustomLobbyPlayer> ();
	}

	#endregion

	#region  CLIENT TO SERVER MESSAGE

	[Command]
	void Cmd_OnMyTeam (int id)
	{
		// if (lobbyPlayer.readyToBegin)
		//      return;
		// Make sure we're a valid team yo
		TeamID = id;
	}

	[Command]
	void Cmd_OnModelChange (int id)
	{
		// if (lobbyPlayer.readyToBegin)
		//      return;
		// Make sure we're a valid team yo
		ModelID = id;
	}

	[Command]
	void Cmd_OnReady (bool ready)
	{
		ReadyStatus = ready;
	}

	[Command]
	void Cmd_ChangeName (string s)
	{
		playerName = s;
		gameObject.name = s;
      
	}

	[Command]
	void Cmd_StartLoading (bool s)
	{
		isLoading = s;
	}


	#endregion

	#region  CLIENT CALLS

	public void ClientChangedTeam (int id)
	{
		Cmd_OnMyTeam (id);
	}

	public void ClientChangedModel (int id)
	{
		Cmd_OnModelChange (id);
	}

	public void ClientStartLoading (bool b)
	{
		Cmd_StartLoading (b);
	}



	public void ClientReady (bool ready)
	{
		Cmd_OnReady (ready);
	}

	public void UpdateLocalPlayerName ()
	{
		playerName = MainMenuUI.Instance.PlayerName;
		Cmd_ChangeName (playerName);
	}

	//    public void ComputePlayerName()
	//    {
	//        //   var _name = "" + GetComponent<NetworkIdentity>().netId;
	//        //Cmd_ChangeName(_name);
	//        if (isLocalPlayer)
	//        {
	//            playerName = GameUI.Instance.PlayerName;
	//            Cmd_ChangeName(playerName);
	//        }
	//
	//
	////        if (Application.isEditor)
	////        {
	////            var _name = "EditorPlayer [" + GetComponent<NetworkIdentity>().netId + "]";
	////            Cmd_ChangeName(_name);
	////            if (isLocalPlayer && isServer)
	////            {
	////                playerName = _name;
	////            }
	////        } else
	////        {
	////            var _name = "Player [" + GetComponent<NetworkIdentity>().netId + "]";
	////            Cmd_ChangeName(_name);
	////            if (isLocalPlayer && isServer)
	////            {
	////                playerName = _name;
	////            }
	////        }
	//
	//    }

	#endregion

    #region   SERVER CALL BACKS

    void OnMyTeam (int _team)
	{
		TeamID = _team;
		playerUI.SetTeam ();
	}

	void OnReady (bool ready)
	{
		ReadyStatus = ready;
		playerUI.SetReady (ready);
	}

	void OnNameChange (string s)
	{
		playerName = s;
		gameObject.name = s;
		playerUI.lobbyObject.GetComponent<LobbySlot> ().playerName.text = s;
	}

	void OnModelChange (int s)
	{
		ModelID = s;
		if (s != -1)
			playerUI.SetPlayerPrefab ();
	}

	void StartLoading (bool b)
	{
		isLoading = b;
		playerUI.StartLoading ();
	}

	#endregion

}
