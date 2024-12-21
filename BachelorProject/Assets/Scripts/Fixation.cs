using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixation
{
    /// <summary> Global Fixation ID </summary>
    public int gfid { get; private set; }
    /// <summary> Local Fixation ID </summary>
    public int lfid { get; private set; }   //TODO: Not implemented yet
    public Vector3 surfaceNormal { get; private set; }
    public DynamicObject dynamicObject { get; private set; } = null;
    public Vector3 position => isLocal ? dynamicObject.transform.position + _position : _position;
    public bool isLocal => dynamicObject != null;
    public Vector3 rawPosition => _position;
    private Vector3 _position;

    public Fixation(Vector3 position, Vector3 surfaceNormal, int gfid, int lfid = -1, DynamicObject dynamicObject = null)
    {
        this.surfaceNormal = surfaceNormal;
        this.dynamicObject = dynamicObject;
        _position = position;
        this.gfid = gfid;

        this.lfid = isLocal ? lfid : gfid;
    }
}
