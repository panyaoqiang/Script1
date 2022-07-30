using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine.UI;

public class socketClient : MonoBehaviour
{
    private Socket socket;
    private byte[] buffer = new byte[1024 * 1024 * 10];
    public Text text;
    public string assemlyInfo;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 3051);
        socket.Bind(ip);
        socket.Connect("127.0.0.1", 3050);//服务器ip及端口
        startReceive();
        Send();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            socket.Send(Encoding.UTF8.GetBytes(assemlyInfo));
        }
    }


    void startReceive()
    {
        socket.BeginReceive(buffer, 0, buffer.Length, 
            SocketFlags.None,receiveCallBack,null);
    }

    void receiveCallBack(IAsyncResult iar)
    {
        int len = socket.EndReceive(iar);
        if (len == 0)
        {
            return; 
        }
        string str = Encoding.UTF8.GetString(buffer,0,len);
        int k = Int32.Parse(str);
        print(k);
        startReceive();
    }

    void Send()
    {
        socket.Send(Encoding.UTF8.GetBytes("finish Connect"));
    }

}
