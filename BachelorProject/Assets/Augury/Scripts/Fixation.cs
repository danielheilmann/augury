using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixation
{
    private int globalFixationID;
    private int localFixationID;   //TODO: Not implemented yet
    public int gfid => globalFixationID;
    public int lfid => localFixationID;
    public Vector3 surfaceNormal { get; private set; }
    public DynamicObject dynamicObject { get; private set; } = null;
    public bool isLocal => dynamicObject != null;
    public Vector3 globalPosition { get; private set; }
    public Vector3 localPosition => isLocal ? globalPosition - dynamicObject.transform.position : globalPosition;

    public Fixation(Vector3 globalPosition, Vector3 surfaceNormal, int gfid, int lfid = -1, DynamicObject dynamicObject = null)
    {
        this.surfaceNormal = surfaceNormal;
        this.dynamicObject = dynamicObject;
        this.globalPosition = globalPosition;
        this.globalFixationID = gfid;
        this.localFixationID = isLocal ? lfid : gfid;
    }
}
