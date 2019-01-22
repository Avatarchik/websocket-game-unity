using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public void Spawn()
    {
        GameObject newGo = new GameObject("Bot");
        newGo.AddComponent<Bot>();
    }

    void Start()
    {
    }
}
