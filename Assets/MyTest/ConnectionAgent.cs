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

public class ConnectionAgent : MonoBehaviour
{
    public WebsocketConnection connection;

    public delegate void CmdHandler(string data);

    private Dictionary<string, CmdHandler> cmdHandlers = new Dictionary<string, CmdHandler>();

    public void RegisterCmdHandler(string cmd, CmdHandler handler)
    {
        if (cmdHandlers.ContainsKey(cmd))
        {
            Debug.LogError("cmd: " + cmd + " has registered");
            return;
        }

        if (handler != null)
        {
            cmdHandlers[cmd] = handler;
        }
        else
        {
            Debug.LogError("handler function is null");
        }
    }

    public void UnregisterCmdHandler(string cmd)
    {
        if (cmdHandlers.ContainsKey(cmd))
        {
            cmdHandlers.Remove(cmd);
        }
    }

    public void SendCmd(string cmd, string data)
    {
        string s = string.Format("{0}:{1}", cmd, data);
        connection.SendStr(s);
        Debug.Log("<color=green>" + "send to server=" + s + "</color>");
    }

    public string ParseVectorToString(Vector3 v)
    {
        try
        {
            string s = string.Format(
                "{0},{1},{2}",
                v.x.ToString("#0.00"),
                v.y.ToString("#0.00"),
                v.z.ToString("#0.00")
            );
            return s;
        }
        catch (System.Exception exc)
        {
            Debug.LogException(exc);
        }
        return null;
    }

    public Vector3 ParseStringToVector(string s)
    {
        try
        {
            string[] sSplit = s.Split(',');
            Vector3 result = Vector3.zero;

            result.x = float.Parse(sSplit[0]);
            result.y = float.Parse(sSplit[1]);
            result.z = float.Parse(sSplit[2]);

            return result;
        }
        catch (System.Exception exc)
        {
            Debug.LogException(exc);
        }
        return Vector3.zero;
    }

    public string ParseTransformToString(Transform t)
    {
        string s = string.Format(
            "{0},{1},{2}|{3},{4},{5}|{6},{7},{8}",
            t.localPosition.x.ToString("#0.00"), t.localPosition.y.ToString("#0.00"), t.localPosition.z.ToString("#0.00"),
            t.localRotation.eulerAngles.x.ToString("#0.00"), t.localRotation.eulerAngles.y.ToString("#0.00"), t.localRotation.eulerAngles.z.ToString("#0.00"),
            t.localScale.x.ToString("#0.00"), t.localScale.y.ToString("#0.00"), t.localScale.z.ToString("#0.00")
        );
        return s;
    }

    public void ParseStringToTransform(string s, ref Transform t)
    {
        try
        {
            string[] sSplit = s.Split('|');

            string pos = sSplit[0];
            string[] posSplit = pos.Split(',');
            Vector3 localPos = t.localPosition;
            float.TryParse(posSplit[0], out localPos.x);
            float.TryParse(posSplit[1], out localPos.y);
            float.TryParse(posSplit[2], out localPos.z);
            t.localPosition = localPos;

            string rot = sSplit[1];
            string[] rotSplit = rot.Split(',');
            Vector3 euler = t.localRotation.eulerAngles;
            float.TryParse(rotSplit[0], out euler.x);
            float.TryParse(rotSplit[1], out euler.y);
            float.TryParse(rotSplit[2], out euler.z);
            t.localRotation = Quaternion.Euler(euler);

            string sca = sSplit[2];
            string[] scaSplit = sca.Split(',');
            Vector3 localSca = t.localScale;
            float.TryParse(scaSplit[0], out localSca.x);
            float.TryParse(scaSplit[1], out localSca.y);
            float.TryParse(scaSplit[2], out localSca.z);
            t.localScale = localSca;
        }
        catch (System.Exception exc)
        {
            Debug.LogException(exc);
        }
    }

    void Start()
    {
        connection.onMessageStr += (string str) =>
        {
            try
            {
                Debug.Log("<color=yellow>" + "recv from server=" + str + "</color>");

                string[] strSplit = str.Split(':');
                string cmd = strSplit[0];
                string data = strSplit[1];

                if (cmdHandlers.ContainsKey(cmd))
                {
                    cmdHandlers[cmd](data);
                }
                else
                {
                    //Debug.LogError("not support this command. " + cmd);
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
