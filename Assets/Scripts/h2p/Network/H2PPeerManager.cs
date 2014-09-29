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

using System.Collections.Generic;
using System.Net;
using com.ficontent.gws.Peer2Peer.Packets;
using com.ficontent.gws.Peer2Peer.Peers;
using UnityEngine;

/// <summary>
/// Sample Unity Host2Peer Manager implementation
/// One peer becomes the host and controls the game simulation for the other peers
/// </summary>
public class H2PPeerManager : AbstractPeerManager
{
    /// <summary>
    /// delay for sending packets
    /// </summary>
    public float snapSendDelay = .05f;      
    /// <summary>
    /// next send time
    /// </summary>
    private float snapNextSend; 

    public override int myPlayerID
    {
        get;
        set;
    }

    /// <summary>
    /// Defines who is the host peer
    /// </summary>
    public override bool isHost { get { return myPlayerID == 1; } }

    private IPacketSerializer packetSerializer = new BinaryFormatterPacketSerializer();
    /// <summary>
    /// Provides the serialization methods
    /// </summary>
    public override IPacketSerializer PacketSerializer
    {
        get { return packetSerializer; }
    }

    /// <summary>
    /// Determines when to update the network
    /// </summary>
    protected override bool UpdateTimeElapsed
    {
        get
        {
            if (Time.time > snapNextSend)
            {
                snapNextSend = Time.time + snapSendDelay;
                return true;
            }
            else
                return false;
        }
    }

    /// <summary>
    /// Initializes the peer
    /// Creates the pid network addresses map using the other peers in the scene
    /// </summary>
    /// <param name="myPID">Peer's Player ID</param>
    public H2PPeerManager(int myPID)
        : base(myPID)
    {
        PIDMap = new Dictionary<int, string>();
        var peers = GameObject.FindObjectsOfType(typeof(H2PLockStepBehaviour)) as H2PLockStepBehaviour[];

        foreach (var p in peers)
        {
            if (p.myPID != this.myPlayerID)
                PIDMap.Add(p.myPID, "127.0.0.1");
        }
    }


    public override void Start()
    {
        base.Start();
    }

    public override void OnQuit()
    {
        this.localPeer.OnApplicationQuit();
    }  
}
