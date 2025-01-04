using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public class ReplayAction
{
    public enum ActionType { GazePoint, DynamicObject }

    public ActionType type;
}

public class ReplayActionGP : ReplayAction
{
    public DateTime timestamp;
    public Vector3 surfaceNormal;
    public DynamicObject dynamicObjectReference;
    public string dynObjID;
    public Vector3 globalPosition;

    public ReplayActionGP(DateTime timestamp, Vector3 globalPosition, Vector3 surfaceNormal, string dynObjID)
    {
        type = ActionType.GazePoint;

        this.timestamp = timestamp;
        this.surfaceNormal = surfaceNormal;
        this.globalPosition = globalPosition;
        this.dynObjID = dynObjID;
        dynamicObjectReference = string.IsNullOrEmpty(dynObjID) ? null : DynamicObjectManager.GetByID(dynObjID);
    }
}

public class ReplayActionDO : ReplayAction
{
    public DynamicObject referenceDynObj;
    public Vector3? updatedPosition = null;
    public Quaternion? updatedRotation = null;
    public Vector3? updatedScale = null;

    public ReplayActionDO(DynamicObject referenceDynObj, Vector3? updatedPosition = null, Quaternion? updatedRotation = null, Vector3? updatedScale = null)
    {
        type = ActionType.DynamicObject;

        this.referenceDynObj = referenceDynObj;
        this.updatedPosition = updatedPosition;
        this.updatedRotation = updatedRotation;
        this.updatedScale = updatedScale;
    }
}
