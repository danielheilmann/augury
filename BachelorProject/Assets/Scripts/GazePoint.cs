using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//! Currently not in use.

[Serializable]
public struct GazePoint
{
    public DateTime timeStamp { get; private set; }
    public Vector3 position { get; private set; }

    public GazePoint(DateTime timeStamp, Vector3 position)
    {
        this.timeStamp = timeStamp;
        this.position = position;
    }

    public void Set(DateTime timeStamp, Vector3 position)
    {
        this.timeStamp = timeStamp;
        this.position = position;
    }
}
