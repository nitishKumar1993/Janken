/***************************************************************************
     * ServerList.cs
     * Script used to manage the server list container
     * 
     **************************************************************************/

using UnityEngine;
using System.Collections;

public class ServerList : MonoBehaviour
{
	
    #region Variables

    // Holder for the servers
    public RectTransform listHolder;
    // prefab for the game info
    public GameObject serverInfoPrefab;

    #endregion

    #region Initialization

    void OnEnable()
    {
        // Subscribe to our Custom Network Discovery event when a server is found
        CustomNetworkManager.s_Singleton.discovery.OnNewServerFound += OnGUIServerFound;
        // We want to clear the list when this is enabled
        foreach (RectTransform rt in listHolder)
            Destroy(rt.gameObject);
    }

    void OnDisable()
    {
        // Un-Subscribe from our Custom Network Discovery event
        CustomNetworkManager.s_Singleton.discovery.OnNewServerFound -= OnGUIServerFound;
    }

    #endregion

    #region Event Handler

    void OnGUIServerFound()
    {
        // Loop through the servers found and instantiate a serverInfo panel for new servers
        for (int s = 0; s < CustomNetworkManager.s_Singleton.discovery.serverList.Count; s++)
        {
            // If the serverInfo hasn't been created/shown yet, then lets do that
            if (!CustomNetworkManager.s_Singleton.discovery.serverList [s].visible)
            {
                // Instantiate the serverInfo Prefab
                var _serverInfo = Instantiate(serverInfoPrefab);
                // Pass the server info ref to the server info panel
                _serverInfo.GetComponent<ServerInfo>().SetServerInfo(CustomNetworkManager.s_Singleton.discovery.serverList [s]);
                // Set the parent to our list holder
                _serverInfo.transform.SetParent(listHolder.transform, false);
                // Set the server info to being visible so it won't be re-created
                CustomNetworkManager.s_Singleton.discovery.serverList [s].visible = true;
            }
        }			
    }

    #endregion
}
