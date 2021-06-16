using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Net;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEditor;
using Game.UI;
using System.Runtime.InteropServices;
using ProtobufProto.Model;
using ServerCommon;
using Game.UI.Panel;

namespace Game
{
    public delegate void BroadcastEventHandle((string action, IDynamicType subdata) msg);

    public class NetManager : MonoBehaviour
    {
        private Dictionary<string, Dictionary<string, Type>> ReceiveTyps;
        //public string ServerHost = "localhost";   
        //public short ServerPort = 4789;  
        public int ServerReconnectSceonds = 10;

        public static NetManager Instance { get; private set; }
        public string LoginAccount { get; set; }
        public string Password { get; set; }
        public string LoginTimestamp { get; private set; }
        public bool IsLogin { get; private set; } = false;
        public bool IsShowTimeinfo { get; set; }
        public UserInfo UserInfo { get; private set; }

        /// <summary>
        /// 与服务器的单次延迟 单位秒
        /// </summary>
        public float TimeDelay { get; private set; }
        /// <summary>
        /// 与服务器的时差 比服务器时间大则正数 否则负数  单位秒
        /// </summary>
        public float ServerTimeDiff { get; private set; }
        /// <summary>
        /// 当前服务器的时间戳(低精度)
        /// </summary>
        public double ServerTime => GetServerTime();

        private bool IsNeedReconnect = false;
        private bool isReConnecting = false;
        private double ctime;
        private double time;
        private double serverbacktime;

        [DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag,int dwReserved);


        private void Awake()
        {
            if (Instance != null && Instance == this)
                return;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            var end = new IPEndPoint(IPAddress.Parse(GlobalConfig.ServerIp), GlobalConfig.ServerTcpPort);
            TcpRequest.Instance.OnConnectionError += Instance_OnConnectionError;
            TcpRequest.Instance.Start(end);
            int flag = 0;
            if (!InternetGetConnectedState(ref flag, 0))
                PanelManager.Instance.ShowMessageBox("未连接到网络，请连接到网络后重试");

            Application.logMessageReceived += Application_logMessageReceived;

            TcpRequest.Instance.OnRecevieMessage += DoHandle;


            StartCoroutine(CheckTime());
        }



        /// <summary>
        /// 检查时间 同时也代替为心跳包
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckTime()
        {
            if (!IsNeedReconnect && TcpRequest.Instance.IsConnected)
            {
                var tmptime = GetTime();
                TcpRequest.Instance.DoRequest<SingleDouble>(null, TcpFlags.TcpCheckTime, res =>
                {
                    ctime = GetTime();
                    var delay = (ctime - tmptime) / 2;     //与服务器延迟粗略的计算为 请求来回行程时间的一半
                    TimeDelay = (float)delay;

                    var servertime = res.Data + delay;  //此时的服务器时间=返回的服务器时间+单向延迟
                    ServerTimeDiff = (float)(ctime - servertime);    //与服务器的时差=当前时间-服务器时间

                    time = tmptime;
                    serverbacktime = res.Data;
                });
            }
            yield return new WaitForSeconds(2.5f);
            StartCoroutine(CheckTime());
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("net start");
        }


        private void OnGUI()
        {
            if(IsShowTimeinfo)
            {
                GUILayout.Label("上下行(rtt):" +(int)((ctime - time)*1000)+" ms");
                GUILayout.Label("预估上/下 行:" + (int)(TimeDelay*1000)+" ms");
                GUILayout.Label("服务器时差:" + (int)(ServerTimeDiff*1000)+" ms");
                GUILayout.Label("上行:" + (int)((serverbacktime - time)*1000)+" ms");
                GUILayout.Label("下行:"+(int)((ctime - serverbacktime)*1000)+" ms");                
            }
        }


        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            //if (type == LogType.Assert || type == LogType.Exception || type == LogType.Error)
            //    Game.UI.PanelManager.Instance.ShowMessageBox(type.ToString() + "  " + condition + " strace:" + stackTrace);
        }

        private void Instance_OnConnectionError(SocketException exception)
        {
            IsNeedReconnect = true;
        }


