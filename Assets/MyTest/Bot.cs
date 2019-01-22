using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public string[] playerNameDefault = {
        "大雄",
        "多拉A夢",
        "胖虎",
        "小夫",
        "靜香",
    };
    public float playerInitHp = 10f;

    public int myPlayerId = 0;
    public string myPlayerName = "unknow";
    public float myPlayerHp = 0;

    private WebsocketConnection websocketConnection;
    private ConnectionAgent connectionAgent;

    public enum State
    {
        Init,

        OpenConnection,
        PlayerJoin,

        AuotPlaying,
    }
    private State state = State.OpenConnection;
    private State stateLast = State.Init;

    public enum AuotPlayingState
    {
        Init,

        Idle,
        Move,
    }
    private AuotPlayingState autoPlayingState = AuotPlayingState.Idle;
    private AuotPlayingState autoPlayingStateLast = AuotPlayingState.Init;

    Vector3 botPosition = Vector3.zero;
    Vector3 botEuler = Vector3.zero;
    Vector3 botScale = Vector3.one;

    float botIdleTimer = 0f;
    float botIdleToMoveWaitSeconds = 0f;

    Vector3 botMoveVector = Vector3.forward;
    float botMoveSpeed = 0f;
    float botMoveTimer = 0f;
    float botMoveToIdleWaitSeconds = 0f;

    private int mySocketId = -1;

    void OnSocketOpen(string data)
    {
        connectionAgent.UnregisterCmdHandler(Cmd.socket_open);

        mySocketId = int.Parse(data);
        myPlayerId = mySocketId; // 用mySocketId當作playerId

        state = State.PlayerJoin;
    }

    void PlayerJoin()
    {
        PlayerSpawnPoint psp = GameObject.FindObjectOfType<PlayerSpawnPoint>();
        Vector3 bornPos = psp.GetSpawnPoint();

        string data = string.Format(
            "{0}|{1}|{2}|{3},{4},{5}|0,0,0|1,1,1",
            myPlayerId,
            playerNameDefault[Random.Range(0, playerNameDefault.Length - 1)],
            playerInitHp,
            bornPos.x, bornPos.y, bornPos.z
        );
        connectionAgent.SendCmd(Cmd.player_join, data);
    }

    void OnPlayerJoinDone(string data)
    {
        connectionAgent.UnregisterCmdHandler(Cmd.player_join_done);

        string[] dataSplit = data.Split('|');

        // player_join_done:0|大中天|10|1.2,1.2,1.2|0,0,0|1,1,1
        int playerId = int.Parse(dataSplit[0]);
        if (playerId != myPlayerId)
        {
            return;
        }
        else
        {
            string playerName = dataSplit[1];
            float playerHp = float.Parse(dataSplit[2]);
            Vector3 playerPos = connectionAgent.ParseStringToVector(dataSplit[3]);
            Vector3 playerEuler = connectionAgent.ParseStringToVector(dataSplit[4]);
            Vector3 playerScale = connectionAgent.ParseStringToVector(dataSplit[5]);

            botPosition = playerPos;
            botEuler = playerEuler;
            botScale = playerScale;

            state = State.AuotPlaying;
        }
    }

    //void OnPlayerTransform(string data)
    //{
    //    // # (both) 更新玩家位置:玩家id|localPosition|localRotation|localScale
    //    string[] dataSplit = data.Split('|');

    //    int playerId = int.Parse(dataSplit[0]);
    //    if (playerId == myPlayerId)
    //    {
    //        /*
    //                Vector3 pos = connectionAgent.ParseStringToVector(dataSplit[1]);
    //                Vector3 rot = connectionAgent.ParseStringToVector(dataSplit[2]);
    //                Vector3 sca = connectionAgent.ParseStringToVector(dataSplit[3]);
    //                transform.position = pos;
    //                transform.eulerAngles = rot;
    //                transform.localScale = sca;
    //        */
    //        return;
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}

    void UpdateState()
    {
        bool isStateChanged = false;
        if (state != stateLast)
        {
            isStateChanged = true;
            stateLast = state;
        }

        switch (state)
        {
            case State.OpenConnection:
                {
                    if (isStateChanged)
                    {
                        connectionAgent.RegisterCmdHandler(Cmd.socket_open, OnSocketOpen);
                        connectionAgent.connection.OpenConnention();
                        break;
                    }
                }
                break;
            case State.PlayerJoin:
                {
                    if (isStateChanged)
                    {
                        connectionAgent.RegisterCmdHandler(Cmd.player_join_done, OnPlayerJoinDone);
                        PlayerJoin();
                    }
                }
                break;
            case State.AuotPlaying:
                {
                    UpdateAutoPlayingState();
                }
                break;
        }
    }

    void UpdateAutoPlayingState()
    {
        bool isStateChanged = false;
        if (autoPlayingState != autoPlayingStateLast)
        {
            isStateChanged = true;
            autoPlayingStateLast = autoPlayingState;
        }

        switch (autoPlayingState)
        {
            case AuotPlayingState.Init:
                {
                    botPosition = Vector3.zero;
                    botEuler = Vector3.zero;
                    botScale = Vector3.one;

                    botIdleTimer = 0f;
                    botIdleToMoveWaitSeconds = 0f;

                    botMoveVector = Vector3.forward;
                    botMoveSpeed = 0f;
                    botMoveTimer = 0f;
                    botMoveToIdleWaitSeconds = 0f;

                    autoPlayingState = AuotPlayingState.Idle;
                }
                break;
            case AuotPlayingState.Idle:
                {
                    if (isStateChanged)
                    {
                        botIdleTimer = 0f;
                        botIdleToMoveWaitSeconds = Random.Range(3f, 6f);
                        break;
                    }

                    botIdleTimer += Time.deltaTime;
                    if (botIdleTimer >= botIdleToMoveWaitSeconds)
                    {
                        autoPlayingState = AuotPlayingState.Move;
                    }
                }
                break;
            case AuotPlayingState.Move:
                {
                    if (isStateChanged)
                    {
                        botMoveVector = new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1));
                        botMoveVector.Normalize();

                        botMoveSpeed = 3f * Time.deltaTime;
                        botMoveTimer = 0f;
                        botMoveToIdleWaitSeconds = Random.Range(3f, 6f);
                        break;
                    }

                    botMoveTimer += Time.deltaTime;
                    botPosition += botMoveVector * botMoveSpeed;

                    // # (both) 更新玩家位置:玩家id|localPosition|localRotation|localScale
                    string data = string.Format(
                        "{0}|{1}|{2}|{3}",
                        myPlayerId,
                        connectionAgent.ParseVectorToString(botPosition),
                        connectionAgent.ParseVectorToString(botEuler),
                        connectionAgent.ParseVectorToString(botScale)
                    );
                    connectionAgent.SendCmd(Cmd.player_transform, data);

                    if (botMoveTimer >= botMoveToIdleWaitSeconds)
                    {
                        autoPlayingState = AuotPlayingState.Idle;
                    }
                }
                break;
        }
    }

    void Start()
    {
        websocketConnection = gameObject.AddComponent<WebsocketConnection>();
        connectionAgent = gameObject.AddComponent<ConnectionAgent>();

        connectionAgent.connection = websocketConnection;
    }

    void Update()
    {
        UpdateState();
    }
}
