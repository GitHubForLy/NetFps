using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Net;
using ProtobufProto.Model;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using ServerCommon;

namespace Game.UI.Panel
{
    public class RoomPanel : PanelBase
    {
        public GameObject UserCardPrefab;
        public GameObject MsgCardPrefab;
        public GameObject KeyValPrefab;
        [Space(10)]
        public GameObject ReadyButton;
        public GameObject[] UserCardSockets;
        public GameObject MsgContent;
        public Scrollbar MsgScrollbar;
        public GameObject RoomInfoContent;
        [Space(10)]
        public string UserNameText = "UserName";
        public string UserStateText = "UserState";
        public string UserOwnerTagText = "OwinTag";
        public string UserSelfTagText = "SelfTag";
        public string JoinTipText = "joinTip";



        private RoomUser Self;
        private bool IsOwner;
        private RoomInfo room;
        private Dictionary<RoomUser, GameObject> users = new Dictionary<RoomUser, GameObject>();

        public override void OnInit(params object[] paramaters)
        {
            room = (RoomInfo)paramaters[0];

            initInfo();
            InitUsers();

            TcpRequest.Instance.OnRecevieMessage += Instance_OnReceiveBroadcast;

            for (int id=0;id<UserCardSockets.Length; id++)
            {
                int index = id;
                UserCardSockets[id].GetComponent<Button>().onClick.AddListener(()=>OnUserCardSocketClick(index));
            }
        }

        /// <summary>
        /// 设置房间信息
        /// </summary>
        private void initInfo()
        {
            if(room.Setting.Mode== FightMode.KillCount)
            {
                Inist("房间号", room.RoomId.ToString());
                Inist("房间名", room.Setting.RoomName);
                Inist("胜利目标", "击杀数");
                Inist("击杀数", room.Setting.TargetKillCount.ToString());
                Inist("房间密码", room.Setting.HasPassword ? "是" : "否");
            }

            void Inist(string key,string val)
            {
                GameObject keyval;
                keyval = Instantiate(KeyValPrefab, RoomInfoContent.transform);
                keyval.transform.Find("Title").GetComponent<Text>().text = key;
                keyval.transform.Find("Value").GetComponent<Text>().text = val;
            }
        }

        private void InitUsers()
        {
            TcpRequest.Instance.DoRequest<RoomUsers>(room.RoomId, TcpFlags.TcpGetRoomUsers, res =>
            {
                foreach (var ru in res.Users)
                {
                    if (ru.Account == NetManager.Instance.LoginAccount)
                    {
                        Self = ru;
                        IsOwner = Self.IsRoomOwner;
                    }
                    var obj = InstantiateUserCard(ru);
                    SetCardInfo(obj, ru);
                    users.Add(ru, obj);
                }

                if (IsOwner)
                    ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "开始游戏";
                else
                    ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "准备";
            });
        }

        private void Instance_OnReceiveBroadcast(TcpFlags action,Func<Type,object>formatter)
        {
            if (action == TcpFlags.TcpRoomChange)
            {
                var user = (RoomUser)formatter(typeof(RoomUser));
                switch (user.LastOpeartion)
                {
                    case RoomUser.RoomOpeartion.Leave:
                        UserLeave(user);
                        break;
                    case RoomUser.RoomOpeartion.Join:
                        UserJoin(user);
                        break;
                    case RoomUser.RoomOpeartion.Ready:
                        UserReady(user);
                        break;
                    case RoomUser.RoomOpeartion.CancelReady:
                        UserCancelReady(user);
                        break;
                    case RoomUser.RoomOpeartion.ChangeIndex:
                        UserChangeIndex(user);
                        break;
                }
            }
            else if (action == TcpFlags.TcpRoomMessage)
            {
                var msg = (RoomMessage)formatter(typeof(RoomMessage));

                if (msg.UserAccount != NetManager.Instance.LoginAccount)
                    AppendMessage(msg.UserAccount, msg.Message);
            }
            else if (action == TcpFlags.TcpBdDoStartFight)
                StartFight();
        }

        private void UpdateOwner(RoomUser subdata)
        {
            //用户离开时重新请求一下用户信息 只是为了单独更新一下房主 
            TcpRequest.Instance.DoRequest<RoomUsers>(room.RoomId, TcpFlags.TcpGetRoomUsers, res =>
            {
                foreach (var ru in res.Users)
                {
                    if(ru.IsRoomOwner)
                    {
                        SetCardInfo(users[ru], ru);
                        if(ru==Self)
                        {
                            IsOwner = true;
                            Self.IsRoomOwner = true;
                            Self.State = RoomUserStates.Ready;
                            ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "开始游戏";
                        }
                    }
                }
            });
        }

