/* Copyright (c) 2013 ETH Zurich
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
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
