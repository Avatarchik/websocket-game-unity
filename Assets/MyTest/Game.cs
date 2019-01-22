using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public string[] playerNameDefault = {
        "中華一番之滿漢傳奇",
        "怪醫黑傑克",
        "金田一少年事件簿",
        "神風怪盜貞德",
        "愛情火辣辣",
        "美少女戰士"
    };
    public float playerInitHp = 10f;

    public ConnectionAgent connectionAgent;

    public Player playerPrefab;
    private Player me;
    public int mySocketId = -1;

    private List<Player> playersInScene = new List<Player>();

    public void OpenSocket()
    {
        connectionAgent.connection.OpenConnention();
    }

    void OnSocketOpen(string data)
    {
        // 表示第一次連上server，把這個socket當成玩家自己
        if (mySocketId == -1)
        {
            int.TryParse(data, out mySocketId);
        }
        StartCoroutine(PlayerJoin());
    }

    void OnPlayerTransform(string data)
    {
        // # (both) 更新玩家位置:玩家id|localPosition|localRotation|localScale
        string[] dataSplit = data.Split('|');

        bool ignore = false;
        int playerId = int.Parse(dataSplit[0]);
        if (me != null)
        {
            if (me.playerId == playerId)
            {
                ignore = true;
            }
        }

        if (ignore)
        {
            return;
        }
        else
        {
            for (int i = 0; i < playersInScene.Count; i++)
            {
                if (playersInScene[i] == null)
                    continue;

                if (playersInScene[i].playerId == playerId)
                {
                    Vector3 pos = connectionAgent.ParseStringToVector(dataSplit[1]);
                    Vector3 rot = connectionAgent.ParseStringToVector(dataSplit[2]);
                    Vector3 sca = connectionAgent.ParseStringToVector(dataSplit[3]);
                    playersInScene[i].transform.position = pos;
                    playersInScene[i].transform.eulerAngles = rot;
                    playersInScene[i].transform.localScale = sca;
                }
            }
        }
    }

    IEnumerator PlayerJoin()
    {
        yield return new WaitForEndOfFrame();

        PlayerSpawnPoint psp = GameObject.FindObjectOfType<PlayerSpawnPoint>();
        Vector3 bornPos = psp.GetSpawnPoint();

        string data = string.Format(
            "{0}|{1}|{2}|{3},{4},{5}|0,0,0|1,1,1",
            mySocketId, // 用mySocketId當作playerId
            playerNameDefault[Random.Range(0, playerNameDefault.Length - 1)],
            playerInitHp,
            bornPos.x, bornPos.y, bornPos.z
        );
        connectionAgent.SendCmd(Cmd.player_join, data);
    }

    void OnPlayerJoinDone(string data)
    {
        string[] dataSplit = data.Split('|');

        // player_join_done:0|大中天|10|1.2,1.2,1.2|0,0,0|1,1,1
        int playerId = int.Parse(dataSplit[0]);
        string playerName = dataSplit[1];
        float playerHp = float.Parse(dataSplit[2]);
        Vector3 playerPos = connectionAgent.ParseStringToVector(dataSplit[3]);
        Vector3 playerEuler = connectionAgent.ParseStringToVector(dataSplit[4]);
        Vector3 playerScale = connectionAgent.ParseStringToVector(dataSplit[5]);

        Player newPlayer = GameObject.Instantiate(playerPrefab);
        newPlayer.playerId = playerId;
        newPlayer.playerName = playerName;
        newPlayer.playerHp = playerHp;
        newPlayer.transform.position = playerPos;
        newPlayer.transform.rotation = Quaternion.Euler(playerEuler);
        newPlayer.transform.localScale = playerScale;

        if (newPlayer.playerId == mySocketId)
        {
            if (me != null)
            {
                GameObject.Destroy(me.gameObject);
            }
            me = newPlayer;
            PlayerHead ph =  me.gameObject.AddComponent<PlayerHead>();

            PlayerMove pm = me.gameObject.AddComponent<PlayerMove>();
            pm.groundLayer = LayerMask.GetMask("Default");
        }
        else
        {
            GameObject.Destroy(newPlayer.GetComponent<AudioListener>());
            GameObject.Destroy(newPlayer.GetComponent<FlareLayer>());
            GameObject.Destroy(newPlayer.GetComponent<Camera>());
        }

        playersInScene.Add(newPlayer);
    }

    void Start()
    {
        connectionAgent.RegisterCmdHandler(Cmd.socket_open, OnSocketOpen);
        connectionAgent.RegisterCmdHandler(Cmd.player_join_done, OnPlayerJoinDone);
        connectionAgent.RegisterCmdHandler(Cmd.player_transform, OnPlayerTransform);
    }

    void Update()
    {
        if (me != null)
        {
            //me.GetComponent<PlayerMove>().groundLayer = LayerMask.GetMask("Default");
        }
    }
}
