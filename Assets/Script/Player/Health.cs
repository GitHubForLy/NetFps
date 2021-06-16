using ServerCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Net;
using UnityEngine;


namespace Game.Player
{

    /// <summary>
    /// 伤害信息
    /// </summary>
    public struct DamageHit
    {
        public float RealDamage { get; set; }

        public bool IsMakeDeath { get; set; }
    }

    public delegate void OnDieEventHandle(GameObject deadTank,Behaviour killer);

    public class Health : NetBehaviour
    {
        public float MaxHealth = 100;
        public float CurrentHealth { get; private set; }
        public bool IsDie { get; private set; }

        public event OnDieEventHandle OnDie;

        private NetIdentity identity;
        private PlayerAnimation playerAnimation;

        // Start is called before the first frame update
        void Awake()
        {
            playerAnimation = GetComponent<PlayerAnimation>();
            identity = GetComponent<NetIdentity>();
            IsDie = false;
            CurrentHealth = MaxHealth;
        }


        //public override void OnBroadcast((string Action, IDynamicType data) dt)
        //{
        //    if(dt.Action==DataModel.BroadcastActions.TakeDamage)
        //    {
        //        (string account,float damage) data = dt.data.GetValue<(string, float)>();
        //        if(data.account== identity.Account && !IsLocalPlayer)
        //        {
        //            TakeDamage(data.damage,BattlegroundManager.Instance.Tanks[data.account].Instance.GetComponent<TankFire>());
        //        }
        //    }
        //}

        public DamageHit TakeDamage(float Damage, Behaviour sender)
        {
            DamageHit hit = new DamageHit() { IsMakeDeath = false, RealDamage = 0 };
            if (IsDie)
                return hit;

            if(IsLocalPlayer)
                BleedBehavior.BloodAmount = CurrentHealth / MaxHealth;

            hit.RealDamage = Damage;
            CurrentHealth -= Damage;
            if (CurrentHealth <= 0)
            {
                hit.IsMakeDeath = true;
                CurrentHealth = 0;
                IsDie = true;
                DoDie(sender);
            }
            return hit;
        }


        public void Revive()
        {
            IsDie = false;
            CurrentHealth = MaxHealth;
        }

        private void DoDie(Behaviour killer)
        {
            OnDie?.Invoke(gameObject,killer);
        }

    }

}
