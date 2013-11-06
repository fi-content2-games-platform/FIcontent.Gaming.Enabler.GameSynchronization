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

using com.ficontent.gws.Peer2Peer.Packets;
using com.ficontent.gws.Peer2Peer.Packets.Actions;

namespace com.ficontent.gws.Peer2Peer.Peers
{
    public interface IPeerManager
    {
        /// <summary>
        /// Dictionary of the actions for each snap and player
        /// </summary>
        SnapActions Actions { get; set; }

        /// <summary>
        /// IPacketSerializer takes care of serializing and deserializing the packets
        /// </summary>
        IPacketSerializer PacketSerializer { get; }

        /// <summary>
        /// Player unique ID
        /// </summary>
        int myPlayerID { get; set; }

        /// <summary>
        /// Whether the peer acts as a host (e.g. playerID == 1)
        /// </summary>
        bool isHost { get; }

        /// <summary>
        /// Defines the delay in snaps between an action emission and its execution (e.g. 2 is a common value)
        /// </summary>
        uint SnapActionDelay { get; }
              
        /// <summary>
        /// Called only once when the object is initalized
        /// </summary>
        void Start();

        /// <summary>
        /// Must be called each frame
        /// </summary>
        void Update();

        /// <summary>
        /// Must be invoked upon quitting the application
        /// </summary>
        void OnQuit();

        /// <summary>
        /// Adds a new action to the dictionary.
        /// To enforce network packet releability, packets are sent for each snapshot also if they don't contain any actions;
        /// Therefore if no actions were performed this method can be called wihtout the action parameter.
        /// </summary>
        /// <param name="simSnap">Snap of the action</param>
        /// <param name="pid">Owner of the action</param>
        /// <param name="action">Action to be added</param>
        void AddAction(uint simSnap, int pid, IAction action);

        /// <summary>
        /// Returns a list of actions for the snap 
        /// </summary>
        /// <param name="simSnap">Snap of the actions</param>
        /// <returns>A dictionary containing actions for each player</returns>
        PlayerActions GetActions(uint simSnap);

        /// <summary>
        /// Called when a succesful simulation step is performed.
        /// It increases the peer manager snap counter
        /// </summary>
        /// <param name="simSnap">New value of the snap counter</param>
        void OnSuccesfulSim(uint simSnap);

    }
}
