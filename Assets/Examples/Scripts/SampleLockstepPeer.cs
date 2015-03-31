// Author:
//       Mattia Ryffel <mattia.ryffel@disneyresearch.com>
//
// Copyright (c) 2015 Disney Research Zurich
//
using UnityEngine;
using System.Collections;
using FIcontent.Gaming.Enabler.GameSynchronization;
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;

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

    public void SendChatMessage(string msg)
    {
        this.networkView.RPC("ReceiveChatMessage", RPCMode.Others, msg);
    }

    [RPC]
    void ReceiveChatMessage(string msg)
    {
        FindObjectOfType<ChatGUI>().AppendMessage(msg);
    }

    protected override void ExecuteAction(FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions.IAction a)
    {
        switch (a.Action)
        {
            case PositionAction.ActionType:
                PositionAction posAction = a as PositionAction;
                Debug.Log("Received position from " + posAction.GUID);
                break;
        }
    }


    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PositionAction p = new PositionAction(Vector3.zero);
            p.GUID = this.GUID;

            AddAction(p);
        }
    }


}

