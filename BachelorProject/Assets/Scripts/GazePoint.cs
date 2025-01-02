using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GazePoint  //!< For some reason, having this as structs makes it so that it cannot simply be overwritten using the Set() method. And just creating a new object every tick leads to noticable frame drops. Therefore, I chose to implement them as class and then cache / preload them into memory (in the GazePointManager)
{
    public string name; //< To be identifiable in a serialized list
    public DateTime timeStamp { get; private set; }
    public Vector3 surfaceNormal { get; private set; }
    public DynamicObject dynamicObject { get; private set; } = null;
    public bool isLocal => dynamicObject != null;
    public int dynObjID => isLocal ? dynamicObject.id : -1;
    public Vector3 position => isLocal ? dynamicObject.transform.position + _position : _position;
    public Vector3 rawPosition => _position;
    private Vector3 _position;

    public GazePoint Set(DateTime timeStamp, Vector3 position, Vector3 surfaceNormal, DynamicObject connectedDynObj = null)
    {
        this.timeStamp = timeStamp;
        this._position = position;
        this.surfaceNormal = surfaceNormal;
        this.name = timeStamp + " " + position;

        this.dynamicObject = connectedDynObj;

        return this;
    }
}

//?> Maybe move "ToJSON" and "FromJSON" methods into this class for easier parsing, so that these functionalities do not have to be in the FileHandler class.