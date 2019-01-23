using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{
    public Vector3 targetPosition;

    void Start()
    {
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
    }
}
