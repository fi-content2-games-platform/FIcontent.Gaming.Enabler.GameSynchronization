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

public class LockStepBehaviour : MonoBehaviour
{
    private PeerManager peerMan;
    private SimManager simMan = new SimManager();

    public static LockStepBehaviour instance;

    public int myPlayerID
    {
        get { return peerMan.myPlayerID; }
    }
    public Dictionary<int, string> pidMap
    {
        get { return peerMan.PIDMap; }
        set { peerMan.PIDMap = value; }
    }

    public bool SimStarted { get { return simMan.SimStarted; } }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("player: " + peerMan.myPlayerID);
        GUILayout.Label("snap: " + simMan.SimSnap);
        GUILayout.EndVertical();
    }

    void Awake()
    {
        peerMan = new PeerManager();
        
        simMan.CheckSum = new TextCheckSum();
        simMan.PeerMan = peerMan;
        simMan.CheckSumEvent += simMan_CheckSumEvent;

        instance = this;
    }

    void Start()
    {
        peerMan.Start();
    }

    void Update()
    {
        peerMan.Update();
        simMan.Update();
    }

    void OnApplicationQuit()
    {
        peerMan.OnQuit();
    }

    [ContextMenu("Trace network activity")]
    void SwitchTraceNet()
    {
        this.peerMan.traceNetActivity = !this.peerMan.traceNetActivity;
    }

    public void AddAction(IAction action)
    {
        uint currentSnap = simMan.SimSnap + peerMan.SnapActionDelay;

        peerMan.AddAction(currentSnap, myPlayerID, action);
    }

    void simMan_CheckSumEvent(string checksum, uint simSnap)
    {
       simMan.map.Add(simSnap, checksum);
        peerMan.AddAction(simSnap, peerMan.myPlayerID, new CheckSumAction(checksum));
    }
}
