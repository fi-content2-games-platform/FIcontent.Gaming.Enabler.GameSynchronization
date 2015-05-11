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
using UnityEngine.UI;
using System.Collections;
using FIcontent.Gaming.Enabler.GameSynchronization;


/// <summary>
/// Chat GUI Component
/// The chat messages are sent by SampleLockstepPeer via RPC
/// </summary>
public class ChatGUI : HideableCanvas
{
    public Text chatText;
    public InputField chatInputField;
    public Scrollbar scrollBar;

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
    }
}

