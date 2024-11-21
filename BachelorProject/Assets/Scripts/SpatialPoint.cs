using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! Currently not in use.
[Serializable]
public struct SpatialPoint
{
    public float timeStampTicks { get; private set; }
    public Vector3 position { get; private set; }

    public SpatialPoint(float timeStampTicks, Vector3 position)
    {
        this.timeStampTicks = timeStampTicks;
        this.position = position;
    }
}
