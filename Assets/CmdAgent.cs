using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cmd
{
    public static readonly string socket_open = "socket_open";
    public static readonly string player_join = "player_join";
    public static readonly string player_join_done = "player_join_done";
    public static readonly string player_leave = "player_leave";
    public static readonly string player_leave_done = "player_leave_done";
    public static readonly string player_transform = "player_transform";
    public static readonly string chat = "chat";
    public static readonly string create_bullet = "create_bullet";
    public static readonly string create_bullet_done = "create_bullet_done";
    public static readonly string bullet_transform = "bullet_transform";
    public static readonly string del_bullet = "del_bullet";
    public static readonly string del_bullet_done = "del_bullet_done";
    public static readonly string bullet_hit_player = "bullet_hit_player";
}

public class CmdAgent : MonoBehaviour
{
    public WebsocketConnection connection;

    public delegate void CmdHandler(string data);

    private Dictionary<string, CmdHandler> cmdHandlers = new Dictionary<string, CmdHandler>();

    public void RegisterCmdHandler(string cmd, CmdHandler handler)
    {
        if (handler != null)
        {
            cmdHandlers[cmd] = handler;
        }
        else
        {
            Debug.LogError("handler function is null");
        }
    }

    void Start()
    {
        connection.onMessageStr += (string str) =>
        {
            try
            {
                string[] strSplit = str.Split(':');
                string cmd = strSplit[0];
                string data = strSplit[1];

                if (cmdHandlers.ContainsKey(cmd))
                {
                    cmdHandlers[cmd](data);
                }
                else
                {
                    Debug.LogError("not support this command. " + cmd);
                }
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
                Debug.LogException(exc);
            }
        };
    }
}
