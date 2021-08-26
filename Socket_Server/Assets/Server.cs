using System.Security.AccessControl;
using System.Net;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;


public class Server : MonoBehaviour
{
     Boolean isClose;
      //开启线程
      void Awake()
      {
           Thread t1 = new Thread(OpenServer);
           t1.Start();
      }

     //开启服务端
     public void OpenServer()
     {
          Socket listenserver = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
          IPAddress ipAds = IPAddress.Parse("127.0.0.1");
          IPEndPoint ipEnd = new IPEndPoint(ipAds,8080);
          listenserver.Bind(ipEnd);
          print($"<color=#ffff00>------------------服务器启动</color>");

          //开启监听
          listenserver.Listen(0);
          print($"<color=#ffff00>-----------------开启监听</color>");

          while(true)
          {
               Socket connectserver = listenserver.Accept();
               print("连接客户端"+connectserver.RemoteEndPoint);

               Byte[] serverByte = new Byte[1024];
               connectserver.Receive(serverByte);
               string serverStr = System.Text.Encoding.Default.GetString(serverByte);
               print($"<color=#ffff00>------获取从客户端传值     </color>"+serverStr);
               string logStr = "服务端返回-------"+serverStr;
              // string serverDisposeStr = serverStr+logStr;
               print("============================="+logStr);
               Byte[] serverDisposeByte = System.Text.Encoding.Default.GetBytes(logStr);
               connectserver.Send(serverDisposeByte);
               

               if (isClose)
               {
                    print($"<color=#ffff00>---------------------服务器关闭</color>");
                    connectserver.Close();                   
               }
          }
         
     }
    
     void OnDestroy()           
     {
          isClose = true;
        //  print("Destroy");
     }
}
