using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class WebsocketConnection : MonoBehaviour
{
    public string websocketServerSite = "ws://192.168.0.131:8080/websocket";

    private WebSocket ws = null;

    public delegate void OnStringMessageDelegate(string str);
    public delegate void OnBytesMessageDelegate(byte[] bytes);

    public event OnStringMessageDelegate onMessageStr;
    public event OnBytesMessageDelegate onMessageBytes;

    public event OnStringMessageDelegate onError;

    private Queue<string> strMessagesFromServer = new Queue<string>();
    private Queue<byte[]> bytesMessagesFromServer = new Queue<byte[]>();

    public void OpenConnention()
    {
        if (ws != null)
        {
            Debug.LogError("connection has already opened");
            return;
        }

        StartCoroutine(ConnectServer());
    }

    public void CloseConnention()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }

    public void SendStr(string str)
    {
        if (ws != null)
        {
            ws.SendString(str);
        }
    }

    public void SendBytes(byte[] bytes)
    {
        if (ws != null)
        {
            ws.Send(bytes);
        }
    }

    Thread threadListenServerMessage = null;
    void KeepListenServerMessage()
    {
        while (true)
        {
            if (ws == null)
                break;

            ListenServerMessage();

            if (ws.error != null && onError != null)
            {
                onError(ws.error);
                break;
            }
            continue;
        }
        ws.Close();
    }

    void ListenServerMessage()
    {
        string replyStr = ws.RecvString();
        if (replyStr != null)
        {
            //Debug.Log("raw replyStr=" + replyStr);
            strMessagesFromServer.Enqueue(replyStr);
        }

        byte[] replyBytes = ws.Recv();
        if (replyBytes != null)
        {
            //Debug.Log("raw replyBytes=" + replyBytes.ToString());
            bytesMessagesFromServer.Enqueue(replyBytes);
        }
    }

    void Start ()
    {
        Application.runInBackground = true;
    }

    void Update()
    {
        while (strMessagesFromServer.Count > 0)
        {
            string s = strMessagesFromServer.Dequeue();
            if (onMessageStr != null)
                onMessageStr(s);
            continue;
        }

        while (bytesMessagesFromServer.Count > 0)
        {
            byte[] bs = bytesMessagesFromServer.Dequeue();
            if (onMessageBytes != null)
                onMessageBytes(bs);
            continue;
        }
    }

    IEnumerator ConnectServer()
    {
        ws = new WebSocket(new Uri(websocketServerSite));
        yield return StartCoroutine(ws.Connect());
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
            ListenServerMessage();
        }

        threadListenServerMessage = new Thread(new ThreadStart(this.KeepListenServerMessage));
        threadListenServerMessage.Start();

        yield break;

        //while (true)
        //{
        //    string reply = ws.RecvString();
        //    if (reply != null && onMessageStr != null)
        //    {
        //        onMessageStr(reply);
        //    }

        //    byte[] bytes = ws.Recv();
        //    if (bytes != null && onMessageBytes != null)
        //    {
        //        onMessageBytes(bytes);
        //    }

        //    if (ws.error != null && onError != null)
        //    {
        //        onError(ws.error);
        //        break;
        //    }

        //    yield return null;
        //}
        //ws.Close();
    }

    private void OnDestroy()
    {
        CloseConnention();

        if (threadListenServerMessage != null)
        {
            threadListenServerMessage.Abort();
            threadListenServerMessage = null;
        }
    }
}
