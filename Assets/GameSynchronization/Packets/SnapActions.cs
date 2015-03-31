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
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;

namespace FIcontent.Gaming.Enabler.GameSynchronization.Packets
{
    /// <summary>
    /// Dictionary of the actions for each snapshot
    /// It can be used to record the match and check for game integrity
    /// </summary>
    public class SnapActions
    {
        // map of the player actions for every snap
        private Dictionary<uint, PlayerActions> snapActions = new Dictionary<uint, PlayerActions>();
        private int playersCount = -1;

        public SnapActions(int playersCount)
        {
            this.playersCount = playersCount;
        }

        public PlayerActions this [uint snap]
        {
            get { return this.snapActions [snap]; }
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
            //if (lastReceivedPacket [packet.GUID] == packet.Snap - 1 && !this.snapActions [packet.Snap].ContainsKey(packet.GUID))
            if (!this.snapActions[packet.Snap].ContainsKey(packet.GUID))
            {
                // adds to the dictionary
                this.snapActions [packet.Snap].Add(packet.GUID, packet);
            }
        }

        /// <summary>
        /// Gets the player actions packet for a snapshot
        /// </summary>
        /// <param name="snap">Snapshot of the packet</param>
        /// <param name="pID">Player id</param>
        /// <returns>The packet containing the actions</returns>
        public IPacket GetActions(uint snap, string guid)
        {
            if (this.snapActions.ContainsKey(snap))
            {
                if (this.snapActions [snap].ContainsKey(guid))
                    return this.snapActions [snap] [guid];
            }

            return null;
        }

        /// <summary>
        /// Retrieves the actions of all players for the specified snapshot only if they all have been received
        /// </summary>
        /// <returns>The actions of all players or null if not every peer has reached the same snap</returns>
        /// <param name="snap">The snap</param>
        public PlayerActions GetActions(uint snap)
        {
            if (this.snapActions.ContainsKey(snap))
            {
                if (this.snapActions [snap].Count == playersCount)
                {
                    return this.snapActions [snap];
                }
            }

            return null; 
        }

        /// <summary>
        /// Safely adds a new action to the dictionary
        /// </summary>
        /// <param name="snap">Snapshot of the action</param>
        /// <param name="guid">Owner of the action</param>
        /// <param name="action">The action to be added</param>        
        public void CreateAction(uint snap, string guid, IAction action)
        {
            // create snap key
            if (!this.snapActions.ContainsKey(snap))
                this.snapActions.Add(snap, new PlayerActions());

            // create player packet
            if (!this.snapActions [snap].ContainsKey(guid))
                this.snapActions [snap].Add(guid, new Packet(guid, snap));

            // add the action   
            if (action != null)
                this.snapActions [snap] [guid].AddAction(action);
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
