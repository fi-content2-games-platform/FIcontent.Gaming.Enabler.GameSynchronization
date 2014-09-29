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
using com.ficontent.gws.Peer2Peer.CheckSum;
using com.ficontent.gws.Peer2Peer.Peers;

namespace com.ficontent.gws.Peer2Peer.Simulation
{
    public interface ISimulationManager
    {
        /// <summary>
        ///  A checksum mechanism to check if the gamestate is the same on all peers    
        /// </summary>
        ICheckSum CheckSum { get; set; }     

        /// <summary>
        /// Instance of the peer manager
        /// </summary>
        IPeerManager PeerMan { get; set; }

        /// <summary>
        /// Simulation snapshot (or server time, unsigned integer)
        /// </summary>
        uint SimSnap { get; }

        /// <summary>
        /// This is the main simulation update. You should call this regularly in your game engine event loop
        /// </summary>
        void Update();
    }
}
