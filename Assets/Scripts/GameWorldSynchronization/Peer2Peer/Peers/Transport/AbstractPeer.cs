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

using System.Net;
using System.Threading;

namespace com.ficontent.gws.Peer2Peer.Peers.Transport
{
    /// <summary>
    /// Abstracts a peer connection into a monobehaviour
    /// </summary>
    public abstract class AbstractPeer : IPeer
    {
        /// <summary>
        /// Server thread
        /// </summary>
        protected Thread listenThread;

        private object handle = new object();
        private bool isDone = false;
        
        #region IPeer members

        public DataReceivedDelegate callBack { get; set; }

        public IPEndPoint localEndPoint{ get; set; }

        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (handle)
                {
                    tmp = isDone;
                }
                return tmp;
            }
        }

        public abstract int Send(System.Net.IPEndPoint remoteEP, byte[] message);

        public virtual void StartListening()
        {
            Init();
            listenThread = new Thread(new ParameterizedThreadStart(Server));
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void StopListening()
        {
            isDone = true;

            try
            {
                Send(localEndPoint, new byte[] { });
                listenThread.Abort();
            }
            catch
            {

            }
        }               

        /// <summary>
        /// When quitting sets isDone and sends a last message to exit the receive loop, then tries to abort the thread
        /// </summary>
        public void OnApplicationQuit()
        {
            StopListening();
        }

        #endregion

        /// <summary>
        /// When quitting sets isDone and sends a last message to exit the receive loop, then tries to abort the thread
        /// </summary>
        public virtual void OnQuit()
        {
            StopListening();
        }

        /// <summary>
        /// Server thread, used to receive messages
        /// When a message is received the callback delegate is called
        /// </summary>
        /// <param name="parameters">ParameterizedThreadStart parameters</param>
        protected abstract void Server(object parameters);

        /// <summary>
        /// Used for initialization
        /// </summary>
        protected abstract void Init();
    }
}