        private void UserLeave(RoomUser user)
        {
            if (user == Self)
                return;
            UpdateOwner(user);

            if(users.ContainsKey(user))
            {
                Destroy(users[user]);
                users.Remove(user);
                UserCardSockets[user.Index].transform.Find(JoinTipText).gameObject.SetActive(true);
            }
        }

        private void UserJoin(RoomUser user)
        {
            Debug.Log("join:"+user.Account);
            if (user.Account != NetManager.Instance.LoginAccount)
            {
                Debug.Log("join1:" + user.Account);
                var obj = InstantiateUserCard(user);
                SetCardInfo(obj, user);
                users.Add(user, obj);
            }
        }
        private void UserReady(RoomUser user)
        {
            Debug.Log("ready");
            if(users.ContainsKey(user))
            {
                Debug.Log("ready1");
                if (user != Self)
                {
                    Debug.Log("ready2");
                    SetCardInfo(users[user], user);
                    users.Keys.First(m => m == user).State = user.State;
                }
            }
        }

        private void UserCancelReady(RoomUser user)
        {
            if (users.ContainsKey(user))
            {
                if (user != Self)
                {
                    SetCardInfo(users[user], user);
                    users.Keys.First(m => m == user).State = user.State;
                }
            }
        }

        private void UserChangeIndex(RoomUser user)
        {
            if (users.ContainsKey(user))
            {
                //if (user != Self)
                //{
                    int oldindex = users.Keys.First(m => m.Account == user.Account).Index;
                    ChangeIndex(user, oldindex, user.Index);                 
                //}
            }
        }

        private GameObject InstantiateUserCard(RoomUser user)
        {
            if(UserCardSockets.Length<=user.Index)
            {
                Debug.LogError("没有找到对应的用户槽");
                return null;
            }

            var cardp = UserCardSockets[user.Index];
            var tipt = cardp.transform.Find(JoinTipText);
            var tip= tipt.gameObject;
            tip.SetActive(false);
            var card = Instantiate(UserCardPrefab, cardp.transform);

            return card;
        }
       
        private void SetCardInfo(GameObject card,RoomUser user)
        {
            card.transform.Find(UserNameText).GetComponent<Text>().text = user.Info.UserName;
            if (user.IsRoomOwner)
            {
                card.transform.Find(UserStateText).GetComponent<Text>().text = "准备";
                card.transform.Find(UserOwnerTagText).gameObject.SetActive(true);
            }
            else
            {
                card.transform.Find(UserOwnerTagText).gameObject.SetActive(false);
                card.transform.Find(UserStateText).GetComponent<Text>().text = user.State == RoomUserStates.Ready ? "准备" : "";
            }
            if(user==Self)
                card.transform.Find(UserSelfTagText).gameObject.SetActive(true);
            else
                card.transform.Find(UserSelfTagText).gameObject.SetActive(false);
        }


        public void LeaveRoom()
        {
            var lod = PanelManager.Instance.OpenPanel<LoadingPanel>();
            TcpRequest.Instance.DoRequest<Respone>(null,TcpFlags.TcpLeaveRoom,res=> 
            {
                lod.Close();
                if (res.IsSuccess)
                {
                    Close();
                    PanelManager.Instance.OpenPanel<RoomListPanel>();
                }
                else
                    PanelManager.Instance.ShowMessageBox("退出失败:" + res.Message);
            });
        }


        public void OnReadyButtonClick()
        {
            if(IsOwner)
            {
                //开始游戏
                DoStartFight();
            }
            else
            {
                if (Self.State == RoomUserStates.Waiting)
                    DoReady();
                else if (Self.State == RoomUserStates.Ready)
                    DoCancelReady();
            }
        }

