using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Net.Sockets;
using System.Net;
using LitJson;

namespace Game_Server
{
    class Program
    {
        static string isRegister;
        static byte[] toClientByte;
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipend = new IPEndPoint(ipaddress,8080);

            socket.Bind(ipend);
            Console.WriteLine("服务端启动成功");
            socket.Listen(0);
            Console.WriteLine("开启监听");

            while (true)
            {
                Socket NewSocket = socket.Accept();
                Console.WriteLine("客户端连接成功  "+NewSocket.RemoteEndPoint);

               //服务端接收到客户端传过来的Json包
                byte[] ServerByte = new byte[2048];
                int count = NewSocket.Receive(ServerByte);
                String ServerJsonStr = System.Text.Encoding.UTF8.GetString(ServerByte,0, count);
                Console.WriteLine("服务端收到客户端传Json包："+ ServerJsonStr);       

                //读取数据库
                string s = "data source = localhost;database = register;user id = root;password = root;charset = utf8";
                //C#规定在类内部只能定义属性或者变量，并初始化，不能直接变量引用变量。
                MySqlConnection conn = new MySqlConnection(s);
                try
                {
                    conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                    Console.WriteLine(">>>>>>>>>>>服务端与数据库建立连接");
               

                    //对收到的Json包进行解析
                    JsonData deJson = JsonMapper.ToObject(ServerJsonStr);
                    string judgeTitle = deJson["router"].ToString();
                    Console.WriteLine(judgeTitle);
                    switch (judgeTitle)
                    {
                        case "register":  //连接数据库进行注册操作 

                            //收到用户注册信息
                            Console.WriteLine(">>>>>>服务端收到客户端传注册信息：");

                            string userName = deJson["data"]["username"].ToString();
                            string psw = deJson["data"]["passward"].ToString();
                            Console.WriteLine(">>>>>>服务端收到客户端传注册用户名：" + userName);
                            Console.WriteLine(">>>>>>服务端收到客户端传注册密码：" + psw);
                            string sqlInsert = $"insert into test(user,passward) values('{userName}','{psw}')";
                            //string sql = "delete from user where userid='9'";
                            //string sql = "update user set username='啊哈',password='123' where userid='8'";
                            MySqlCommand cmdInsert = new MySqlCommand(sqlInsert, conn);
                            int result = cmdInsert.ExecuteNonQuery();//3.执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                            break;

                        case "login": //连接数据库进行登录操作  

                            //获取用户输入数据
                            string userNameLogin = deJson["data"]["username"].ToString();
                            string pswLogin = deJson["data"]["passward"].ToString();
                            Console.WriteLine(">>>>>>服务端收到客户端传登录信息：");

                            //查询数据库
                            string sql = "select * from test where user='" + userNameLogin + "' and passward='" + pswLogin + "'";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            MySqlDataReader reader = cmd.ExecuteReader();//执行ExecuteReader()返回一个MySqlDataReader对象
                            //string readDataName = reader.GetString("user");
                            //string readDataPwd = reader.GetString("passward");
                            //Console.WriteLine(readDataName + readDataPwd);
                            if (reader.Read())
                            {
                                Console.WriteLine("数据库有");
                                Console.WriteLine(">>>>>>>>>>>>登录成功");
                                isRegister = "login_Success";

                                toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                NewSocket.Send(toClientByte);
                            }else
                            {
                                isRegister = "login_Wrong";
                                toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                NewSocket.Send(toClientByte);
                                Console.WriteLine(">>>>>>>>>>>>登录失败，用户名或密码错误");

                                //if (userNameLogin.Length != 0 && pswLogin.Length != 0)
                                //{
                                //    if (readDataName.Equals(userNameLogin))
                                //    {
                                //        Console.WriteLine(">>>>>>>>>>>>用户名校验通过");
                                //        if (readDataPwd.Equals(pswLogin))
                                //        {
                                //            Console.WriteLine(">>>>>>>>>>>>登录成功");
                                //            isRegister = "login_Success";

                                //            toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                //            NewSocket.Send(toClientByte);
                                //        }
                                //        else
                                //        {
                                //            isRegister = "password_Wrong";
                                //            Console.WriteLine(">>>>>>>>>>>>密码错误");
                                //            toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                //            NewSocket.Send(toClientByte);
                                //        }
                                //    }
                                //    else
                                //    {
                                //        isRegister = "userName_Wrong";
                                //        toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                //        NewSocket.Send(toClientByte);
                                //        Console.WriteLine(">>>>>>>>>>>>用户名错误");
                                //    }
                                //}
                                //else
                                //{
                                //isRegister = "userInput_Null";
                                //toClientByte = System.Text.Encoding.UTF8.GetBytes(isRegister);
                                //NewSocket.Send(toClientByte);
                                //Console.WriteLine(">>>>>>>>>>>>用户名或密码不能为空");
                                //}
                            }
                            Console.WriteLine(reader.Read());
                         
                            //判读数据库内是否有数据

                            //if (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                            //{
                          
                            //}                          
                            break;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }

            }                       
        }
    }
}
