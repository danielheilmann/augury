using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GazePoint  //!< For some reason, having this as structs makes it so that it cannot simply be overwritten using the Set() method. And just creating a new object every tick leads to noticable frame drops. Therefore, I chose to implement them as class and then cache / preload them into memory (in the GazePointManager)
{
    public string name; //< Just to be identifiable in a serialized list
    public DateTime timeStamp { get; private set; }
    public Vector3 surfaceNormal { get; private set; }
    public DynamicObject dynamicObject { get; private set; } = null;
    public bool isLocal => dynamicObject != null;
    public string dynObjID => isLocal ? dynamicObject.id : "";
    public Vector3 globalPosition { get; private set; }
    public Vector3 localPosition => isLocal ? globalPosition - dynamicObject.transform.position : globalPosition;

    public GazePoint() { }
    public GazePoint(DateTime timeStamp, Vector3 globalPosition, Vector3 surfaceNormal, DynamicObject connectedDynObj = null)
    {
        Set(timeStamp, globalPosition, surfaceNormal, connectedDynObj);
    }

    public GazePoint Set(DateTime timeStamp, Vector3 localPosition, Vector3 surfaceNormal, DynamicObject connectedDynObj = null)
    {
        this.timeStamp = timeStamp;
        this.globalPosition = localPosition;
        this.surfaceNormal = surfaceNormal;
        this.name = timeStamp + " " + localPosition;

        this.dynamicObject = connectedDynObj;

        return this;
    }
}

//?> Maybe move "ToJSON" and "FromJSON" methods into this class for easier parsing, so that these functionalities do not have to be in the FileHandler class.