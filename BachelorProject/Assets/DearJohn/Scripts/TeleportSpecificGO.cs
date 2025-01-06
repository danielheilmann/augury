using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpecificGO : MonoBehaviour
{
    
    [SerializeField] private GameObject objectToTeleport;

    public void Teleport()
    {
        if (objectToTeleport == null)
        {
            Debug.LogError("Either no object has been assigned or the assigned object was deleted.");
            return;
        }
        objectToTeleport.transform.position = this.transform.position;
    }
}
