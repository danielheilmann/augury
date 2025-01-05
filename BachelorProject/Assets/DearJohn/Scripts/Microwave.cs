using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microwave : QuestItem
{
    public const string CanBreakMicrowaveTag = "CanBreakMicrowave";
    [SerializeField] private GameObject fixedMicrowaveModel;
    [SerializeField] private GameObject brokenMicrowaveModel;

    private void Start()
    {
        SetBroken(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.type == Type.BrokenMicrowave) return; //< No need to evaluate further collisions if the microwave is already broken.

        if (other.CompareTag(CanBreakMicrowaveTag))
            SetBroken(true);
    }

    private void SetBroken(bool isBroken)
    {
        if (isBroken) type = Type.BrokenMicrowave;
        else type = Type.FixedMicrowave;

        fixedMicrowaveModel.SetActive(!isBroken);
        brokenMicrowaveModel.SetActive(isBroken);
    }
}
