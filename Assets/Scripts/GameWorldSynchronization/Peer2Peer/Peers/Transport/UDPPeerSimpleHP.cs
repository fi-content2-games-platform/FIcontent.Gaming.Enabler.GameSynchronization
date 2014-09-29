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

using System.Net;
using System.Net.Sockets;

namespace com.ficontent.gws.Peer2Peer.Peers.Transport
{
    /// <summary>
    /// Sends and receives bytes via UDP sockets.
    /// This implementation uses System.Net.Sockets.UdpClient and uses a simple approach to hole punching
    /// </summary>
    /// <remarks>
    /// Data sent is not compressed
    /// </remarks>
    public class UDPPeerSimpleHP : AbstractPeer
    {
        private UdpClient rx, tx;

        protected override void Init()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, localEndPoint.Port);
            rx = new UdpClient(ipep);
            tx = new UdpClient(localEndPoint.Port + 1);
            tx.Client.ReceiveTimeout = 100;
        }

        public override int Send(IPEndPoint remoteEP, byte[] message)
        {
            int ret = tx.Send(message, message.Length, remoteEP);
            tx.Receive(ref remoteEP);

            return ret;
        }

        protected override void Server(object obj)
        {
            byte[] data;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (!IsDone)
            {
                data = rx.Receive(ref sender);
                callBack(data, ref sender);
                rx.Send(new byte[0], 0, sender);
            }
        }
    }
}
