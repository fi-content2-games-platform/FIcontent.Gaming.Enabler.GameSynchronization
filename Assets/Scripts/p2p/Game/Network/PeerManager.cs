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
using System.Net;
using com.ficontent.gws.Peer2Peer.Packets;
using com.ficontent.gws.Peer2Peer.Peers;
/*  ----------------------------------------------------------------------------
 *  Disney Research Zurich
 *  ----------------------------------------------------------------------------
 *  SmartFoxServer 2x - p2p game Example
 *  ----------------------------------------------------------------------------
 *  File:       PeerManager.cs
 *  Author:     Mattia Ryffel
 *  ----------------------------------------------------------------------------
 */
using UnityEngine;

public class PeerManager : AbstractPeerManager
{
    public float snapSendDelay = .05f;      // delay for sending packets
    private float snapNextSend;     // next send time
    public bool traceNetActivity = false;

    public override int myPlayerID { get; set; }
    public override bool isHost { get { return false; } }

    private IPacketSerializer packetSerializer = new BinaryFormatterPacketSerializer();
    public override IPacketSerializer PacketSerializer
    {
        get { return packetSerializer; }
    }

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

    public PeerManager()
        : base(-1)
    {
        PIDMap = new Dictionary<int, string>();

#if UNITY_EDITOR
        this.myPlayerID = 1;
        PIDMap.Add(2, "127.0.0.1");
#else
        this.myPlayerID = 2;
        PIDMap.Add(1, "127.0.0.1");
#endif
    }
        
    public override void Start()
    {
        base.Start();
    }

    public override void OnQuit()
    {
        this.localPeer.OnApplicationQuit();
    }

    protected override int Send(IPEndPoint ip, IPacket packet)
    {
        int retBytes = base.Send(ip, packet);

        if (traceNetActivity)
            Debug.Log(string.Format("tx {0} bytes = {1} dest= {2}", packet, retBytes, ip));

        return retBytes;
    }

    public override void DataReceivedCallBack(byte[] data, ref IPEndPoint sender)
    {
        var packet = packetSerializer.GetPacket(data);
        Actions.PacketReceived(packet);

        if (traceNetActivity)
            Debug.Log(string.Format("rx {0} from {1}", packet, sender));
    }
}
