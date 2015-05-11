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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FIcontent.Gaming.Enabler.GameSynchronization.Packets
{
    /// <summary>
    /// IPacketSerializer implementation that uses the .net BinaryFormatter class to provide serialization
    /// </summary>
    public class BinaryFormatterPacketSerializer : IPacketSerializer
    {
        IFormatter bformatter = new BinaryFormatter();

        public object Serialize(IPacket packet)
        {
            byte [] bytes;
            using (var stream = new System.IO.MemoryStream())
            {
                bformatter.Serialize(stream, packet);
                
                bytes = new byte[stream.Length];
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                stream.Read(bytes, 0, (int)stream.Length);
                stream.Close();
            }

            return bytes;
        }

        public IPacket Deserialize(object msg)
        {
            byte [] bytes = (byte[])msg;

            IPacket ret = null;
            using (var stream = new System.IO.MemoryStream(bytes))
            {
                stream.Position = 0;
                ret = (IPacket)bformatter.Deserialize(stream);
                stream.Close();
            }
            
            return ret;
        }
    }
}
