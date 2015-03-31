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
using FIcontent.Gaming.Enabler.GameSynchronization.Packets;
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;

namespace FIcontent.Gaming.Enabler.GameSynchronization
{
    [RequireComponent(typeof(NetworkView))]
    public abstract class LockstepPeer : MonoBehaviour
    {
        public uint simulationSnap;
        public uint commandSnap;
        private bool simulationStarted;

        public bool SimulationStarted
        { get { return simulationStarted; } }

        private SnapActions actions;
        private IPacketSerializer packetSerializer = new BinaryFormatterPacketSerializer();
        protected new NetworkView networkView;
        private int playerCount;

        public void RepeatSend(bool start)
        {
            if (start)
                InvokeRepeating("SendSnap", 0f, LockstepSettings.Instance.snapSendInterval);
            else
                CancelInvoke("SendSnap");
        }

        public string GUID
        {
            get
            {
                return Network.player.ToString();

            }
        }

        void Start()
        {
            this.networkView = GetComponent<NetworkView>();
        }

        protected virtual void Update()
        {
            if (simulationStarted)
            {
                PlayerActions currentActions;
                while ((currentActions = this.actions.GetActions(simulationSnap)) != null)
                {
            
                    foreach (var packet in currentActions) // each packet (one per player) in player actions
                        foreach (var action in packet.Value) // each action
                        {
                            ExecuteAction(action);
                        }

                    simulationSnap++; // advance a step

                } // currentActions != null
            
            } // simulationStarted
        }

        protected abstract void ExecuteAction(IAction action);
    
        void OnPlayerConnected(NetworkPlayer player)
        {
            playerCount++;
        }

        public void CreateServer()
        {
            playerCount = 1;
            bool useNat = !Network.HavePublicAddress();

#if UNITY_EDITOR
            useNat = false;
#endif

            Network.InitializeServer(
                    LockstepSettings.Instance.maxConnections, 
                    LockstepSettings.Instance.portNumber, 
                    useNat);

        }

        public void ConnectToServer(HostData hostData)
        {
            Network.Connect(hostData);
        }
 
        void InitializeLockstep()
        {
            simulationSnap = 0;
            commandSnap = 0;

            this.actions = new SnapActions(playerCount);

            simulationStarted = true;

            RepeatSend(true);
        }

        public void AddAction(IAction action)
        {
            if (simulationStarted)
                this.actions.CreateAction(simulationSnap + LockstepSettings.Instance.snapActionDelay, this.GUID, action);
        }

        /// <summary>
        /// Sends the actions for the current snapshot
        /// </summary>
        public void SendSnap()
        {
            // add an empty action in case there isn't any
            this.actions.CreateAction(simulationSnap, this.GUID, null);

            var packet = this.actions.GetActions(commandSnap, this.GUID);
            if (packet != null)
            {
                byte [] msg = (byte[])this.packetSerializer.Serialize(packet);
                this.networkView.RPC("ReceivePacket", RPCMode.OthersBuffered, msg);
                commandSnap++;
            }
        }

        [RPC]
        void ReceivePacket(byte[] packet)
        {
            this.actions.PacketReceived(this.packetSerializer.Deserialize(packet));
        }

        public void StartSimulation()
        {
            this.networkView.RPC("ReceiveStartSimulation", RPCMode.AllBuffered, playerCount);
        }

        [RPC]
        void ReceiveStartSimulation(int playerCount)
        {
            this.playerCount = playerCount;

            InitializeLockstep();
        }

    }
}