        private IEnumerator Reconnect()
        {
            PanelManager.Instance.ShowMessageBox($"连接出错将在{ServerReconnectSceonds}秒后重连");
            //EditorUtility.DisplayDialog("提示", $"连接出错将在{ServerReconnectSceonds}秒后重连", "确认");
            isReConnecting = true;
            for (int i = ServerReconnectSceonds; i >= 0; i--)
            {
                if (TcpRequest.Instance.IsConnected)
                    break;
                Debug.LogWarning($"连接出错,将在{i}秒后重连");
                yield return new WaitForSeconds(1);
            }
            Debug.LogWarning($"对服务器进行重连");
            if(!TcpRequest.Instance.IsConnected)
                TcpRequest.Instance.ReStart();
            isReConnecting = false;

            if (IsLogin)
                ReLogin();     
        }



        private void Update()
        {
            if (IsNeedReconnect && !isReConnecting)
                StartCoroutine(Reconnect());
            IsNeedReconnect = false;

            //InvokeBroadcastMethod();

            if (Input.GetKeyDown(KeyCode.BackQuote))
                IsShowTimeinfo = !IsShowTimeinfo;
        }

        ///// <summary>
        ///// 广播消息
        ///// </summary>
        //private void InvokeBroadcastMethod()
        //{
        //    while (TcpRequest.Instance.BroadQueue.Count > 0)
        //    {
        //        var msg = TcpRequest.Instance.BroadQueue.Dequeue();

        //        DoHandle(msg);
        //        //调用事件

        //        OnReceiveBroadcast?.Invoke(msg);
        //    }
        //}

        private void DoHandle(TcpFlags action,Func<Type,object> formatter)
        {
            if(action==TcpFlags.TcpLogout)
            {
                OnUserLoginOut((LogoutInfo)formatter(typeof(LogoutInfo)));
            }
        }

        /// <summary>
        /// 处理用户登出
        /// </summary>
        private void OnUserLoginOut(LogoutInfo info)
        {
            if (IsLogin)
            {
                if (info.UserAccount == LoginAccount)
                {
                    //被挤下来
                    if (info.Timestamp == LoginTimestamp)
                    {
                        Debug.Log("被挤下来了");
                        PanelManager.Instance.ShowMessageBox("你的账号已于别处登录", MessageBoxButtons.Ok, res =>
                        {
                            IsLogin = false;
                            if(GameController.CurrentSceneName=="StartScene")
                            {
                                PanelManager.Instance.CloseAll();
                                PanelManager.Instance.OpenPanel<LoginPanel>();
                            }
                            else
                                GameController.LoadScene<LoginPanel>(Scenes.Start);
                        });
                    }
                }
            }
        }




        public void Login(string account,string password,Action<Respone<(UserInfo info, string timestamp)>> callback)
        {
            LoginRequest loginRequest = new LoginRequest
            {
                Account = account,
                Password = password
            };
            TcpRequest.Instance.DoRequest<Respone<(UserInfo info,string timestamp)>>(loginRequest, TcpFlags.TcpLogin, (res) =>
            {
                if (res.IsSuccess)
                {
                    LoginAccount = account;
                    Password = password;
                    IsLogin = true;
                    LoginTimestamp = res.Data.timestamp;
                    UserInfo = res.Data.info;
                }
                //GameController.AddDelegate(() =>
                //{
                    callback?.Invoke(res);
                //});
            });
        }

        public void ReLogin()
        {
            if (!IsLogin)
                return;

            Login(LoginAccount, Password, res =>
            {
                if (!res.IsSuccess)
                {
                    PanelManager.Instance.ShowMessageBox("重新登录失败:" + res.Message);
                }
            });         
        }


        void OnDestroy()
        {
            if(Instance==this)
            {
                if (!IsLogin)
                {
                    TcpRequest.Instance.DoRequest<SingleInt>(null, TcpFlags.TcpLogout, () =>
                    {
                        TcpRequest.Instance.Close();
                    });
                }
                else
                {
                    TcpRequest.Instance.Close();
                }
                UdpRequest.Instance.Close();
            }           
        }


        public static double GetTime()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds;
        }

        public  double GetServerTime()
        {
             return GetTime() - ServerTimeDiff;
        }
    }
}


