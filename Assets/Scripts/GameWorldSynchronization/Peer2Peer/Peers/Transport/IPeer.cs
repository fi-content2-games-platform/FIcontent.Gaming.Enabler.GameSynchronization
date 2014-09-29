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
using System.Net;

namespace com.ficontent.gws.Peer2Peer.Peers.Transport
{
    /// <summary>
    /// Delegate used for managing received data
    /// </summary>
    /// <param name="data">Data received</param>
    /// <param name="sender">Reference to the sender</param>
    public delegate void DataReceivedDelegate(byte[] data, ref IPEndPoint sender);

    /// <summary>
    /// Defines a network peer. Contains methods for sending and receiving data
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// End point of the current player
        /// </summary>
        IPEndPoint localEndPoint { get; set; }

        /// <summary>
        /// This function is called whenever data is received
        /// </summary>
        DataReceivedDelegate callBack { get; set; }

        /// <summary>
        /// Sends data to an endpoint
        /// </summary>
        /// <param name="remoteEP">Destination endpoint</param>
        /// <param name="message">Data to be sent</param>
        /// <returns>Number of bytes sent</returns>
        int Send(System.Net.IPEndPoint remoteEP, byte[] message);

        /// <summary>
        /// Starts listening for messages
        /// </summary>
        void StartListening();
         
        /// <summary>
        /// Shuts down the server
        /// </summary>
        void StopListening();
                        

        /// <summary>
        /// When set to true the server stops listening for new messages
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// This method is called for finalization
        /// </summary>
        void OnApplicationQuit();
    }
}
