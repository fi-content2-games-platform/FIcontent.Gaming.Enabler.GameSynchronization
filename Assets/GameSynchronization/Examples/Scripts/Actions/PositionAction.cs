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
using FIcontent.Gaming.Enabler.GameSynchronization.Packets.Actions;
using System.Runtime.Serialization;

/// <summary>
/// Sample implementation of an Action.
/// The AbstractAction class is extended to send a Vector3.
/// </summary>
[System.Serializable]
public class PositionAction : AbstractAction
{
    public const int ActionType = 3;

    private float x;
    private float y;
    private float z;
    
    public Vector3 position
    {
        get { return new Vector3(x, y, z); }
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
    
    public PositionAction(Vector3 point, string guid)
        : base(ActionType)
    {
        this.position = point;
        base.GUID = guid;
    }
    
    
    /// <summary>
    /// Deserialization constructor
    /// </summary>    
    public PositionAction(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        this.x = info.GetSingle("x");
        this.y = info.GetSingle("y");
        this.z = info.GetSingle("z");
    }
    
    /// <summary>
    /// Serialization method
    /// </summary> 
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        
        info.AddValue("x", x);
        info.AddValue("y", y);
        info.AddValue("z", z);
    }

}

