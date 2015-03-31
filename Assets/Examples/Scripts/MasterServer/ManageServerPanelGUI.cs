using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FIcontent.Gaming.Enabler.GameSynchronization;

public class ManageServerPanelGUI :HideableGUI
{

    public RectTransform serverListPanel;
    public GameObject serverButtonPrefab;
    public InputField serverNameInputField;

    public string serverName
    {
        get
        {
            string serverName = Network.player.ipAddress;

            if (serverNameInputField && !string.IsNullOrEmpty(serverNameInputField.text))
                serverName = serverNameInputField.text;

            return serverName;
        }
    }

    public void ClearServerList()
    {
        foreach (RectTransform t in serverListPanel)
        {
            Destroy(t.gameObject);
        }
    }

    public void AddServerToList(HostData hostData)
    {
        var newButton = Instantiate(serverButtonPrefab) as GameObject;
        newButton.GetComponentInChildren<Text>().text = hostData.gameName;
        newButton.GetComponent<RectTransform>().SetParent(serverListPanel);
        newButton.GetComponent<ServerButton>().hostData = hostData;
    }
}
