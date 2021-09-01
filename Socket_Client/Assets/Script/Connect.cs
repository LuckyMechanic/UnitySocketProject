using System.Runtime.CompilerServices;
using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;

/// <summary>
/// 异步发送
/// </summary>
public class Connect : MonoBehaviour
{
    public InputField inputFieldA;
    public Text receiveText;
    Socket socket;
    //接收缓冲区
    byte[] readBuff = new byte[1024];
    string recvStr = "";

    public void AsynchronousSend()
    {
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socket.BeginConnect("127.0.0.1",8080,ConnectionCallback,socket);

        string sendStr = inputFieldA.text;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
        // for (int i = 0; i < 10000; i++)
        // {
        //       socket.BeginSend(sendBytes,0,sendBytes.Length,0,SendCallback,socket);            
        // }
    }

     void Update()
     {
        // print("-------------");
         receiveText.text = recvStr;
     }

    //connect 回调
    public void ConnectionCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            socket.EndConnect(ar);
            print("Socket connect success");  
            socket.BeginReceive(readBuff,0,1024,0,ReceiveCallback,socket);
              
        }
        catch (SocketException ex)
        {            
            print("Scoket connect fail "+ex.ToString());
        }
    }

    //receiveCallback 回调
    public void ReceiveCallback(IAsyncResult ar)
    {
         try
         {
             Socket socket = (Socket) ar.AsyncState;
             int count = socket.EndReceive(ar);
             recvStr = System.Text.Encoding.Default.GetString(readBuff,0,count);
             socket.BeginReceive(readBuff,0,1024,0,ReceiveCallback,socket);
         }
         catch(SocketException ex)
         {
             print("Socket Receive fail"+ex.ToString());
         }
    }

    //send 回调
    public void SendCallback(IAsyncResult ar)
    {   
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndSend(ar);
            print("Socket Send success"+count);
        }
        catch(SocketException ex)
        {
            print("Socket Send fail"+ex.ToString());
        }
    }
}
