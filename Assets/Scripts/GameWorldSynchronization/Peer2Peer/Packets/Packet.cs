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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using com.ficontent.gws.Peer2Peer.Packets.Actions;

namespace com.ficontent.gws.Peer2Peer.Packets
{

    /// <summary>
    /// Defines a packet that can be serialized and sent on the network
    /// </summary>
    [Serializable]
    public class Packet : ISerializable, IPacket
    {                               
        /// <summary>
        /// List of the actions of the packet
        /// </summary>
        private List<IAction> actions;         

        /// <summary>
        /// Creates a packet
        /// </summary>
        /// <param name="pid">player id</param>
        /// <param name="snap">snap</param>
        public Packet(int pid, uint snap)
        {
            this.PID = pid;
            this.Snap = snap;
            actions = new List<IAction>();            
        }
        /// <summary>
        /// Deserializes a packet
        /// </summary>    
        public Packet(SerializationInfo info, StreamingContext context)
        {
            Snap = info.GetUInt32("snap");
            PID = info.GetInt32("pID");            
            actions = new List<IAction>((IAction[])info.GetValue("actions", typeof(IAction[])));
        }

        #region IPacket members

        public uint Snap { get; set; }
        public int PID { get; set; }          
        public int ActionCount
        {
            get { return actions.Count; }
        }

        public void AddAction(IAction action)
        {
            action.PID = this.PID;
            this.actions.Add(action);
        }

        #endregion

        #region ISerializable members

        /// <summary>
        /// implementation of ISerializable: Serializes a packet
        /// </summary>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("snap", Snap);
            info.AddValue("pID", PID);
            info.AddValue("actions", actions.ToArray());            
        }

        #endregion

        #region IEnumerable members

        IEnumerator<IAction> IEnumerable<IAction>.GetEnumerator()
        {
            return this.actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.actions.GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("pID={0} snap={1} actions={2}", PID, Snap, actions.Count);
        }      
    }
}
