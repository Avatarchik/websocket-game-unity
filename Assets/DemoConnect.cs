//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityToolbox;

//public class DemoConnect : MonoBehaviour
//{
//    public Connect connect;
//    public DebugWindow debugWindow;

//    public InputField inputField;

//    public void ConnectServer()
//    {
//        connect.OpenConnention();
//    }

//    public void DisconnectServer()
//    {
//        connect.CloseConnention();
//    }

//    public void SendStr()
//    {
//        string str = inputField.text;

//        connect.SendStr(str);
//    }

//    public void SendBytes()
//    {
//        string str = inputField.text;
//        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

//        connect.SendBytes(bytes);
//    }

//    void Start ()
//    {
//        connect.onMessageStr += (str) =>
//        {
//            debugWindow.Log("(str) " + str, true);
//        };

//        connect.onMessageBytes += (bytes) =>
//        {
//            debugWindow.Log("(bytes) " + System.Text.Encoding.UTF8.GetString(bytes), true);
//        };

//        connect.onError += (str) =>
//        {
//            debugWindow.LogError(str, true);
//        };
//    }

//}
