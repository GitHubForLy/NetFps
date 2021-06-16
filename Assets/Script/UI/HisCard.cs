using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtobufProto.Model;
using Game.Net;

namespace Game.UI
{
    public class HisCard : MonoBehaviour
    {
        [SerializeField]
        private Text m_States;
        [SerializeField]
        private Text m_Time;

        private CombatInfo info;
        private int id;

        public void SetInfo(CombatInfo info)
        {
            m_States.text = info.isWin ? "胜利" : "失败";
            m_States.color = info.isWin ? Color.green : Color.red;
            m_Time.text = info.StartTime;
            this.info = info;
            id = info.BattleId;
        }

        public void LookReplay()
        {
            TcpRequest.Instance.DoRequest<TotalOpears>(id, TcpFlags.TcpGetCombatData, res =>
            {
                BattlegroundManager.IsReplay = true;
                BattlegroundManager.Room = new RoomInfo()
                {
                    Setting = info.Setting
                };
                BattlegroundManager.Users = info.Users.Users.ToArray();
                BattlegroundManager.ReplayOpears = res;
                GameController.LoadScene(Scenes.Battle);
            });   
        }
    }

}