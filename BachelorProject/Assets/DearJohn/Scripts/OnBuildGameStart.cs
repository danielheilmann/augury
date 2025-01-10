using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To auto-start a recording session in the built version of the game, which in turn initializes the GameManager.
/// </summary>
public class OnBuildGameStart : MonoBehaviour
{
#if !UNITY_EDITOR
    void Start()
    {
        SessionManager.Instance.StartRecordSession();
    }
#endif
}
