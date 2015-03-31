using UnityEngine;
using System.Collections;
using FIcontent.Gaming.Enabler.GameSynchronization;

public class ServerButton : MonoBehaviour {

    public HostData hostData;

    public void Connect()
    {
        Debug.Log(hostData);
        FindObjectOfType<LockstepPeer>().ConnectToServer(this.hostData);
    }
}
