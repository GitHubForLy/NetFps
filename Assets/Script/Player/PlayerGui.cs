using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Player;
using Game.Net;
using Pt = ProtobufProto.Model;

namespace Game.Player
{
    public class PlayerGui : NetBehaviour
    {
        public Texture AimTarget;
        public Texture FireTarget;
        public Texture KillTexture;
        public Image HealthObj;
        public Text HealthText;

        public float KillIconDelayDuration = 1.5f;

        [HideInInspector]
        public Vector3 TargetPos;

        private Health health;
        private float lastKillEnemyTime = -10;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();

            if (!HealthObj)
                HealthObj = GameObject.FindGameObjectWithTag(Tags.Hp).GetComponent<Image>();
            if (!HealthText)
                HealthText = GameObject.FindGameObjectWithTag(Tags.HpText)?.GetComponent<Text>();

            TcpRequest.Instance.OnRecevieMessage += Instance_OnReceiveMessage;
        }

        private void Instance_OnReceiveMessage(Pt.TcpFlags action,System.Func<System.Type,object> formatter)
        {
            if (!IsLocalPlayer)
                return;
           //if(action == DataModel.BroadcastActions.Die)
           // {
           //     var killer = msg.subdata.GetValue<string>();
           //     if (killer == NetManager.Instance.LoginAccount) //是自己杀了对方
           //         lastKillEnemyTime = Time.time;
           //}
        }

        private void TankFire_OnKillEnemy(DamageHit damageHit)
        {
            lastKillEnemyTime = Time.time;
        }




        private void OnGUI()
        {
            if(!IsLocalPlayer)
                return;
            if (Time.timeScale == 0)//暂停游戏
                return;
            
            DrawAimIco();

            DrawHealth();

            DrawKillIcon();

        }

        private void DrawAimIco()
        {
            if (health.IsDie)
                return;

            //var tar = Camera.main.WorldToScreenPoint(TargetPos);
            //Rect rt = new Rect(tar.x - AimTarget.width / 2, tar.y - AimTarget.height / 2, AimTarget.width, AimTarget.height);
            //GUI.DrawTexture(rt, AimTarget);


            //var pos = Camera.main.WorldToScreenPoint(firePos);
            //Rect frt = new Rect(pos.x - FireTarget.width / 2, Screen.height - pos.y - FireTarget.height / 2, FireTarget.width, FireTarget.height);
            //GUI.DrawTexture(frt, FireTarget);


            //if(ishit && hit.collider.gameObject.CompareTag(Tags.Tank))
            //{
            //    var account= hit.collider.gameObject.GetComponent<NetIdentity>().Account;
            //    var name= BattlegroundManager.Instance.Tanks[account].UserName;
            //    var cont = new GUIContent(name);
            //    var style = new GUIStyle();
            //    style.normal.textColor = Color.white;
            //    style.alignment = TextAnchor.MiddleCenter;
            //    var size= style.CalcSize(cont);
            //    Rect trt = new Rect(new Vector2(pos.x-size.x/2, pos.y-size.y/2), size);

            //    GUI.Label(trt,name,style);
            //}
        }

        /// <summary>
        /// 绘制生命条
        /// </summary>
        private void DrawHealth()
        {
            HealthObj.fillAmount = health.CurrentHealth / health.MaxHealth;
            if(health!=null)
                HealthText.text = $"{Mathf.Ceil(health.CurrentHealth)}/{health.MaxHealth}";
        }

        private void DrawKillIcon()
        {

            if (Time.time - lastKillEnemyTime < KillIconDelayDuration)
            {
                Rect rt = new Rect(Screen.width / 2 - KillTexture.width / 2, Screen.height / 2 - KillTexture.height / 2,
                    KillTexture.width, KillTexture.height);
                GUI.DrawTexture(rt, KillTexture);
            }
        }

        private void OnDestroy()
        {
            TcpRequest.Instance.OnRecevieMessage -= Instance_OnReceiveMessage;
        }
    }
}