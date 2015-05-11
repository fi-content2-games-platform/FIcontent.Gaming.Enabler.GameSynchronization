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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;

namespace FIcontent.Gaming.Enabler.GameSynchronization.Packets
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
        /// Creates a packet owned by a player
        /// </summary>
        /// <param name="guid">player guid that owns the packet</param>
        /// <param name="snap">snapshot of the packet</param>
        public Packet(string guid, uint snap)
        {
            this.GUID = guid;
            this.Snap = snap;
            actions = new List<IAction>();            
        }
        /// <summary>
        /// Deserializes a packet
        /// </summary>    
        public Packet(SerializationInfo info, StreamingContext context)
        {
            Snap = info.GetUInt32("snap");
            GUID = info.GetString("guid");            
            actions = new List<IAction>((IAction[])info.GetValue("actions", typeof(IAction[])));
        }

        #region IPacket members

        public uint Snap { get; set; }
        public string GUID { get; set; }          
        public int ActionCount
        {
            get { return actions.Count; }
        }

        public void AddAction(IAction action)
        {
            action.GUID = this.GUID;
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
            info.AddValue("guid", GUID);
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
            return string.Format("GUID={0} snap={1} actions={2}", GUID, Snap, actions.Count);
        }      
    }
}
