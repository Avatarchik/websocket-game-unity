using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float forwardSpeed = 3;
    public float strafeSpeed = 3;
    public float runMultiplier = 1.5f;
    public KeyCode runKey = KeyCode.LeftShift;
    public LayerMask groundLayer;

    RaycastHit hit;
    float hoverHeight = 0;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastEuler = Vector3.zero;
    private Vector3 lastScale = Vector3.one;

    public Player player;
    public ConnectionAgent connectionAgent;

    void UpdateNetTransform()
    {
        bool needUpdate = false;
        Vector3 nowPos = transform.position;
        if (nowPos.x != lastPos.x || nowPos.y != lastPos.y || nowPos.z != lastPos.z)
        {
            lastPos = nowPos;
            needUpdate = true;
        }

        Vector3 nowEuler = transform.eulerAngles;
        if (nowEuler.x != lastEuler.x || nowEuler.y != lastEuler.y || nowEuler.z != lastEuler.z)
        {
            lastEuler = nowEuler;
            needUpdate = true;
        }

        Vector3 nowScale = transform.localScale;
        if (nowScale.x != lastScale.x || nowScale.y != lastScale.y || nowScale.z != lastScale.z)
        {
            lastScale = nowScale;
            needUpdate = true;
        }

        if (needUpdate)
        {
            // # (both) 更新玩家位置:玩家id|localPosition|localRotation|localScale
            string data = string.Format(
                "{0}|{1}|{2}|{3}",
                player.playerId,
                connectionAgent.ParseVectorToString(nowPos),
                connectionAgent.ParseVectorToString(nowEuler),
                connectionAgent.ParseVectorToString(nowScale)
            );
            connectionAgent.SendCmd(Cmd.player_transform, data);
        }
    }

    void Awake()
    {
    }

    void Start()
    {
        player = gameObject.GetComponent<Player>();
        connectionAgent = GameObject.Find("GameLogic").GetComponent<ConnectionAgent>();
    }

    void Update()
    {

        float y = Input.GetAxis("Vertical") * forwardSpeed * (Input.GetKey(runKey) ? runMultiplier : 1) * Time.deltaTime;
        float x = Input.GetAxis("Horizontal") * strafeSpeed * Time.deltaTime;

        // hover
        if (Physics.Raycast(transform.position + Vector3.up * 9999, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            hoverHeight = hit.point.y + 1.8f;
        }


        transform.Translate(new Vector3(x, hoverHeight - transform.position.y + 1.8f, y));

        UpdateNetTransform();
    }
}
