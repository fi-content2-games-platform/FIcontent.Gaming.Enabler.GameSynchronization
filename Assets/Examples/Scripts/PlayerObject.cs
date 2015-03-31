// Author:
//       Mattia Ryffel <mattia.ryffel@disneyresearch.com>
//
// Copyright (c) 2015 Disney Research Zurich
//
using UnityEngine;
using System.Collections;

public class PlayerObject : MonoBehaviour
{
    public string owner;

    public void Move(Vector3 direction)
    {
        this.transform.Translate(direction);
    }
}

