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

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Outputs debug information gathered from the LockStepBehaviours in the scene
/// </summary>
public class TestGUI : MonoBehaviour
{
    private List<ITestInfos> peers;

    void Start()
    {
        peers = new List<ITestInfos>();
        peers.AddRange(FindObjectsOfType(typeof(TestLockStepBehaviour)) as TestLockStepBehaviour[]);
        peers.AddRange(FindObjectsOfType(typeof(H2PLockStepBehaviour)) as H2PLockStepBehaviour[]);
    }

    void OnGUI()
    {
        foreach (var p in peers)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("player: " + p.MyPlayerID);
            GUILayout.Label("snap: " + p.SimSnap);
            GUILayout.Label(p.OtherInfos);
            GUILayout.EndVertical();
        }
    }
}

/// <summary>
/// Stores informations to be displayed on TestGUI
/// </summary>
public interface ITestInfos
{
    int MyPlayerID { get; }
    uint SimSnap { get; }
    string OtherInfos { get; }
}
