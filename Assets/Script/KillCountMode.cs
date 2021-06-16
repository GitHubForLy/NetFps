using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.UI.Panel;
using Game.Player;
using Game.Net;
using ProtobufProto.Model;
using ServerCommon;
using System;
using UnityEngine.UI;
using Game.Player;

namespace Game
{
    public class KillCountMode : MonoBehaviour
    {
        [Tooltip("复活时间")]
        public float ReviveTime = 3.0f;

        public GameObject ResultTip;


        private Dictionary<BattlegroundManager.Player,float> playerPendingRevive=new Dictionary<BattlegroundManager.Player, float>();
        private GameObject canvas;
        private KillCountPanel countPanel;
        private int redKillCount=0;
        private int blueKillCount = 0;
        private float logicTotailTime;
        private bool isEnd = false;
        private int winTeam;


        void Start()
        {
            canvas = GameObject.FindGameObjectWithTag(Tags.Canvas);
            countPanel= PanelManager.Instance.SetupPanel<KillCountPanel>(canvas.transform);
            //countPanel.StartTime(MaxSeconds);
            //countPanel.OnTimeFinished += CountPanel_OnTimeFinished;
            countPanel.SetTargetKillCount(BattlegroundManager.Room.Setting.TargetKillCount); 

            BattlegroundManager.Instance.OnPlayerDied += OnPlayerDied;
            BattlegroundManager.Instance.OnLogicDown += OnLogicDown;


            ResultTip.SetActive(false);
            UdpRequest.Instance.OnReceiveMessage += Udp_OnReceiveMessage;
        }

        private void Udp_OnReceiveMessage(UdpFlags action, UdpRequest.Formatter formatter)
        {
            switch(action)
            {
                case UdpFlags.UdpDownGameEnd:
                    OnDownGameEnd();
                    break;
            }
        }


        private void OnLogicDown(PlayerOperations ops)
        {
            //根据逻辑帧判断累计时间复活
            BattlegroundManager.Player[] keys=new BattlegroundManager.Player[playerPendingRevive.Keys.Count];
            playerPendingRevive.Keys.CopyTo(keys,0);
            foreach(var pk in keys)
            {
                if( playerPendingRevive[pk]>=ReviveTime)
                {
                    BattlegroundManager.Instance.Revive(pk);
                    playerPendingRevive.Remove(pk);
                }
                else
                {
                    playerPendingRevive[pk] = playerPendingRevive[pk] + GlobalConfig.UpOperDuration/1000f;
                }
            }

            UpdateRemainingTime();
        }

        private void OnPlayerDied(BattlegroundManager.Player player, BattlegroundManager.Player killer)
        {
            if (killer.TeamNumber == 0)
            {
                redKillCount++;
                countPanel.RedCount = redKillCount;
            }
            else
            {
                blueKillCount++;
                countPanel.BlueCount = blueKillCount;
            }

            if (redKillCount >= BattlegroundManager.Room.Setting.TargetKillCount)
                StartCoroutine(GameEnd(0));
            else if (blueKillCount >= BattlegroundManager.Room.Setting.TargetKillCount)
                StartCoroutine(GameEnd(1));
            else
                playerPendingRevive.Add(player, 0);
        }


        private IEnumerator GameEnd(int WinTeam)
        {
            int win = WinTeam == -1 ? -1 : (WinTeam == BattlegroundManager.Instance.LocalPlayer.TeamNumber ? 1 : 0);

            if (win == -1)
                ResultTip.transform.Find("Text").GetComponent<Text>().text = "平局";
            else if(win == 1)
                ResultTip.transform.Find("Text").GetComponent<Text>().text = "胜利";
            else
                ResultTip.transform.Find("Text").GetComponent<Text>().text = "失败";
            ResultTip.SetActive(true);

            winTeam = WinTeam;
            InvokeRepeating(nameof(SendGameEnd), 0, 0.2f);

            //至少等待3秒种
            yield return new WaitForSecondsRealtime(3f);

            if(!BattlegroundManager.IsReplay)
            {
                if (win == 1)
                    DoWin();
                else if (WinTeam == -1)  //平局
                    DogFall();
                else
                    DoFail();
            }
        }

        private void SendGameEnd()
        {
            UdpRequest.Instance.Request(UdpFlags.UdpUpGameEnd, new GameEndResult {  WinTeam=winTeam});
        }

        private void OnDownGameEnd()
        {
            CancelInvoke(nameof(SendGameEnd));
            isEnd = true;
        }

        /// <summary>
        /// 平局
        /// </summary>
        private void DogFall()
        {
            GameController.LoadScene<ResultPanel>(Scenes.Start, -1,BattlegroundManager.Room);
        }

        public void DoWin()
        {
            GameController.LoadScene<ResultPanel>(Scenes.Start, 1, BattlegroundManager.Room);
        }
        public void DoFail()
        {
            GameController.LoadScene<ResultPanel>(Scenes.Start, 0, BattlegroundManager.Room);
        }

        private void UpdateRemainingTime()
        {
            logicTotailTime += GlobalConfig.UpOperDuration / 1000f;
            var remain = (int)(BattlegroundManager.Room.Setting.MaxTime - logicTotailTime);
            countPanel.SetRemainTime(remain);
            if (remain <= 0)
                CountPanel_OnTimeFinished();
        }

        private void CountPanel_OnTimeFinished()
        {
            if (redKillCount > blueKillCount)
                StartCoroutine(GameEnd(0));
            else if(blueKillCount > redKillCount)
                StartCoroutine(GameEnd(1));
            else
                StartCoroutine(GameEnd(-1));
        }

        void OnDestroy()
        {
            UdpRequest.Instance.OnReceiveMessage -= Udp_OnReceiveMessage;
        }
    }
}

