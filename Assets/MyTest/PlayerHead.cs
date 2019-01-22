using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public float speed = 100f;

    void Start()
    {
    }

    void Update()
    {
        float applyValue = Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0f, -applyValue, 0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0f, applyValue, 0f);
        }
    }
}
