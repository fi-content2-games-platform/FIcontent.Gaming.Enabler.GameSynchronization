/* Copyright (c) 2013 Disney Research Zurich and ETH Zurich
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

using System.Collections.Generic;
using com.ficontent.gws.Peer2Peer.CheckSum;
using UnityEngine;
using com.ficontent.gws.Peer2Peer.Packets.Actions;
using com.ficontent.gws.Peer2Peer.Packets;

/// <summary>
/// This class shows hot to use the Peer2Peer Lockstep with Unity.
/// The monobehaviour instantiates and calls the Simulation and Peer Manager methods from Start() and Update().
/// 
/// Uses H2PSimManager and H2PPeerManager classes to extend the lockstep model.
/// At each simulation step the host peer sends around the object transform.
/// 
/// Implements ITestInfos to send debug information about the peer to the TestGUI behaviour.
/// </summary>
public class H2PLockStepBehaviour : MonoBehaviour, ITestInfos
{
    private H2PPeerManager peerMan;
    private H2PSimManager simMan = new H2PSimManager();

    /// <summary>
    /// The Peer object to be moved around by the host
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// Used to initialize the peers player ID
    /// </summary>
    public int myPID;
       
    public int MyPlayerID
    {
        get { return peerMan.myPlayerID; }
    }
    private string otherInfos = string.Empty;
    public string OtherInfos
    {
        get { return otherInfos; }
    }
    public uint SimSnap
    {
        get { return simMan.SimSnap; }
    }

    /// <summary>
    /// Instantiates and initializes the PeerManager and SimManager class.
    /// Subscribes to the SimManager events
    /// </summary>
    void Awake()
    {
        peerMan = new H2PPeerManager(myPID);
        
        simMan.CheckSumEvent += simMan_CheckSumEvent;
        simMan.SimulationStartEvent += simMan_SimStarted;
        simMan.SimulationStepEvent += simMan_SuccesfulSimulation;
        simMan.CheckSum = new TextCheckSum();
        simMan.PeerMan = peerMan;

        //creates the gameobject for the example
        //the peer clients will synchronize their object with the transform of the host object.
        var go = Instantiate(prefab) as GameObject;
        go.name = "test object " + (peerMan.isHost ? "host" : "client") + " " + myPID;
        simMan.objects.Add(go.transform);
    }

    /// <summary>
    /// Checksum Event
    /// Called each time a checksum is calculated
    /// </summary>
    /// <param name="checksum">calculated checksum string</param>
    /// <param name="simSnap">snapshot of the checksum</param>
    void simMan_CheckSumEvent(string checksum, uint simSnap)
    {
        simMan.map.Add(simSnap, checksum);
        peerMan.AddAction(simSnap, peerMan.myPlayerID, new CheckSumAction(checksum));
    }

    /// <summary>
    /// Simulation started event.
    /// Called the first time a packet from each peer is received
    /// </summary>
    void simMan_SimStarted()
    {
        Debug.Log("Started " + peerMan.myPlayerID);
    }

    /// <summary>
    /// Succesful Simulation Event
    /// Called each time a simulation step is executed
    /// </summary>
    /// <param name="simSnap">simulation snapshot executed</param>
    void simMan_SuccesfulSimulation(uint simSnap)
    {
        //sends the transforms
        SendPosition();

        //updates the debug infos
        otherInfos = simMan.otherInfos;
    }
    
    /// <summary>
    /// Start calls
    /// </summary>
    void Start()
    {
        peerMan.Start();
    }

    /// <summary>
    /// Update calls
    /// </summary>
    void Update()
    {
        peerMan.Update();
        simMan.Update();
    }

    /// <summary>
    /// Network finalization
    /// </summary>
    void OnApplicationQuit()
    {
        peerMan.OnQuit();
    }
    
    /// <summary>
    /// The host peer broadcasts the host object transform
    /// </summary>
    private void SendPosition()
    {
        if (peerMan.isHost)
        {
            TransformsAction t = new TransformsAction();
            t.Add(simMan.objects[0].transform);
            peerMan.BroadCast(t, simMan.SimSnap);
        }
    }
}
