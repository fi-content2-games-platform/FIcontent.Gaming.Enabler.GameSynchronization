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
using UnityEngine;
using System.Collections;

namespace FIcontent.Gaming.Enabler.GameSynchronization
{
    /// <summary>
    /// General settings for the Lockstep mechanism
    /// </summary>
    public class LockstepSettings : SingletonBehaviour<LockstepSettings>
    {
        /// <summary>
        /// Defines how frequently the actions are sent through the network
        /// </summary>
        public float snapSendInterval = 0.1f;
        public int portNumber = 1337;
        public int maxConnections = 8;

        /// <summary>
        /// The delay (in simulation snap time) of when the new actions are pushed in the queue
        /// An action issued at snap time n will be executed at snap time n + snapActionDelay
        /// </summary>
        public uint snapActionDelay = 2;
    }
}

