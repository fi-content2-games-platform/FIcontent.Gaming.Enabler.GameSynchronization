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
    /// <summary>
    /// Abstract class for the Lockstep mechanism
    /// Defines the methods to send the packets and advance the simulation turns
    /// </summary>
    [RequireComponent(typeof(NetworkView))]
    public abstract class LockstepPeer : MonoBehaviour
    {
        /// <summary>
        /// The current simulation snap.
        /// If the actions from all the players are received for this snap, they are executed and the simulation advance to the next snap.
        /// </summary>
        public uint simulationSnap;
        /// <summary>
        /// Counter of the last action sent
        /// </summary>
        public uint commandSnap;
        private bool simulationStarted;

        public bool SimulationStarted
        { get { return simulationStarted; } }

        /// <summary>
        /// Dictionary containing the actions for every snap from all the players.
        /// The content of this dictionary can reproduce the simulation as a demo.
        /// </summary>
        private SnapActions actions;
        private IPacketSerializer packetSerializer = new BinaryFormatterPacketSerializer();
        protected new NetworkView networkView;
        private int playerCount;

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

        #region Lockstep mechanism 

        /// <summary>
        /// Initializes the Lockstep
        /// </summary>
        private void InitializeLockstep()
        {
            simulationSnap = 0;
            commandSnap = 0;
            
            this.actions = new SnapActions(playerCount);
            
            simulationStarted = true;
            
            RepeatSend(true);

            OnSimulationStarted();
        }

        /// <summary>
        /// Lockstep mechanism:
        /// When the actions for the current snap are received from all the players they are executed and the simulation can proceed to the next step.
        /// </summary>
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

        /// <summary>
        /// Executes the actions from that specific turn
        /// </summary>
        /// <param name="action">Action to be executed</param>
        protected abstract void ExecuteAction(IAction action);

        /// <summary>
        /// Event raised when the simulation is started.
        /// </summary>
        protected abstract void OnSimulationStarted();

        /// <summary>
        /// Adds an action to the queue of actions to send
        /// </summary>
        /// <param name="action">Action.</param>
        public void AddAction(IAction action)
        {
            if (simulationStarted)
                this.actions.CreateAction(simulationSnap + LockstepSettings.Instance.snapActionDelay, this.GUID, action);
        }

        #endregion
    
        #region Connection

        /// <summary>
        /// The peer acting as a server counts the connected players to initialize the lockstep
        /// </summary>
        /// <param name="player">Player.</param>
        void OnPlayerConnected(NetworkPlayer player)
        {
            playerCount++;
        }

        /// <summary>
        /// Unity does not allow p2p networking, so one of the peers must act as a server
        /// </summary>
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
 
        #endregion
       
        #region Send Snap RPC

        /// <summary>
        /// Sends the actions for the current snapshot.
        /// If there are no current actions an empty packet is added to ensure that every player still gets a message for every snap.
        /// Since Unity networking RPC calls are reliable there is no need to take track of the last received packets from the other players.
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
        private void ReceivePacket(byte[] packet)
        {
            this.actions.PacketReceived(this.packetSerializer.Deserialize(packet));
        }

        private void RepeatSend(bool start)
        {
            if (start)
                InvokeRepeating("SendSnap", 0f, LockstepSettings.Instance.snapSendInterval);
            else
                CancelInvoke("SendSnap");
        }

        #endregion

        #region Start Simulation RPC

        /// <summary>
        /// The server sends this message when starting the simulation.
        /// Everyone gets the number of players to initialize their lockstep mechanism and the simulation starts.
        /// </summary>
        public void StartSimulation()
        {
            this.networkView.RPC("ReceiveStartSimulation", RPCMode.AllBuffered, playerCount);
        }

        [RPC]
        private void ReceiveStartSimulation(int playerCount)
        {
            this.playerCount = playerCount;

            InitializeLockstep();
        }

        #endregion

    }
}