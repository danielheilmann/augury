using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurrentPlayerToHere : MonoBehaviour
{
    public void Teleport()
    {
        GameObject objectToTeleport = GameManager.Instance.GetCurrentPlayerCharacter;
        if (GameManager.Instance.GetCurrentPlayerCharacter == null)
        {
            Debug.LogError("Either no object has been assigned or the assigned object was deleted.");
            return;
        }
        objectToTeleport.transform.position = this.transform.position;
        objectToTeleport.transform.rotation = this.transform.rotation;
    }
}
