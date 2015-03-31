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
using FIcontent.Gaming.Enabler.GameSynchronization;
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;

/// <summary>
/// Sample implementation of the LockstepPeer.
/// Chat messages are added as separate RPC calls.
/// The ExecuteAction override implements the execution of the actions for the current simulation turn.
/// </summary>
using System.Collections.Generic;


public class SampleLockstepPeer : LockstepPeer
{

    private static SampleLockstepPeer instance;

    public static SampleLockstepPeer Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<SampleLockstepPeer>();
            return instance;
        }
    }

    public GameObject playerObjectPrefab;
    private Dictionary<string, PlayerObject> playerObjects;

    /// <summary>
    /// Extra RPC to send the chat messages.
    /// This is out of scope of the Lockstep mechanism, and can run also when the simulation is not started, so a normal RPC can be used instead.
    /// </summary>
    /// <param name="msg">Message.</param>
    public void SendChatMessage(string msg)
    {
        this.networkView.RPC("ReceiveChatMessage", RPCMode.Others, msg);
    }

    [RPC]
    void ReceiveChatMessage(string msg)
    {
        FindObjectOfType<ChatGUI>().AppendMessage(msg);
    }

    #region Lockstep overrides

    protected override void ExecuteAction(FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions.IAction a)
    {
        switch (a.Action)
        {
            case CreateObjectAction.ActionType:
                var go = Instantiate(playerObjectPrefab) as GameObject;
                go.GetComponent<PlayerObject>().owner = a.GUID;
                playerObjects.Add(a.GUID, go.GetComponent<PlayerObject>());
                break;

            case PositionAction.ActionType:
                var posAction = a as PositionAction;
                playerObjects [a.GUID].Move(posAction.position);
                break;
        }
    }

    protected override void OnSimulationStarted()
    {
        playerObjects = new Dictionary<string, PlayerObject>();

        AddAction(new CreateObjectAction(this.GUID));
    }

    protected override void Update()
    {
        base.Update();

        Vector3 movement = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            movement = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            movement = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            movement = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            movement = Vector3.right;

        if (movement != Vector3.zero)
        {
            var posAction = new PositionAction(movement, this.GUID);
            AddAction(posAction);
        }
    }

    #endregion


}

