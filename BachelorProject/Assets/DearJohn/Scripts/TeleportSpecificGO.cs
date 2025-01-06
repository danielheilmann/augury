using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpecificGO : MonoBehaviour
{
    
    [SerializeField] private GameObject objectToTeleport;

    public void Teleport()
    {
        if (SessionManager.currentMode == SessionManager.DataMode.Replay)
        {
            Debug.LogWarning("Teleportation is not permitted in Replay mode");
            return;
        }

        GameObject objectToTeleport = GameManager.Instance.PlayerCharacter;
        if (objectToTeleport == null)
        {
            Debug.LogError("Either no object has been assigned or the assigned object was deleted.");
            return;
        }
        GameManager.Instance.PlayerCharacter.transform.position = this.transform.position;
    }
}
