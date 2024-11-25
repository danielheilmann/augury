using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool OpenExplorerOnSave = false;
    public static float expectedSessionRuntimeInMinutes = 10f;  //< This value is used to calculate the initial buffer size for the gazepoint dictionary based on session duration and tick rate.
}
