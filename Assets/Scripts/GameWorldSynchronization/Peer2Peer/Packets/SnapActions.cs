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
using com.ficontent.gws.Peer2Peer.Packets.Actions;

namespace com.ficontent.gws.Peer2Peer.Packets
{
    /// <summary>
    /// Dictionary of the actions for each snapshot
    /// It can be used to record the match and check for game integrity
    /// </summary>
    public class SnapActions
    {
        public Dictionary<int, uint> LastReceivedPacket = new Dictionary<int, uint>();      // map of the last packets received from other peers  <playerId, snap>
        private Dictionary<uint, PlayerActions> snapActions = new Dictionary<uint, PlayerActions>();

        /// <summary>
        /// Initializes the dictionary by creating empty actions for the first snapshots
        /// </summary>
        /// <param name="pids">Array of all the pids</param>
        /// <param name="firstSnap">First game snapshot</param>
        /// <returns>The initialized dictionary</returns>
        public SnapActions(int playerID, int[] pids, uint firstSnap)
            : base()
        {
            // init first nop actions
            for (uint i = 0; i < firstSnap; i++)
            {
                this.snapActions.Add(i, new PlayerActions());
                this.snapActions[i].Add(playerID, new Packet(playerID, i));

                foreach (var p in pids)
                    this.snapActions[i].Add(p, new Packet(p, i));
            }

            //first action in the buffer
            this.snapActions.Add(firstSnap, new PlayerActions());
            this.snapActions[firstSnap].Add(playerID, new Packet(playerID, firstSnap));

            // init ack map
            LastReceivedPacket.Clear();
            foreach (var i in pids)
            {
                LastReceivedPacket.Add(i, firstSnap - 1);
            }
        }

        public PlayerActions this[uint snap]
        {
            get { return this.snapActions[snap]; }
        }

        /// <summary>
        /// Manages a received packet
        /// </summary>
        /// <param name="packet">packet to manage</param>
        public void PacketReceived(IPacket packet)
        {
            // still no packets for this snap
            if (!this.snapActions.ContainsKey(packet.Snap))
            {
                this.snapActions.Add(packet.Snap, new PlayerActions());
            }

            // the next packet has been received
            // and still no packets from this pid
            if (LastReceivedPacket[packet.PID] == packet.Snap - 1 && !this.snapActions[packet.Snap].ContainsKey(packet.PID))
            {
                // ack this packet
                LastReceivedPacket[packet.PID]++;

                // adds to the dictionary
                this.snapActions[packet.Snap].Add(packet.PID, packet);
            }
        }

        /// <summary>
        /// Gets the player actions packet for a snapshot
        /// </summary>
        /// <param name="snap">Snapshot of the packet</param>
        /// <param name="pID">Player id</param>
        /// <returns>The packet containing the actions</returns>
        public IPacket GetActions(uint snap, int pID)
        {
            if (this.snapActions.ContainsKey(snap))
            {
                if (this.snapActions[snap].ContainsKey(pID))
                    return this.snapActions[snap][pID];
            }

            return null;
        }

        /// <summary>
        /// Safely adds a new action to the dictionary
        /// </summary>
        /// <param name="snap">Snapshot of the action</param>
        /// <param name="pid">Owning the action</param>
        /// <param name="action">The action to be added</param>        
        public void CreateAction(uint snap, int pid, IAction action)
        {
            // create snap key
            if (!this.snapActions.ContainsKey(snap))
                this.snapActions.Add(snap, new PlayerActions());

            // create player packet
            if (!this.snapActions[snap].ContainsKey(pid))
                this.snapActions[snap].Add(pid, new Packet(pid, snap));

            // add the action   
            if (action != null)
                this.snapActions[snap][pid].AddAction(action);
        }

        /// <summary>
        /// Removes a snapshot from the dictionary
        /// </summary>
        /// <param name="simSnap">Snapshot to be removed</param>
        public void Remove(uint simSnap)
        {
            this.snapActions.Remove(simSnap);
        }

        /// <summary>
        /// Check if the snapshot is contained in the dictionary
        /// </summary>
        /// <param name="simSnap">Snapshot to be checked</param>
        /// <returns>True if contains the snapshot</returns>
        public bool Contains(uint simSnap)
        {
            return this.snapActions.ContainsKey(simSnap);
        }
    }
}
