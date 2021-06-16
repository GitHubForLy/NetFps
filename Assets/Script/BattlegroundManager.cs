using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player;
using Game.Net;
using ServerCommon;
using System.Runtime.InteropServices;
using Game.UI;
using System.Linq;
using Game.UI.Panel;
using UnityEngine.UI;
using Pt = ProtobufProto.Model;

namespace Game
{
    public delegate void LogicDownEventHandle(Pt.PlayerOperations ops);
    public delegate void PlayerDiedEventHandle(BattlegroundManager.Player player,BattlegroundManager.Player killer);
    /// <summary>
    /// 这个类只应负责处理开始游戏后的单次对局 
    /// </summary>
    public class BattlegroundManager : MonoBehaviour
    {
        #region 对局开始前要设置的静态变量
        /// <summary>
        /// 所在房间的用户
        /// </summary>
        public static Pt.RoomUser[] Users { get;  set; }

        /// <summary>
        /// 所在的房间
        /// </summary>
        public static Pt.RoomInfo Room { get; set; }

        /// <summary>
        /// 回访历史帧
        /// </summary>
        public static Pt.TotalOpears ReplayOpears { get; set; }

        /// <summary>
        /// 是否是回放模式
        /// </summary>
        public static bool IsReplay { get; set; }
        #endregion



        public GameObject PlayerPrefab;
        public Transform[] SpawnPoints;

        //本地Player
        public Player LocalPlayer {get;set;}
        //当前操作信息
        public Pt.PlayerOperation LocalOperation { get; } = new Pt.PlayerOperation();

        public event LogicDownEventHandle OnLogicDown;
        public event PlayerDiedEventHandle OnPlayerDied;


        private bool inputopen = false;
        private bool escapeOpen = false;

        //上传操作的序号
        private int upIndex = 0;
        private int maxFrameIndex = 0;
        //丢失的帧序列
        private List<int> lackFrameIndexs = new List<int>();
        private Dictionary<int, Pt.PlayerOperations> frames = new Dictionary<int, Pt.PlayerOperations>();
        private int currentFrameIndex = 0;
        private bool isStart = false;

        public class Player
        {
            public int TeamNumber{get;set;}
            public Pt.RoomUser RoomUser{ get; set; }

            // public string Account{get;set;}
            public string UserName { get; set; }

            public GameObject Instance { get; set; }
            public bool IsDie
            { 
                get { return Instance==null||Instance.GetComponent<Health>().IsDie; } 
            }
        }

        /// <summary>
        /// 战场实例
        /// </summary>
        public static BattlegroundManager Instance { get;private set; }
        public static float StartTime;

        /// <summary>
        /// 全局坦克列表
        /// </summary>
        public Dictionary<string, Player> Players{get;}=new Dictionary<string, Player>();

        //private PlayerCamera cameraFollow;
        private float timescale;

        private void Awake()
        {
            //不需要调用  DontDestroyOnLoad 因为要让这个脚本只在TankScene中保持单例  而不是全局单例
            Instance = this;
            if(!IsReplay)
                TcpRequest.Instance.OnRecevieMessage += Tcp_OnReceiveBroadcast;
        }



        // Start is called before the first frame update
        void Start()
        {
            if (!NetManager.Instance.IsLogin)
                return;

            timescale = Time.timeScale;
            if(!IsReplay)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                PanelManager.Instance.DefaultCursroMode = CursorLockMode.Locked;
                PanelManager.Instance.DefaultCursorVisble = false;
                UdpRequest.Instance.OnReceiveMessage += Udp_OnReceiveMessage;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                PanelManager.Instance.DefaultCursroMode = CursorLockMode.None;
                PanelManager.Instance.DefaultCursorVisble = true;
            }

            StartGame();
        }




        //udp消息
        private void Udp_OnReceiveMessage(Pt.UdpFlags action, UdpRequest.Formatter formatter)
        {
            switch (action)
            {
                case Pt.UdpFlags.UdpDownFrame:
                    OnDownFrame(formatter.Deserailize<Pt.DownOperations>());
                    break;
                case Pt.UdpFlags.UdpDownLackFrame:
                    OnDownLackFrame(formatter.Deserailize<Pt.DownLackFrame>());
                    break;
                case Pt.UdpFlags.UdpBattleStart:
                    OnBattleStart();
                    break;
            }
        }

        //接收到丢帧数据
        private void OnDownLackFrame(Pt.DownLackFrame downLackFrame)
        {
            foreach(var frame in downLackFrame.Frames)
            {
                Debug.Log("接收到丢帧：" + frame.FrameIndex);

                lackFrameIndexs.Remove(frame.FrameIndex);
                frames[frame.FrameIndex] = frame.Operations;
            }
        }

        //下发帧数据
        private void OnDownFrame(Pt.DownOperations operations)
        {
            //记录丢帧
            for(int i=maxFrameIndex+1;i<operations.FrameIndex;i++)
            {
                lackFrameIndexs.Add(i);
                Debug.Log("lack...:"+i);
            }

            maxFrameIndex = operations.FrameIndex;

            //处理丢帧
            HandleLackFrame();

            frames[operations.FrameIndex] = operations.Operations;
        }

