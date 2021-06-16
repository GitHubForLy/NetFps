using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using UnityEngine;
using System.Net.Sockets;
using JsonFormatter;
using ServerCommon;
using AopCore;
using ProtobufProto.Model;
using ProtobufProto;

namespace Game.Net
{
    /// <summary>
    /// 事件回调函数
    /// </summary>
    public delegate void EventCallback<T>(T Data);
    public delegate void TcpMessageEventHandle(TcpFlags action, Func<Type, object> formatter);

    class EventList
    {
        private Dictionary<TcpFlags,(Type,Delegate)> queue=new Dictionary<TcpFlags, (Type, Delegate)>();
        public int Count => queue.Count;
        public void Add<T>(TcpFlags action, EventCallback<T> callback)
        {
            if (queue.ContainsKey(action))
                queue[action] = (typeof(T),callback);
            else
                queue.Add(action, (typeof(T), callback));
        }

        public bool ContainsAction(TcpFlags action) => queue.ContainsKey(action);

        public EventCallback<T> Get<T>(TcpFlags action)
        {
            return (EventCallback<T>)queue[action].Item2;
        }
        public (Type,Delegate) Get(TcpFlags action)
        {
            return queue[action];
        }

        public bool Remove(TcpFlags action)
        {
            return queue.Remove(action);
        }
    }


    /// <summary>
    /// 请求服务器
    /// </summary>
    public class TcpRequest
    {
        private AsyncTcpClient NetClient;
        private int eventReqId=0;
        private EventList eventList = new EventList();

        private static TcpRequest instance;
        private EndPoint endPoint;
        private ProtobufFormatter formatter;

        public event TcpMessageEventHandle OnRecevieMessage;
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected => NetClient.IsConnected;

        ///// <summary>
        ///// 广播消息队列 
        ///// </summary>
        //public BroadcastQueue BroadQueue {get;} =new BroadcastQueue();

        /// <summary>
        /// 与服务器连接出错
        /// </summary>
        public event Action<SocketException> OnConnectionError;

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static TcpRequest Instance
        {
            get
            {
                if (instance == null)
                    instance = new TcpRequest();
                return instance;
            }
        }


        private TcpRequest()
        {
            formatter=new ProtobufFormatter();
            Init();
            //eventWait = new AutoResetEvent(false);
        }

        private void Init()
        {
            NetClient = new AsyncTcpClient();
            NetClient.OnReceiveData += NetClient_OnReceiveData;
            NetClient.OnConnectException += NetClient_OnConnectException;
        }

        private void NetClient_OnConnectException(object sender, SocketException e)
        {
            OnConnectionError?.Invoke(e);
        }


        /// <summary>
        /// 连接到服务器并开始接收数据
        /// </summary>
        public void Start(EndPoint endPoint)
        {
            this.endPoint = endPoint;
            NetClient.Connect(endPoint);
            NetClient.StartRecive();
        }


        public void Close()
        {
            NetClient.Close();
        }

        /// <summary>
        /// 重新连接服务器并接受数据
        /// </summary>
        public void ReStart()
        {
            if (NetClient.IsConnected)
                NetClient.Close();

            Init();
            Start(endPoint);
        }


        private void NetClient_OnReceiveData(object sender, byte[] data)
        {
            data = ProtobufDataPackage.UnPackageData(data, out TcpFlags action);

            //先调用事件
            GameController.AddDelegate(() =>
            {
                OnRecevieMessage?.Invoke(action, t =>
                {
                    try
                    {
                        return formatter.Deserialize(t, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("反序列化错误, action:" + action + "  message:" + e.Message);
                        return null;
                    }
                });
            });

            //调用对应的请求方法回调
            if (eventList.ContainsAction(action))
                CallEvent(action, data);
        }


        private void CallEvent(TcpFlags action,byte[] data)
        {
            if (eventList.ContainsAction(action))
            {
                (Type datatype, Delegate callback) dega = eventList.Get(action);
                object obj= formatter.Deserialize(dega.datatype, data);

                GameController.AddDelegate(() =>
                {
                    dega.callback.DynamicInvoke(obj);
                });
                eventList.Remove(action);
            }
        }


        /// <summary>
        /// 请求消息
        /// </summary>
        /// <param name="ActionName"></param>
        public void DoRequest(TcpFlags action)
        {
            DoRequest(null, action);
        }

        /// <summary>
        /// 请求消息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ActionName"></param>
        public void DoRequest(object request,TcpFlags action)
        {
            DoRequest<ProtoBuf.IExtensible>( request, action, null,null);
        }

        /// <summary>
        /// 使用指定回调函数请求
        /// </summary>
        public void DoRequest<ResType>(object request, TcpFlags action, EventCallback<ResType> callback)/*where ResType:ProtoBuf.IExtensible*/
        {
            DoRequest(request, action, callback, null);
        }

        /// <summary>
        /// 使用指定回调函数请求
        /// </summary>
        public void DoRequest<ResType>(object request, TcpFlags action, Action ComplatedCallback) /*where ResType : ProtoBuf.IExtensible*/
        {
            DoRequest<ResType>(request, action, null, ComplatedCallback);
        }

        /// <summary>
        /// 使用指定回调函数请求
        /// </summary>
        public void DoRequest<ResType>(object request, TcpFlags action, EventCallback<ResType> callback,Action ComplatedCallback) /*where ResType : ProtoBuf.IExtensible*/
        {
            if(!NetClient.IsConnected)
                ReStart();

            //if (Delegates.Count > 0 || eventList.Count > 0 && ComplatedCallback!=null)//如果还有没处理的就不能重复发
            //{
            //    Debug.LogWarning(action + " 请求被撤销, 还有请求没有处理完！");
            //    return;
            //}

            var data= formatter.Serialize(request);
            data=ProtobufDataPackage.PackageData(action,data);
            if (callback != null)
            {
                eventList.Add(action, callback);
            }
            NetClient.SendDataAsync(data, ComplatedCallback);
        }


        ///// <summary>
        ///// 广播推送消息
        ///// </summary>
        //public void Broadcast(object request, TcpFlags ActionName)
        //{
        //    Request req = new Request
        //    {
        //        Action = ActionName,
        //        Controller = ControllerNames.Broadcast,
        //        Data =request
        //    };

        //    NetClient.SendDataAsync(formatter.Serialize(req)); 
        //}

        ///// <summary>
        ///// 广播方法的调用
        ///// </summary>
        //public void BroadcastMethod(BroadcastMethod method)
        //{
        //    Request req = new Request
        //    {
        //        Action = nameof(BroadcastMethod),
        //        Controller = ControllerNames.Broadcast,
        //        Data = method
        //    };

        //    NetClient.SendDataAsync(formatter.Serialize(req));
        //}

        ///// <summary>
        ///// 广播字段复制
        ///// </summary>
        //public void BroadcastField(FieldUpdateArgs args)
        //{

        //}
    }
}
