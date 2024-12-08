using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GazePoint  //TODO: For some reason, having this as structs makes it so that it cannot simply be overwritten using the Set() method. And just creating a new object every tick leads to noticable frame drops.
{
    public string name;
    public DateTime timeStamp { get; private set; }
    public Vector3 position => isLocal ? dynamicObject.transform.position + _position : _position;
    public Vector3 rawPosition => _position;
    private Vector3 _position;
    public DynamicObject dynamicObject { get; private set; } = null;
    public bool isLocal => dynamicObject != null;
    public int dynObjID => isLocal ? dynamicObject.id : -1;

    public GazePoint(DateTime timeStamp, Vector3 position)
    {
        this.timeStamp = timeStamp;
        this._position = position;
        this.name = "";
    }

    public void Set(DateTime timeStamp, Vector3 position, DynamicObject connectedDynObj = null)
    {
        this.timeStamp = timeStamp;
        this._position = position;
        this.name = timeStamp + " " + position;

        this.dynamicObject = connectedDynObj;
    }
}

//TODO: Maybe implement something like a "GazePointParser" static helper class here with "ToJSON" and "FromJSON" methods for parsing, so that these functionalities do not have to be in the FileHandler class.