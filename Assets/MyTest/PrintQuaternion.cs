using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintQuaternion : MonoBehaviour
{
    void Update()
    {
        Debug.Log(string.Format("{0},{1},{2},{3}", transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w));
    }
}
