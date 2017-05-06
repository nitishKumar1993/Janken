/***************************************************************************
     * LobbySlot.cs
     * Script for the visual representation of the player in a lobby
     * 
     **************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbySlot : MonoBehaviour
{

	#region Variables / Delegates / Events

	public delegate void LobbyHookBool (bool toggle);

	public delegate void LobbyHook ();

	public LobbyHookBool OnUserReady;
	public LobbyHook OnChangeTeam;
	public LobbyHook OnLeaveLobby;

	[SerializeField] Image playerAvatar;
	[SerializeField] Text lobbyStatus;

	public Text playerName;
	string status;
	public bool ready;
	public Image image;

	#endregion

	#region Server Callbacks

	public void SetLobbyStatus (string _status)
	{
		status = _status;
		lobbyStatus.text = status;
	}

	public void SetTeam (Color _teamColor)
	{
		playerAvatar.color = _teamColor;
	}

	#endregion

	#region Player Events

	public void ChangeTeam ()
	{
		// pass event to any observers
		if (OnChangeTeam != null)
			OnChangeTeam.Invoke ();
	}

	public void Lock ()
	{
		// pass event to any observers
		if (OnUserReady != null)
			OnUserReady.Invoke (ready);
	}

	public void Leave ()
	{
    
		// pass event to any observers
		if (OnLeaveLobby != null)
			OnLeaveLobby.Invoke ();
	}

	#endregion
}
