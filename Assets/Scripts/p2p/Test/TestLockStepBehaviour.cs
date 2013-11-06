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
using com.ficontent.gws.Peer2Peer.Packets.Actions;
using UnityEngine;

public class TestLockStepBehaviour : MonoBehaviour, ITestInfos
{
    private TestPeerManager peerMan;
    private SimManager simMan = new SimManager();

    public static TestLockStepBehaviour instance;

    public int myPID;
    public int playerID
    {
        get { return myPID; }        
    }
    public uint SimSnap { get { return simMan.SimSnap; } }

    public string OtherInfos { get { return string.Empty; } }

    public int MyPlayerID
    {
        get { return peerMan.myPlayerID; }
    }
    public Dictionary<int, string> pidMap
    {
        get { return peerMan.PIDMap; }
        set { peerMan.PIDMap = value; }
    }

    void Awake()
    {
        peerMan =  new TestPeerManager(playerID);        
        simMan.CheckSum = new TextCheckSum();
        simMan.PeerMan = peerMan;

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
        peerMan.traceNetActivity = !peerMan.traceNetActivity;
    }

    public void AddAction(IAction action)
    {
        uint currentSnap = simMan.SimSnap + peerMan.SnapActionDelay;

        peerMan.AddAction(currentSnap, MyPlayerID, action);
    }
}
