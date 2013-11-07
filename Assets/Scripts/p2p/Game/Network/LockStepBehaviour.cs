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

/// <summary>
/// This class shows hot to use the Peer2Peer Lockstep with Unity.
/// The monobehaviour instantiates and calls the Simulation and Peer Manager methods from Start() and Update().
/// Uses SimManager and PeerManager classes to extend the lockstep model.
/// </summary>
public class LockStepBehaviour : MonoBehaviour
{
    private PeerManager peerMan;
    private SimManager simMan = new SimManager();
    
    public bool SimStarted { get { return simMan.SimStarted; } }

    /// <summary>
    /// Prints some debug infos about the peer
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("player: " + peerMan.myPlayerID);
        GUILayout.Label("snap: " + simMan.SimSnap);
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Instantiates and initializes the PeerManager and SimManager class.
    /// Subscribes to the SimManager events
    /// </summary>
    void Awake()
    {
        peerMan = new PeerManager();

        simMan.CheckSum = new TextCheckSum();
        simMan.PeerMan = peerMan;
        simMan.CheckSumEvent += simMan_CheckSumEvent;
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
    /// Enables showing the network traffic logs in the console
    /// </summary>
    [ContextMenu("Trace network activity")]
    void SwitchTraceNet()
    {
        this.peerMan.traceNetActivity = !this.peerMan.traceNetActivity;
    }

    /// <summary>
    /// Adds an action to the snapshot action queue
    /// </summary>
    /// <param name="action">action to be added</param>
    public void AddAction(IAction action)
    {
        peerMan.AddAction(simMan.SimSnap, peerMan.myPlayerID, action);
    }

    /// <summary>
    /// Checksum Event
    /// Called each time a checksum is calculated
    /// </summary>
    /// <param name="checksum">calculated checksum string</param>
    /// <param name="simSnap">snapshot of the checksum</param>
    void simMan_CheckSumEvent(string checksum, uint simSnap)
    {
        //adds the checksum to the player checksum dictionary
        simMan.map.Add(simSnap, checksum);

        //broadcasts a checksum action
        //the other peers will check with their respective checksum for this snapshot when receiving this packet
        peerMan.AddAction(simSnap, peerMan.myPlayerID, new CheckSumAction(checksum));
    }
}
