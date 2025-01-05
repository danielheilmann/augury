using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class QuestItemXRSocketInteractor : XRSocketInteractor
{
    public QuestItem.Type questItem;

    private new void OnValidate()
    {
        // this.gameObject.name = $"Socket - {questItem}";
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.TryGetComponent(out QuestItem questItem) && questItem.type == this.questItem;
    }

    [Obsolete]
    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.TryGetComponent(out QuestItem questItem) && questItem.type == this.questItem;
    }
}
