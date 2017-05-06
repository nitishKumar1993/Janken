/***************************************************************************
     * ServerInfo.cs
     * Script used to populate the Server Info prefab when a server is found
     * 
     **************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerInfo : MonoBehaviour
{

	#region Variables

	// UI References
	public Text GameName;
	public Text Type;
	public Text Map;
	// Cached reference to this server info
	ServerResponse serverInfo;

	#endregion

	#region Public Functions

	public void SetServerInfo (ServerResponse _resp)
	{
		// Cache the server info
		serverInfo = _resp;
		// Set the UI up to show server info name
		GameName.text = serverInfo.serverName;
		// Set the UI up to show server info IP
		Type.text = serverInfo.GameType;
		Map.text = serverInfo.MapName;
	}

	public void JoinGame ()
	{
		MainMenuUI.Instance.selectedServerIP = serverInfo.serverIP;
		MainMenuUI.Instance.selectedPort = serverInfo.serverPort;
		MainMenuUI.Instance.str_GameType = serverInfo.GameType;
		CustomNetworkManager.s_Singleton.currentServerInfo = serverInfo;
//        // Set the networkaddress we want to use to connect to
//        CustomNetworkManager.s_Singleton.networkAddress = serverInfo.serverIP;
//        // Call our custom Attempt to connect as client method
//        CustomNetworkManager.s_Singleton.TryClient();
	}

	#endregion
}