        private void DoReady()
        {
            var lod = PanelManager.Instance.OpenPanel<MaskPanel>();
            TcpRequest.Instance.DoRequest<Respone>(null, TcpFlags.TcpRoomReady, res =>
            {
                lod.Close();
                if (res.IsSuccess)
                {
                    ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "取消准备";
                    Self.State = RoomUserStates.Ready;
                    SetCardInfo(users[Self], Self);
                }
                else
                {
                    PanelManager.Instance.ShowMessageBox("准备失败:" + res.Message);
                }
            });
        }
        private void DoCancelReady()
        {
            var lod = PanelManager.Instance.OpenPanel<MaskPanel>();
            TcpRequest.Instance.DoRequest<Respone>(null, TcpFlags.TcpRoomCancelReady, res =>
            {
                lod.Close();
                if (res.IsSuccess)
                {
                    ReadyButton.transform.GetChild(0).GetComponent<Text>().text = "准备";
                    Self.State = RoomUserStates.Waiting;
                    SetCardInfo(users[Self], Self);
                }
                else
                {
                    PanelManager.Instance.ShowMessageBox("取消准备失败:" + res.Message);
                }
            });
        }

        /// <summary>
        /// 用户槽点击事件
        /// </summary>
        /// <param name="Index">目标索引</param>
        public void OnUserCardSocketClick(int Index)
        {
            if (users.Any(m => m.Key.Index == Index))
                return;
            var lod = PanelManager.Instance.OpenPanel<MaskPanel>();
            TcpRequest.Instance.DoRequest<Respone>(Index, TcpFlags.TcpRoomChangeIndex, res =>
            {
                lod.Close();
                //if (res.IsSuccess)
                //{
                //    ChangeIndex(Self,Self.Index,Index);
                //}
            });
        }

        /// <summary>
        /// 更改用户的位置
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="index">新位置</param>
        private void ChangeIndex(RoomUser user,int oldIndex, int index)
        {
            var thisuser = users.Keys.First(m => m.Index == oldIndex);
            thisuser.Index = index;
            thisuser.Team = user.Team;    //队伍也可能变更

            UserCardSockets[oldIndex].transform.Find(JoinTipText).gameObject.SetActive(true);
            var cardp = UserCardSockets[index];
            cardp.transform.Find(JoinTipText).gameObject.SetActive(false);
            users[user].transform.SetParent(cardp.transform);
        }

        public override void OnEscape()
        {
            if(Self.State== RoomUserStates.Ready && !IsOwner)
            {
                OnReadyButtonClick();
                return;
            }

            PanelManager.Instance.ShowMessageBox("确定要退出房间吗？", MessageBoxButtons.OkCancel, res =>
            {
                if(res==MessageBoxResult.Ok)
                {
                    LeaveRoom();
                }
            });
        }

        /// <summary>
        /// 消息框输入消息
        /// </summary>
        public override void OnEnter()
        {
            InputPanel.ShowInput(message =>
            {
                //AppendMessage(NetManager.Instance.LoginAccount,message);
                TcpRequest.Instance.DoRequest(new RoomMessage() {  Message=message, UserAccount=NetManager.Instance.UserInfo.UserName}, 
                    TcpFlags.TcpRoomMessage);
            });
        }

        public void AppendMessage(string account,string message)
        {
            var card = Instantiate(MsgCardPrefab, MsgContent.transform);
            card.transform.GetChild(0).GetComponent<Text>().text = account + ":"+message;
            StartCoroutine(InsSrollBar());
        }

        /// <summary>
        /// 滚动滑动条到底部
        /// </summary>
        IEnumerator InsSrollBar()
        {
            yield return new WaitForEndOfFrame();
            MsgScrollbar.value = 0;
        }

        private void DoStartFight()
        {
            if(users.Any(m=>m.Key.State!= RoomUserStates.Ready))
            {
                PanelManager.Instance.ShowMessageBox("尚有人未准备 无法开始");
                return;
            }
            var lod = PanelManager.Instance.OpenPanel<LoadingPanel>();
            TcpRequest.Instance.DoRequest<Respone>(Time.time, TcpFlags.TcpDoStartFight, res =>
            {
                lod.Close();
                if (!res.IsSuccess)
                    PanelManager.Instance.ShowMessageBox("无法开始:" + res.Message);
            });
        }

        private void StartFight()
        {
            //这里直接采用房间的用户列表信息 也可以由StartFight返回用户列表
            BattlegroundManager.Users = users.Keys.ToArray();
            BattlegroundManager.Room = room;
            GameController.LoadScene(Scenes.Battle);
        }

        private void OnDestroy()
        {
            TcpRequest.Instance.OnRecevieMessage -= Instance_OnReceiveBroadcast;
        }
    }
}
