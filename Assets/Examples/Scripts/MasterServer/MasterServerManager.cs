using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FIcontent.Gaming.Enabler.GameSynchronization;

public class MasterServerManager : MonoBehaviour
{

    public string gameTypeName = "test_gamesync";
    private HostData[] hostData;
    public ManageServerPanelGUI manageServerGUI;
    public ChatGUI chatGUI;
    public StartButtonGUI startButton;

    public void PublishServer()
    {
        FindObjectOfType<LockstepPeer>().CreateServer();
    }


    /// <summary>
    /// When receiving the hostlist the server list is refreshed
    /// </summary>
    /// <param name="mse">Mse.</param>
    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.HostListReceived)
        {
            if (MasterServer.PollHostList().Length != 0)
            {
                hostData = MasterServer.PollHostList();
            }

            if (hostData == null)
                return;

            manageServerGUI.ClearServerList();

            int i = 0;
            while (i < hostData.Length)
            {
                manageServerGUI.AddServerToList(hostData [i]);
                i++;
            }
        }
    }

    public void RefreshList()
    {
        MasterServer.RequestHostList(this.gameTypeName);
    }

#region Unity monobehaviour methods

    void Start()
    {
        manageServerGUI.Show(true);
        chatGUI.Show(false);
        startButton.Show(false);
    }

    void OnConnectedToServer()
    {
        manageServerGUI.Show(false);
        chatGUI.Show(true);
    }
    
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        manageServerGUI.Show(true);
        chatGUI.Show(false);
    }

    /// <summary>
    /// Registers the newly created server to the MasterServer
    /// </summary>
    void OnServerInitialized()
    {
        MasterServer.UnregisterHost();
        MasterServer.RegisterHost(this.gameTypeName, manageServerGUI.serverName);

        manageServerGUI.Show(false);
        chatGUI.Show(true);
        startButton.Show(true);
    }

#endregion
}
