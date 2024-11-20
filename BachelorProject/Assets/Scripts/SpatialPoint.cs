using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpatialPoint
{
    [SerializeField] public DateTime timeStamp;
    [SerializeField] public Vector3 position;

    public SpatialPoint(DateTime timeStamp = new(), Vector3 position = new())
    {
        this.timeStamp = timeStamp;
        this.position = position;
    }

    // public void Set(DateTime timeStamp, Vector3 position)
    // {
    //     this.timeStamp = timeStamp;
    //     this.position = position;
    // }
}
