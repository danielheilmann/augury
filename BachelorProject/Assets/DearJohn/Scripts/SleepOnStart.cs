using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepOnStart : MonoBehaviour
{
    void Start() => GetComponent<Rigidbody>().Sleep();
}
