using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamFadeOnGameEnd : MonoBehaviour
{
    [SerializeField] private CameraFade endFade;
    private void OnEnable()
    {
        GameManager.OnGameEnd.AddListener(TriggerFade);
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd.RemoveListener(TriggerFade);
    }

    private void TriggerFade()
    {
        endFade.enabled = true;
    }
}
