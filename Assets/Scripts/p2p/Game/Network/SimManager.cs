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
/// Simulation manager
/// Implements the lockstep simulation
/// </summary>
public class SimManager : AbstractSimManager
{
    public readonly float snapDelay = .1f;               // delay of the simulation snapshot
      
    private float nextSnap;
    private readonly uint checkSumDelay = 10;    
    
    public Dictionary<uint, string> map = new Dictionary<uint, string>();
    public static List<Vector3> objects = new List<Vector3>();


    protected override object GetCheckSumParams()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var o in objects)
            sb.AppendLine(o.ToString());

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

    protected override void ExecuteAction(IAction a)
    {
        switch (a.Action)
        {
            case ActionType.DRAW:
                DrawPointAction drawAction = a as DrawPointAction;
                DrawBehaviour.Draw(drawAction);
                objects.Add(drawAction.position);
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

            default:            
                break;
        }

    }   

    #endregion
}
