using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformToEveryone : MonoBehaviour
{
    public int playerId = 0;
    public ConnectionAgent agent;

    Vector3 lastPosition;
    Vector3 lastEuler;
    Vector3 lastScale;

    public void Init(int playerId, ConnectionAgent agent)
    {
        this.playerId = playerId;
        this.agent = agent;
    }

    void SyncTransform(Vector3 pos, Vector3 euler, Vector3 scale)
    {
        if (agent == null)
        {
            return;
        }

        // 更新玩家位置:玩家id|localPosition|localRotation|localScale
        string data = string.Format(
            "{0}|{1}|{2}|{3}",
            playerId,
            agent.ParseVectorToString(pos),
            agent.ParseVectorToString(euler),
            agent.ParseVectorToString(scale)
        );
        agent.SendCmd(Cmd.player_transform, data);
    }

    void Start()
    {
        lastPosition = transform.position;
        lastEuler = transform.eulerAngles;
        lastScale = transform.localScale;
    }

    void Update()
    {
        bool needSync = false;
        if (lastPosition != transform.position)
        {
            lastPosition = transform.position;
            needSync = true;
        }
        if (lastEuler != transform.eulerAngles)
        {
            lastEuler = transform.eulerAngles;
            needSync = true;
        }
        if (lastScale != transform.localScale)
        {
            lastScale = transform.localScale;
            needSync = true;
        }

        if (needSync)
            SyncTransform(transform.position, transform.eulerAngles, transform.localScale);
    }
}
