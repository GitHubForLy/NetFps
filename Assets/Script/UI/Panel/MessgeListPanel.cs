using System.Collections;
using System.Collections.Generic;
using Game.Net;
using UnityEngine;
using UnityEngine.UI;
using ProtobufProto.Model;

namespace Game.UI.Panel
{
    public class MessgeListPanel : PanelBase
    {
        [SerializeField]
        private GameObject ScrollView;
        [SerializeField]
        private GameObject ScrollbarVertical;
        [SerializeField]
        private GameObject InputObj;
        [SerializeField]
        private GameObject MsgsScrollView;
        [SerializeField]
        private GameObject MsgContent;
        [SerializeField]
        private Scrollbar MsgScrollbar;

        public GameObject MsgCardPrefab;

        public Color ActiveViewColor;
        public Color UnActiveViewColor;
        public Color ActiveFontColor;
        public Color UnActiveFontColor;

        public bool isActive=false;
        private void Start()
        {
            StartCoroutine(Unactinvestart());
            TcpRequest.Instance.OnRecevieMessage += Instance_OnReceiveBroadcast;
        }

        IEnumerator Unactinvestart()
        {
            yield return new WaitForEndOfFrame();
            DoUnActive();
        }

        private void Instance_OnReceiveBroadcast(TcpFlags action,System.Func<System.Type,object>formatter)
        {
            if (action == TcpFlags.TcpRoomMessage)
                OnRoomMsg((RoomMessage)formatter(typeof(RoomMessage)));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OnEnter();
        }

        public override void OnEnter()
        {
            if (!isActive)
                DoActive();
            else
                DoUnActive();
        }

        public void DoActive()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            BattlegroundManager.Instance.PauseGame();

            ScrollView.GetComponent<Image>().color = ActiveViewColor;
            ScrollbarVertical.GetComponent<Image>().color = ActiveViewColor;
            var old = MsgScrollbar.colors;
            old.normalColor = ActiveViewColor;
            MsgScrollbar.colors = old;

            InputObj.SetActive(true);
            InputObj.GetComponent<InputField>().ActivateInputField();
            isActive = true;
        }

        public void DoUnActive()
        {
            if(!BattlegroundManager.IsReplay)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            BattlegroundManager.Instance.ResumeGame();

            var str = InputObj.GetComponent<InputField>().text;
            if (!string.IsNullOrEmpty(str))
            {
                //AppendMessage(NetManager.Instance.LoginAccount, str);
                TcpRequest.Instance.DoRequest(new RoomMessage {  UserAccount=NetManager.Instance.UserInfo.UserName, Message=str}, 
                    TcpFlags.TcpRoomMessage);
            }
            InputObj.GetComponent<InputField>().text="";

            var old = MsgScrollbar.colors;
            old.normalColor = UnActiveViewColor;
            MsgScrollbar.colors = old;

            ScrollView.GetComponent<Image>().color = UnActiveViewColor;
            ScrollbarVertical.GetComponent<Image>().color = UnActiveViewColor;
            InputObj.SetActive(false);
            isActive = false;
        }

        private void OnRoomMsg(RoomMessage message)
        {
            //if (message.UserAccount != NetManager.Instance.LoginAccount)
                AppendMessage(message.UserAccount, message.Message);
        }

        /// <summary>
        /// 向消息栏中增加消息
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <param name="message"></param>
        private void AppendMessage(string loginAccount, string message)
        {
            var card = Instantiate(MsgCardPrefab, MsgContent.transform);
            card.transform.GetChild(0).GetComponent<Text>().text = "[" + loginAccount + "]" + " :" + message;
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

        private void OnDestroy()
        {
            TcpRequest.Instance.OnRecevieMessage -= Instance_OnReceiveBroadcast;
        }
    }
}
