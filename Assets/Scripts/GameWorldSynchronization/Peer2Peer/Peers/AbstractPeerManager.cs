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
using com.ficontent.gws.Peer2Peer.Packets.Actions;
using com.ficontent.gws.Peer2Peer.Peers.Transport;

namespace com.ficontent.gws.Peer2Peer.Peers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractPeerManager : IPeerManager
    {
        /// <summary>
        /// Map of the player ips used for initialization <playerID, IP>
        /// </summary>        
        public Dictionary<int, string> PIDMap;

        /// <summary>
        /// Peer network layer
        /// </summary>
        protected IPeer localPeer;

        /// <summary>
        /// Used to compute the network cycles, when it is time to check for sending next network updates
        /// </summary>
        protected abstract bool UpdateTimeElapsed { get; }

        private readonly uint snapActionDelay = 2;
        private uint snap = 0;        
        private IPEndPoint localEndPoint;
        /// <summary>
        /// Starting of the port range
        /// e.g. adding the player id x 2 to the port is the receiving port, adding player id x 2 + 1 is the sending port
        /// </summary>
        private readonly int basePort = 12300;
        /// <summary>
        /// Dictionary of the player ids and endpoint (IP and port)
        /// </summary>
        protected Dictionary<int, IPEndPoint> playerEndPointsMap;

        protected bool keepActions = true;

        public AbstractPeerManager(int myPID)
        {
            this.myPlayerID = myPID;

            //init the dictionaries
            this.playerEndPointsMap = new Dictionary<int, IPEndPoint>();
        }

        #region IPeerManager members

        public abstract bool isHost { get; }
        public abstract int myPlayerID { get; set; }
        public SnapActions Actions { get; set; }
        public uint SnapActionDelay { get { return snapActionDelay; } }
        public abstract IPacketSerializer PacketSerializer { get; }
                
        public virtual void Start()
        {
            // creates the star topology
            foreach (var p in PIDMap)
            {
                this.playerEndPointsMap.Add(p.Key, new IPEndPoint(IPAddress.Parse(p.Value), basePort + p.Key * 2));
            }

            // assign ports            
            this.localEndPoint = new IPEndPoint(IPAddress.Parse(IPHelper.LocalIPAddress), basePort + myPlayerID * 2);

            // creates the network peer
            this.localPeer = new UDPPeer();
            this.localPeer.localEndPoint = localEndPoint;
            this.localPeer.callBack = new DataReceivedDelegate(DataReceivedCallBack);
            this.localPeer.StartListening();

            // init last received packet dictionary
            int[] pidsArray = new int[PIDMap.Count];
            PIDMap.Keys.CopyTo(pidsArray, 0);

            this.Actions = new SnapActions(myPlayerID, pidsArray, SnapActionDelay);
        }
        public virtual void Update()
        {
            // send packets every turn
            if (UpdateTimeElapsed)
            {
                SendPackets();  //1-4ms            
            }
        }
        public abstract void OnQuit();
        
        public virtual void AddAction(uint simSnap, int pid, IAction action)
        {
            uint currentSnap = simSnap + SnapActionDelay;

            Actions.CreateAction(currentSnap, pid, action);
        }
        public PlayerActions GetActions(uint simSnap)
        {
            if (Actions.Contains(simSnap))
            {
                if (Actions[simSnap].Count == PIDMap.Count + 1)
                {
                    return Actions[simSnap];
                }
            }

            return null; // if not every peer has reached the same snap
        }

        #endregion

        /// <summary>
        /// Called when a succesful simulation step is performed.
        /// It increases the peer manager snap counter
        /// </summary>
        /// <param name="simSnap">New value of the snap counter</param>
        public void OnSuccesfulSim(uint simSnap)
        {
            this.snap = simSnap;
            this.CleanActions(simSnap);
        }

        /// <summary>
        /// Callback that manages the receiving of a packet
        /// </summary>
        /// <param name="data">Data received</param>
        /// <param name="sender">Reference to the sender end point</param>
        public virtual void DataReceivedCallBack(byte[] data, ref IPEndPoint sender)
        {
            var packet = PacketSerializer.GetPacket(data);
            Actions.PacketReceived(packet);
        }

        /// <summary>
        /// Sends the packets to the peers
        /// Only the packets since the last received packet are sent to each peer
        /// </summary>
        protected virtual void SendPackets()
        {
            foreach (var k in playerEndPointsMap)
            {
                // last snap received from that peer
                uint from = this.Actions.LastReceivedPacket[k.Key] + 1;
                uint to = this.snap + SnapActionDelay;
                if (from > to) from = to;

                for (uint i = from; i <= to; i++)
                {
                    var packet = Actions.GetActions(i, myPlayerID);
                    if (packet != null)
                    {
                        this.Send(k.Value, packet);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a packet to an endpoint
        /// </summary>
        /// <param name="ip">destination</param>
        /// <param name="packet">the packet</param>
        /// <returns>bytes sent</returns>
        protected virtual int Send(IPEndPoint ip, IPacket packet)
        {
            byte[] msg = PacketSerializer.GetBytes(packet);
            return localPeer.Send(ip, msg);
        }

        /// <summary>
        /// Broadcasts an action immediatly.
        /// The action is broadcasted only one time. This call is meant for actions that don't require a reliable channel.
        /// </summary>
        /// <param name="action">Action to be broadcasted</param>
        /// <param name="simSnap">Snapshot of the action</param>
        public virtual void BroadCast(IAction action, uint simSnap)
        {
            Packet packet = new Packet(myPlayerID, simSnap + SnapActionDelay);
            packet.AddAction(action);
            foreach (var p in this.playerEndPointsMap)
                Send(p.Value, packet);
        }

        /// <summary>
        /// Cleans the actions in the dictionary for a specified snap (e.g. to save memory)
        /// </summary>
        /// <param name="simSnap">Snap to be cleaned</param>
        private void CleanActions(uint simSnap)
        {
            if (keepActions && !Actions.Contains(simSnap))
                return;

            Actions[simSnap].Clear();
            Actions.Remove(simSnap);
        }
    }

}