        //处理丢帧
        private void HandleLackFrame()
        {
            if (lackFrameIndexs.Count <= 0)
                return;
            Pt.ReqLackFrame req = new Pt.ReqLackFrame();
            req.Indexes = lackFrameIndexs.Take(GlobalConfig.ReqLackFrameCount).ToArray(); //一次最多只请求5帧

            UdpRequest.Instance.Request(Pt.UdpFlags.UdpReqFrame, req);
        }


        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            //Time.timeScale = 0;

            LocalPlayer.Instance.GetComponent<Game.Player.PlayerGui>().enabled = false;
            LocalPlayer.Instance.GetComponent<PlayerInput>().IsEnableInput = false;
            //cameraFollow.enabled = false;
        }
        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            //Time.timeScale = timescale;
            LocalPlayer.Instance.GetComponent<Game.Player.PlayerGui>().enabled = true;
            LocalPlayer.Instance.GetComponent<PlayerInput>().IsEnableInput = true;
        }

        /// <summary>
        /// 接收到tcp消息
        /// </summary>
        private void Tcp_OnReceiveBroadcast(Pt.TcpFlags action,System.Func<System.Type,object>formatter)
        {
            switch (action)
            {
                case Pt.TcpFlags.TcpLogout:
                    OnUserLoginOut((Pt.LogoutInfo)formatter(typeof(Pt.LogoutInfo)));
                    break;
                case Pt.TcpFlags.TcpDonwDataFail:
                    OnDataFail((Pt.DownFailStates)formatter(typeof(Pt.DownFailStates)));
                    break;
            }
        }

        //状态不同步
        private void OnDataFail(Pt.DownFailStates downFailStates)
        {
            //Debug.LogError("状态不同步   failaccount:" + downFailStates.FailAccount + "  frame:" + downFailStates.Frame);
        }

        private void OnUserLoginOut(Pt.LogoutInfo info)
        {
            if (info.UserAccount != NetManager.Instance.LoginAccount)
            {
                LoginoutTank(info.UserAccount);
            }
        }


        public void LoginoutTank(string account)
        {
            if(Players.ContainsKey(account)) 
            {
                Destroy(Players[account].Instance);
                Players.Remove(account);
            }
        }

        /// <summary>
        /// 是否本地玩家
        /// </summary>
        public bool IsLocalPlayer(NetIdentity identity)
        {
            if (identity == null)
            {
                Debug.LogWarning("identity is null");
                return true;
            }
            if (identity.Account == NetManager.Instance.LoginAccount)
                return true;
            return false;
        }


        private void StartGame()
        {
            if(Players.ContainsKey(NetManager.Instance.LoginAccount))
            {
                Debug.LogError("已经存在账号:"+ NetManager.Instance.LoginAccount);
                return;
            }

            if(SpawnPoints.Length>= Users.Length)
            {
                foreach(var user in Users)
                {
                    SpawnPlayer(user, SpawnPoints[user.Index].position, SpawnPoints[user.Index].eulerAngles);
                }
            }
            else
            {
                Debug.LogError("没有路点");
            }

            ResetLocalOperation();

            if (!IsReplay)
                InvokeRepeating(nameof(SendBattleReady), 0, 0.1f);
            else
                ReplayOperation();
        }


        //回放处理 
        private void ReplayOperation()
        {

            var canvas = GameObject.FindGameObjectWithTag(Tags.Canvas);
            PanelManager.Instance.SetupPanel<ReplayPanel>(canvas.transform);
        }


        public void AddFrame(int index,Pt.PlayerOperations operations)
        {
            frames[index] = operations;
        }

        //发送准备完成
        private void SendBattleReady()
        {
            UdpRequest.Instance.Request(Pt.UdpFlags.UdpGameReady, new Pt.SingleString() { Data = NetManager.Instance.LoginAccount });
        }

        //战场开始
        private void OnBattleStart()
        {
            if (isStart)
                return;
            isStart = true;
            CancelInvoke(nameof(SendBattleReady));
            InvokeRepeating(nameof(UpOperation), 0, GlobalConfig.UpOperDuration / 1000f);
            InvokeRepeating(nameof(LogicUpdate), 0, GlobalConfig.LogicUpdateDuration / 1000f);
            
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        public void LogicUpdate()
        {
            if (frames.ContainsKey(currentFrameIndex+1))
            {
                var ops= frames[currentFrameIndex + 1];

                Pt.UPDatas updata = new Pt.UPDatas();
                updata.Account = NetManager.Instance.LoginAccount;
                updata.Frame = currentFrameIndex + 1;

                OnLogicDown?.Invoke(ops);

                foreach (var op in ops.Operations)
                {
                    var data = GetInputData(op);
                    if(Players.ContainsKey(op.Account))
                        Players[op.Account].Instance.GetComponent<PlayerInput>().LogicUpdate(data);

                    //上传校验数据
                    var dt = UpDataCheck(currentFrameIndex + 1, Players[op.Account]);
                    updata.Datas.Add(op.Account, dt);

                    if (op.Account==NetManager.Instance.LoginAccount)
                    {
                        ResetLocalOperation();
                    }
                }


                UdpRequest.Instance.Request(Pt.UdpFlags.UdpUpDataCheck, updata);

                currentFrameIndex++;
            }
        }

        private Pt.UpDataCheck UpDataCheck(int frameIndex,Player player)
        {
            PlayerTarget target = player.Instance.GetComponent<PlayerTarget>();

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            var str = target.TargetPosition.ToRawString() +","+ target.TargetRotation.eulerAngles.ToRawString() +","+ target.TargetCameraRotation.eulerAngles.ToRawString();
            var bytes= System.Text.Encoding.UTF8.GetBytes(str);
            var hash= md5.ComputeHash(bytes);
            var hashstr = System.BitConverter.ToString(hash);


            Pt.UpDataCheck data = new Pt.UpDataCheck
            {
                Account = player.UserName,
                Hash =/*hashstr+"||"+*/ str 
            };
            return data;
        }

        private InputData GetInputData(Pt.PlayerOperation oper)
        {
            return new InputData
            {
                IsCrouch = oper.CrouchInput,
                InputJump = oper.JumpInput,
                InputMouseVector = new Vector2(oper.ForwardInput, oper.TurnInput ),
                InputSmoothLook=new Vector2(oper.MouseX,oper.MouseY),
                IsFire=oper.FireInput,
                IsReload=oper.ReloadInput
            };
        }

        //重置当前帧操作数据
        private void ResetLocalOperation()
        {
            LocalOperation.Account = NetManager.Instance.LoginAccount;

            var input = LocalPlayer.Instance.GetComponent<PlayerInput>();
            input.Data.Reset();
        }


        //上传逻辑帧操作
        private void UpOperation()
        {

            var input= LocalPlayer.Instance.GetComponent<PlayerInput>();

            LocalOperation.Account = NetManager.Instance.LoginAccount;
            LocalOperation.CrouchInput = input.Data.IsCrouch;
            LocalOperation.FireInput = input.Data.IsFire;
            LocalOperation.JumpInput = input.Data.InputJump;
            LocalOperation.ForwardInput =input.Data.InputMouseVector.x;
            LocalOperation.TurnInput = input.Data.InputMouseVector.y;
            LocalOperation.MouseX =input.Data.InputSmoothLook.x;
            LocalOperation.MouseY = input.Data.InputSmoothLook.y;
            LocalOperation.ReloadInput = input.Data.IsReload;

            Pt.UpFrame upFrame = new Pt.UpFrame()
            { 
                UpIndex= upIndex,
                Oper =LocalOperation 
            };
            upIndex++;
            //Debug.Log(LocalOperation.FireInput);
            UdpRequest.Instance.Request(Pt.UdpFlags.UdpUpFrame, upFrame);

            input.Data.IsFire = false;
        }



        /// <summary>
        /// 生成坦克
        /// </summary>
        public Player SpawnPlayer(ProtobufProto.Model.RoomUser user,Vector3 position,Vector3 rotation)
        {
           
            var account = user.Account;
            var instance = Instantiate(PlayerPrefab, position, Quaternion.Euler(rotation));
       
            instance.GetComponent<PlayerTeam>().TeamNumber = user.Team;
            instance.GetComponent<NetIdentity>().Account = account;

            Player player = new Player
            {
                TeamNumber = user.Team,
                Instance = instance,
                RoomUser = user,
                UserName = user.Info.UserName
            };

            //发送死亡事件
            instance.GetComponent<Health>().OnDie += (py, killer) => 
                OnPlayerDied?.Invoke(player, Players[killer.GetComponent<NetIdentity>().Account]);

            //本地玩家和网络玩家的处理
            if (user.Account == NetManager.Instance.LoginAccount)
            {
                LocalPlayer = player;
                instance.transform.Find("ThirdPersion").gameObject.SetActive(false);
            }
            else
                instance.transform.Find("FirstPersion/Camera/WeaponCamera/AK47_anim").gameObject.SetActive(false);


            if (Players.ContainsKey(account))
            {
                if (Players[account].Instance != null)
                    Destroy(Players[account].Instance);
                Players[account] = player;
            }
            else
                Players.Add(account, player);

            return player;
        }


        public void Revive(Player player)
        {
            SpawnPlayer(player.RoomUser, SpawnPoints[player.RoomUser.Index].position, SpawnPoints[player.RoomUser.Index].eulerAngles);
        }


        private void OnDestroy()
        {
            UdpRequest.Instance.OnReceiveMessage -= Udp_OnReceiveMessage;
            TcpRequest.Instance.OnRecevieMessage -= Tcp_OnReceiveBroadcast;
        }

    }
}

