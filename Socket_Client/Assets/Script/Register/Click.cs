using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Click : MonoBehaviour
{
    Socket socket;
    //让服务器区分登录和注册
    string toServerJudge;
    byte[] sendJudge;
    //登录模块 InputField
    public InputField userNameInputField;
    public InputField passWardInputField;

    //注册模块 InputField
    public InputField userNameInputFieldRegister;
    public InputField passWardInputFieldRegister;



    public void OnClick(int value)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socket.Connect(endPoint);
        print("与服务器连接成功");
        switch (value)
        {
            //点击注册按钮            
            case 0:
                string userNameRegisterStr = userNameInputFieldRegister.text;
                string passWardRegisterStr = passWardInputFieldRegister.text;

                //将注册信息打包成Json包
                JsonData register = new JsonData();
                register["router"] = "register";
                register["data"] = new JsonData();
                register["data"]["username"] = userNameRegisterStr;
                register["data"]["passward"] = passWardRegisterStr;

                string jsonDataRegister = JsonMapper.ToJson(register);
                print(jsonDataRegister);

                byte[] sendNameRegisterByte = System.Text.Encoding.UTF8.GetBytes(jsonDataRegister);
                socket.Send(sendNameRegisterByte);

                break;
            //点击登录按钮
            case 1:

                string userNameStr = userNameInputField.text;
                string passWardStr = passWardInputField.text;
                // print(userNameStr.Length);
                // print(passWardStr.Length);

                //将登录信息打包成Json包
                JsonData login = new JsonData();
                login["router"] = "login";
                login["data"] = new JsonData();
                login["data"]["username"] = userNameStr;
                login["data"]["passward"] = passWardStr;

                string jsonDataLogin = JsonMapper.ToJson(login);
                print(jsonDataLogin);
                byte[] sendByte = System.Text.Encoding.UTF8.GetBytes(jsonDataLogin);
                socket.Send(sendByte);


                byte[] sendReceiveByte = new byte[2048];
                int count = socket.Receive(sendReceiveByte);
                string isLoad = System.Text.Encoding.UTF8.GetString(sendReceiveByte, 0, count);

                switch (isLoad)
                {
                    case "login_Success":
                        print($"<color=#ffff00>登录成功</color>");
                        break;
                    case "login_Wrong":
                        print($"<color=#ffff00>登录失败，用户名或密码错误</color>");
                        break;                   
                }
                break;
        }
    }
}
