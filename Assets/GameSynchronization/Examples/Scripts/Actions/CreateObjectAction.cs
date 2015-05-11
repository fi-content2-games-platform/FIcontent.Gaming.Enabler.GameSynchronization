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
/// The AbstractAction class is extended to send an instruction to create an object.
/// </summary>
[System.Serializable]
public class CreateObjectAction : AbstractAction
{
    public const int ActionType = 4;
    
    public CreateObjectAction(string guid)
        : base(ActionType)
    {
        base.GUID = guid;
    }
    
    
    /// <summary>
    /// Deserialization constructor
    /// </summary>    
    public CreateObjectAction(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
       
    }
    
    /// <summary>
    /// Serialization method
    /// </summary> 
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
    
}

