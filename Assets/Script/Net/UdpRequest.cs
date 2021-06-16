using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ProtobufProto.Model;
using System;

namespace Game.Net
{
    public delegate T FormatterFunc<T>();
    public delegate void UdpMessageEventHandle(UdpFlags action, UdpRequest.Formatter formatter);




    /// <summary>
    /// udp请求
    /// </summary>
    public class UdpRequest
    {
        public class Formatter
        {
            private static ProtobufProto.ProtobufFormatter formatter = new ProtobufProto.ProtobufFormatter();
            private byte[] _data;
            public Formatter(byte[]data)
            {
                _data = data;
            }
            public T Deserailize<T>()
            {
                return formatter.Deserialize<T>( _data);
            }
        }

        private UdpClient udpClient;
        private static UdpRequest instance;
        private ProtobufProto.ProtobufFormatter formatter = new ProtobufProto.ProtobufFormatter();
        private IPEndPoint serverpt;
        private bool isRun = true;

        public event UdpMessageEventHandle OnReceiveMessage;

        public static UdpRequest Instance
        {
            get
            {
                if (instance == null)
                    instance = new UdpRequest();
                return instance;
            }
        }

        private UdpRequest()
        {
            serverpt = new IPEndPoint(IPAddress.Parse(GlobalConfig.ServerIp), GlobalConfig.ServerUdpPort);

            udpClient = new UdpClient();
            udpClient.Connect(serverpt);
            IPEndPoint pt = new IPEndPoint(IPAddress.Any, 0);

            byte[] buffer = new byte[50]; 
            udpClient.BeginReceive(func, null);

            void func(IAsyncResult res)
            {
                var data = udpClient.EndReceive(res, ref pt);

                if (data.Length != 0)
                {
                    try
                    {
                        data = ProtobufDataPackage.UnPackageData(data, out UdpFlags action);

                        GameController.AddDelegate(() =>
                        {
                            OnReceiveMessage?.Invoke(action, new Formatter(data));
                        });
                    }
                    catch(Exception e)
                    {
                        Debug.LogError("udp 接收处理出错:" + e.Message);
                    }
                }
                //if (!isRun)
                //    return;
                udpClient.BeginReceive(func, null);
            }
        }

        public void Close()
        {
            Debug.Log("udp close");
            udpClient.Close();
            isRun = false;
        }


        /// <summary>
        /// 向服务端发送udp数据
        /// </summary>
        public void Request(UdpFlags action,object message)
        {
            var data=formatter.Serialize(message);
            data=ProtobufDataPackage.PackageData(action, data);
            //udpClient.Send(data, data.Length);
            udpClient.Send(data,data.Length);
        }
    }

}
