using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//! Currently not in use.

[Serializable]
public record GazePoint(float timeStampTicks, Vector3 position)
{
    public float timeStampTicks { get; private set; } = timeStampTicks;
    public Vector3 position { get; private set; } = position;
}
