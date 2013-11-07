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
using System.Collections;

/// <summary>
/// Gets the player inputs.
/// MouseClick button generates a DrawAction.
/// </summary>
public class InputBehaviour : MonoBehaviour
{
    private LockStepBehaviour lockStep;
    
    void Start()
    {
        lockStep = FindObjectOfType(typeof(LockStepBehaviour)) as LockStepBehaviour;
    }

    void Update()
    {
        //game simulation must be started
        if (!lockStep.SimStarted)
            return;

        //get button 0 and creates a draw action at the screen location
        if (Input.GetMouseButton(0))
            GetPoint();

    }

    void GetPoint()
    {
        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        float enter = 100f;
        if (p.Raycast(r, out enter))
        {
            var hit = r.GetPoint(enter);
            var action = new DrawPointAction(hit);
            lockStep.AddAction(action);
        }
    }
}
