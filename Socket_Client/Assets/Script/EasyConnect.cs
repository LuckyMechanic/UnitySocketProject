using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;

/// <summary>
/// Socket 客户端连接
/// </summary>
public class EasyConnect : MonoBehaviour
{
    public InputField inputField;
    public Text sendReceiveText;
    Socket socket;
   
   //连接
    public void send()
    {
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socket.Connect("127.0.0.1",8080);

        string sendStr = inputField.text;
        byte[] sendByte = System.Text.Encoding.Default.GetBytes(sendStr);

        socket.Send(sendByte);
        byte[] sendReceive = new byte[2048];
        
        socket.Receive(sendReceive);
        string sendReceiveStr = System.Text.Encoding.Default.GetString(sendReceive);
        sendReceiveText.text = sendReceiveStr;       
    }
}
