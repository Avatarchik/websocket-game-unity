using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Vector3 randomShift = new Vector3(60f, 0f, 60f);

    public Vector3 GetSpawnPoint()
    {
        Vector3 myPos = transform.position;
        Vector3 spawnPos = new Vector3(
            Random.Range(-randomShift.x, randomShift.x) + myPos.x,
            Random.Range(-randomShift.y, randomShift.y) + myPos.y,
            Random.Range(-randomShift.z, randomShift.z) + myPos.z
        );

        return spawnPos;
    }
}
