// Author:
//       Mattia Ryffel <mattia.ryffel@disneyresearch.com>
//
// Copyright (c) 2015 Disney Research Zurich
//
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FIcontent.Gaming.Enabler.GameSynchronization;

public class ChatGUI : HideableGUI
{
    public Text chatText;
    public InputField chatInputField;
    public Scrollbar scrollBar;

    // Use this for initialization
    void Start()
    {
        chatInputField.onEndEdit.AddListener(SubmitChatMessage);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        AppendMessage(string.Format("<i>{0} connected</i>\n", player));
    }

    public void AppendMessage(string message)
    {
        chatText.text += message;

        scrollBar.value = 0f;
    }
    
    void SubmitChatMessage(string line)
    {
        string message = string.Format("<b>{0}:</b> {1}\n", Network.player.ToString(), line);

        if (!string.IsNullOrEmpty(line))
        {
            SampleLockstepPeer.Instance.SendChatMessage(message);
            AppendMessage(message);
        }
        chatInputField.text = string.Empty;
        chatInputField.Select();
        chatInputField.ActivateInputField();
    }
}

