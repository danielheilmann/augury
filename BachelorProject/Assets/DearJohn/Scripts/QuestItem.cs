using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class QuestItem : MonoBehaviour
{
    public enum Type { CardboardBox, FixedMicrowave, BrokenMicrowave, HamsterWheel, Battery, AntennaTV, Lever }

    public Type type;
    public Mesh model => GetComponentInChildren<MeshFilter>(false).sharedMesh;


    public void Strip()
    {
        Destroy(GetComponent<XRGrabInteractable>());
        Destroy(GetComponent<XRGeneralGrabTransformer>());
        if (type != Type.Lever)
            Destroy(GetComponent<Rigidbody>());
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

}
