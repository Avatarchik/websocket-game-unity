using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Toggle autoPlayToggle;
    public bool autoPlay = false;

    public string[] playerNameDefault = {
        "蕭敬騰",
        "蔡依林",
        "周杰倫",
        "伊能靜",
        "王力宏",
        "林依晨"
    };
    public float playerInitHp = 10f;

    public ConnectionAgent connectionAgent;

    public Player playerPrefab;
    public Player me;
    public int mySocketId = -1;

    private List<Player> playersInScene = new List<Player>();

    public Player GetPlayerById(int playerId)
    {
        foreach (Player p in playersInScene)
        {
            if (p.playerId == playerId)
                return p;
        }
        return null;
    }

    public void OpenSocket()
    {
        connectionAgent.connection.OpenConnention();
    }

    void OnSocketOpen(string data)
    {
        // 表示第一次連上server，把這個socket當成玩家自己
        if (mySocketId == -1)
        {
            mySocketId = int.Parse(data);
        }
        StartCoroutine(JoinTheGame());
    }

    IEnumerator JoinTheGame()
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

            me.gameObject.AddComponent<PlayerGravity>();
            me.name = "ME";

            SyncTransformToEveryone nst = me.gameObject.AddComponent<SyncTransformToEveryone>();
            nst.Init(newPlayer.playerId, connectionAgent);

            PlayerMove pm = me.gameObject.GetComponent<PlayerMove>();
            Joypad joypad = GameObject.FindObjectOfType<Joypad>();
            joypad.onPointerEventW.onPress.AddListener(pm.GoForward);
            joypad.onPointerEventS.onPress.AddListener(pm.GoBackward);
            joypad.onPointerEventA.onPress.AddListener(pm.GoLeft);
            joypad.onPointerEventD.onPress.AddListener(pm.GoRight);
            joypad.onPointerEventQ.onPress.AddListener(pm.TurnLeft);
            joypad.onPointerEventE.onPress.AddListener(pm.TurnRight);
            joypad.testAutoPlay = autoPlay;
        }
        else
        {
            GameObject.Destroy(newPlayer.GetComponentInChildren<AudioListener>());
            GameObject.Destroy(newPlayer.GetComponentInChildren<FlareLayer>());
            GameObject.Destroy(newPlayer.GetComponentInChildren<Camera>());
            newPlayer.gameObject.GetComponent<PlayerMove>().enabled = false;
            newPlayer.gameObject.GetComponent<CharacterController>().enabled = false;

            newPlayer.gameObject.AddComponent<LerpPosition>();
        }

        playersInScene.Add(newPlayer);
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

                    LerpPosition lp = playersInScene[i].GetComponent<LerpPosition>();
                    lp.targetPosition = pos;
                    //playersInScene[i].transform.position = pos;
                    playersInScene[i].transform.eulerAngles = rot;
                    playersInScene[i].transform.localScale = sca;
                }
            }
        }
    }

    void Start()
    {
        connectionAgent.RegisterCmdHandler(Cmd.socket_open, OnSocketOpen);
        connectionAgent.RegisterCmdHandler(Cmd.player_join_done, OnPlayerJoinDone);
        connectionAgent.RegisterCmdHandler(Cmd.player_transform, OnPlayerTransform);
    }

    void Update()
    {
        if (autoPlayToggle != null)
        {
            autoPlay = autoPlayToggle.isOn;
        }
    }
}
