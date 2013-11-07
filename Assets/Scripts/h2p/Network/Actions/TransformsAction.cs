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

using System;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores a list of transforms and provides methods for serialization.
/// This class shows how to extend the AbstractAction class which contains the base action information and serialization methods.
/// </summary>
[Serializable]
public class TransformsAction : com.ficontent.gws.Peer2Peer.Packets.Actions.AbstractAction
{
    public List<TransformsAction.Transform> t;

    public TransformsAction()
        : base(ActionType.TRANSFORMS)
    {
        t = new List<TransformsAction.Transform>();
    }


    /// <summary>
    /// Deserialization
    /// </summary>        
    public TransformsAction(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        this.t = (List<TransformsAction.Transform>)info.GetValue("transforms", typeof(List<TransformsAction.Transform>));
    }

    public void Add(UnityEngine.Transform transform)
    {
        this.t.Add(new Transform(transform));
    }

    /// <summary>
    /// Serialization
    /// </summary>    
    /// <remarks>
    /// On iOS generic collections must be replaced by arrays for serialization due to the aot compiler limitations.
    /// </remarks>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue("transforms", t);
    }

    /// <summary>
    /// Provides a serializable Transform class
    /// </summary>
    [Serializable]
    public class Transform : ISerializable
    {
        public float x, y, z, qx, qy, qz, qw;

        public Transform(UnityEngine.Transform t)
        {
            this.x = t.position.x;
            this.y = t.position.y;
            this.z = t.position.z;
            this.qx = t.rotation.x;
            this.qy = t.rotation.y;
            this.qz = t.rotation.z;
            this.qw = t.rotation.w;
        }

        public Transform(SerializationInfo info, StreamingContext context)
        {
            x = info.GetSingle("x");
            y = info.GetSingle("y");
            z = info.GetSingle("z");
            qx = info.GetSingle("qx");
            qy = info.GetSingle("qy");
            qz = info.GetSingle("qz");
            qw = info.GetSingle("qw");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", x);
            info.AddValue("y", y);
            info.AddValue("z", z);
            info.AddValue("qx", qx);
            info.AddValue("qy", qy);
            info.AddValue("qz", qz);
            info.AddValue("qw", qw);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}) ({3}, {4}, {5}, {6})", x, y, z, qx, qy, qz, qw);
        }
    }
}

/// <summary>
/// Extends the Transform class methods
/// </summary>
public static class TransformExtensions
{
    public static void SetTransform(this UnityEngine.Transform trans, TransformsAction.Transform t)
    {
        trans.position = new Vector3(t.x, t.y, t.z);
        trans.rotation = new Quaternion(t.qx, t.qy, t.qz, t.qw);
    }
}
