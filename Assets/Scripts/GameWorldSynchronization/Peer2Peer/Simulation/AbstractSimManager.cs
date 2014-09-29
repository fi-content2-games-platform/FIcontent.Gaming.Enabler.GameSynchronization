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

using com.ficontent.gws.Peer2Peer.CheckSum;
using com.ficontent.gws.Peer2Peer.Packets;
using com.ficontent.gws.Peer2Peer.Packets.Actions;
using com.ficontent.gws.Peer2Peer.Peers;

namespace com.ficontent.gws.Peer2Peer.Simulation
{
    public abstract class AbstractSimManager : ISimulationManager
    {
        private uint simSnap = 0;

        public delegate void SimulationStartEventHandler();
        public event SimulationStartEventHandler SimulationStartEvent;

        public delegate void SimulationStepEventHandler(uint simSnap);
        public event SimulationStepEventHandler SimulationStepEvent;

        public delegate void CheckSumEventHandler(string checkSum, uint simSnap);
        public event CheckSumEventHandler CheckSumEvent;

        public bool SimStarted = false;

        #region ISimulationManager members
        
        public uint SimSnap                        
        {
            get { return simSnap; }
            private set { simSnap = value; }
        }
        public IPeerManager PeerMan { get; set; }        
        public ICheckSum CheckSum { get; set; }                

        public virtual void Update()
        {
            if (!GameHasStarted)
            {
                return;
            }

            // creates an empty packet in the actions buffer (something must be sent every snapshot also if no actions are performed)
            PeerMan.AddAction(SimSnap, PeerMan.myPlayerID, null);

            if (UpdateTimeElapsed) // simulation update
            {               
                if (ExecuteActions())
                {
                    // increments the snapshot of the peer manager
                    PeerMan.OnSuccesfulSim(simSnap);

                    OnSimulationStep();
                                     
                    // increments the current simulation snapshot
                    SimSnap++;

                    //compute checksum
                    if (GameStateCheckSumNeeded)
                    {
                        //compute checksum for the current snap and broadcast to the peers
                        OnComputeCheckSum();                        
                    }

                    // network initialization
                    if (SimSnap == PeerMan.SnapActionDelay)
                        OnSimulationStart();
                }
            }
        }
        
        #endregion

        protected abstract object GetCheckSumParams();                     
                        
        /// <summary>
        /// Tells if the game has started. Implementation specific
        /// </summary>
        /// <returns></returns>
        protected abstract bool GameHasStarted { get; }

        /// <summary>
        /// Indicates if enough time has passed for a new update of the simulation to be executed
        /// </summary>
        /// <returns></returns>
        protected abstract bool UpdateTimeElapsed { get; }

        /// <summary>
        /// Tells if the game state should be checked. You want to call this regularly
        /// </summary>
        /// <returns></returns>
        protected abstract bool GameStateCheckSumNeeded { get; }

        /// <summary>
        /// Executes the simulation of an action
        /// Implemented to dispatch the actions to their performers
        /// </summary>
        /// <param name="a">Action to be executed</param>
        protected abstract void ExecuteAction(IAction a);

        /// <summary>
        /// Called when the simulation starts.
        /// It can be used for level and game objects initialization
        /// </summary>
        private void OnSimulationStart()
        {
            SimStarted = true;
            if (SimulationStartEvent != null)
                SimulationStartEvent();
        }

        /// <summary>
        /// Called when the simulation executes the actions and moves to the next step
        /// </summary>
        private void OnSimulationStep()
        {
            if (SimulationStepEvent != null && SimStarted)
                SimulationStepEvent(simSnap);
        }

        /// <summary>
        /// Computes the checksum with the checksum class provided and raises the event
        /// </summary>
        private void OnComputeCheckSum()
        {
            if (CheckSumEvent != null)
                CheckSumEvent(CheckSum.GetHash(GetCheckSumParams()), simSnap);            
        }

        /// <summary>
        /// Executes the actions for the current snapshot
        /// </summary>
        /// <returns>true if all the peers had sent their actions</returns>
        private bool ExecuteActions()
        {
            PlayerActions pa = PeerMan.GetActions(SimSnap);
            if (pa == null)
                return false;

            foreach (var packet in pa) // each packet (one per player) in player actions
                foreach (var action in packet.Value) // each action
                {
                    ExecuteAction(action);
                }

            return true;
        }
    }
}
