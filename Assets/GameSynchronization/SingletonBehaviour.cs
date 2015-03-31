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

/// <summary>
/// Singleton implementation for MonoBehaviours
/// The object instance is first searched in the scene, created if not found and finally returned.
/// If multiple instances are found an exception is raised.
/// </summary>
namespace FIcontent.Gaming.Enabler.GameSynchronization
{
    public class SingletonBehaviour<T> : MonoBehaviour  
    where T : UnityEngine.Component
    {
        private static T instance;

        public static T Instance
        {
            get
            { 

                if (instance == null)
                {
                    var found = FindObjectsOfType<T>();

                    if (found.Length > 1)
                        throw new UnityException("SingletonBehaviour: more than one instance has been found in the scene!");
                    else if (found.Length == 1)
                        instance = found [0];
                    else 
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
        
                return instance;
            }
        }

    }
}

