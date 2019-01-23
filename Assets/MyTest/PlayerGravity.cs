using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public CharacterController controller;

    void Gravity()
    {
        controller.Move(new Vector3(0f, -9.8f * 100f * Time.deltaTime, 0f));
    }

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        Gravity();
    }
}
