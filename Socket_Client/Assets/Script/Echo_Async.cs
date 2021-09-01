using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;

public class Echo_Async : MonoBehaviour
{
    Socket socket;
    public InputField inputField;
    public Text text;

    //接收缓冲区
    byte[] readBuff = new byte[4096];
    string recvStr;
    public void AsyncConection()
    {
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socket.BeginConnect("127.0.0.1",8080,ConnectCallback,socket);
    }

    /// <summary>
    /// Connect 回调
    /// </summary>
    /// <param name="ar"></param>
    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            print("Socket  Connect Success");
            socket.BeginReceive(readBuff,0,4096,0,ReceiveCallback,socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail "+ex.ToString());
        }

    }

    /// <summary>
    /// Receive 回调
    /// </summary>
    /// <param name="ar"></param>
    public void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndReceive(ar);
            print("客户端收到字节流长度 "+count);
for (int i = 0; i < count; i++)
{
    print("========"+readBuff[i]);
}

            recvStr = System.Text.Encoding.UTF8.GetString(readBuff,0,count);    
            print(recvStr);
           // byte[] sByte = System.Text.Encoding.Default.GetBytes(recvStr);
            
        }
        catch (SocketException ex)
        {
            print("Socket Receive fail "+ex.ToString());
        }
    }

    public void send()
    {
        string sendStr = inputField.text;
        byte[] sendByte = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendByte);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = recvStr;
    }
}
