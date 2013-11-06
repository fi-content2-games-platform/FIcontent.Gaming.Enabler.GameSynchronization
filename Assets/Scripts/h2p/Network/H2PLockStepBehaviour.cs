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

public class H2PLockStepBehaviour : MonoBehaviour, ITestInfos
{
    private H2PPeerManager peerMan; // = new H2PPeerManager();
    private H2PSimManager simMan = new H2PSimManager();

    public GameObject prefab;
    public int myPID;
    public int playerID
    {
        get { return myPID; }
    }

    public int MyPlayerID
    {
        get { return peerMan.myPlayerID; }
    }
    public Dictionary<int, string> pidMap
    {
        get { return peerMan.PIDMap; }
        set { peerMan.PIDMap = value; }
    }

    private string otherInfos = string.Empty;
    public string OtherInfos
    {
        get { return otherInfos; }
    }

    void Awake()
    {
        peerMan = new H2PPeerManager(myPID);
        //peerMan.myPlayerID = myPID;
        //peerMan.Awake();

        simMan.CheckSumEvent += simMan_CheckSumEvent;
        simMan.SimulationStartEvent += simMan_SimStarted;
        simMan.SimulationStepEvent += simMan_SuccesfulSimulation;
        simMan.CheckSum = new TextCheckSum();
        simMan.PeerMan = peerMan;

        var go = Instantiate(prefab) as GameObject;
        go.name = "test object " + (peerMan.isHost ? "host" : "client") + " " + myPID;
        simMan.objects.Add(go.transform);
    }

    void simMan_CheckSumEvent(string checksum, uint simSnap)
    {
        simMan.map.Add(simSnap, checksum);
        peerMan.AddAction(simSnap, peerMan.myPlayerID, new CheckSumAction(checksum));
    }

    void simMan_SimStarted()
    {
        Debug.Log("Started " + peerMan.myPlayerID);
    }

    void simMan_SuccesfulSimulation(uint simSnap)
    {
        SendPosition();
        otherInfos = simMan.otherInfos;
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

        peerMan.AddAction(currentSnap, MyPlayerID, action);
    }


    public uint SimSnap
    {
        get { return simMan.SimSnap; }
    }

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
