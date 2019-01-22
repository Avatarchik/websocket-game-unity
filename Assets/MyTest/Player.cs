using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId;
    public string playerName;
    public float playerHp = 0f;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastEuler = Vector3.zero;
    private Vector3 lastScale = Vector3.one;

    //public ConnectionAgent connectionAgent;

    //void UpdateNetTransform()
    //{
    //    bool needUpdate = false;
    //    Vector3 nowPos = transform.position;
    //    if (nowPos.x != lastPos.x || nowPos.y != lastPos.y || nowPos.z != lastPos.z)
    //    {
    //        lastPos = nowPos;
    //        needUpdate = true;
    //    }

    //    Vector3 nowEuler = transform.eulerAngles;
    //    if (nowEuler.x != lastEuler.x || nowEuler.y != lastEuler.y || nowEuler.z != lastEuler.z)
    //    {
    //        lastEuler = nowEuler;
    //        needUpdate = true;
    //    }

    //    Vector3 nowScale = transform.localScale;
    //    if (nowScale.x != lastScale.x || nowScale.y != lastScale.y || nowScale.z != lastScale.z)
    //    {
    //        lastScale = nowScale;
    //        needUpdate = true;
    //    }

    //    if (needUpdate)
    //    {
    //        // # (both) 更新玩家位置:玩家id|localPosition|localRotation|localScale
    //        string data = string.Format(
    //            "{0}|{1}|{2}|{3}",
    //            playerId,
    //            connectionAgent.ParseVectorToString(nowPos),
    //            connectionAgent.ParseVectorToString(nowEuler),
    //            connectionAgent.ParseVectorToString(nowScale)
    //        );
    //        connectionAgent.SendCmd(Cmd.player_transform, data);
    //    }
    //}

    void Start()
    {
        //connectionAgent = GameObject.Find("GameLogic").GetComponent<ConnectionAgent>();
    }

    void Update()
    {
        //UpdateNetTransform();
    }
}
