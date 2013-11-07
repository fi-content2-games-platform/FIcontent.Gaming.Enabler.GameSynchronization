/* Copyright (c) 2013 Disney Research Zurich and ETH Zurich
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

using com.ficontent.gws.Peer2Peer.Packets.Actions;
using com.ficontent.gws.Peer2Peer.Simulation;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Host to Peer Simulation manager
/// Implements the lockstep simulation by extending the AbstractSimManager class
/// </summary>
public class H2PSimManager : AbstractSimManager
{
    /// <summary>
    /// delay of the simulation snapshot in ms
    /// </summary>
    public readonly float snapDelay = .1f;
    /// <summary>
    /// next simulation snapshot time
    /// </summary>
    private float nextSnap;
    /// <summary>
    /// interval of snapshots for checksum calculation
    /// </summary>
    private readonly uint checkSumDelay = 10;

    /// <summary>
    /// map of the local player checksums
    /// </summary>
    public  Dictionary<uint, string> map = new Dictionary<uint, string>();
    /// <summary>
    /// list of the game objects
    /// </summary>
    public List<Transform> objects = new List<Transform>();
    public string otherInfos;

    /// <summary>
    /// buffer of the positions of the objects
    /// </summary>
    private Queue<Vector3> vbuffer = new Queue<Vector3>();

    /// <summary>
    /// retrieves the string for the checksum calculation input
    /// </summary>
    protected override object GetCheckSumParams()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(objects[0].position.ToString());
        return sb.ToString();
    }
        
    protected override bool GameHasStarted
    {
        get { return true; }
    }

    protected override bool UpdateTimeElapsed
    {
        get
        {
            if (Time.time > nextSnap)
            {
                nextSnap = Time.time + snapDelay;
                return true;
            }
            else
                return false;
        }
    }

    protected override bool GameStateCheckSumNeeded
    {
        get { return this.SimSnap % checkSumDelay == 0; }
    }

    #region Execute actions

    /// <summary>
    /// Executes the actions received from the network
    /// It's called when a simulation step is executed, all the actions for each player for that snapshot are executed
    /// </summary>
    protected override void ExecuteAction(IAction a)
    {
        switch (a.Action)
        {
            case ActionType.TRANSFORMS:
                var ta = a as TransformsAction;
                if (!PeerMan.isHost)
                {
                    SetPosition(ta.t[0]);
                }
                break;

            case ActionType.CHECKSUM:
                var checkAction = a as CheckSumAction;
                var snap = SimSnap - PeerMan.SnapActionDelay;

                if (!map[snap].Equals(checkAction.checkSum))
                {
                    Debug.LogError("Checksum Error!");
                    Debug.Log(checkAction.checkSum);
                    Debug.Log(map[snap]);
                }
                else
                    Debug.Log("Checksum OK!");
                break;

            case ActionType.DRAW:
                Debug.Log("rx " + PeerMan.myPlayerID);
                break;

            default:
                break;
        }

    }

    void SetPosition(TransformsAction.Transform t)
    {
        vbuffer.Enqueue(new Vector3(t.x, t.y, t.z));

        if (vbuffer.Count > 2)
        {
            vbuffer.Dequeue();
        }

    }

    #endregion
    
    /// <summary>
    /// the game object position is updated using the position buffer
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (vbuffer.Count == 2)
        {
            var o = objects[0];

            o.position = Vector3.Lerp(o.position, vbuffer.Peek(), .5f);
            otherInfos = objects[0].transform.position + " " + objects[0].transform.rotation;
        }
    }  
}
