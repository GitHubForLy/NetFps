using ServerCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pt = ProtobufProto.Model;

namespace Game.Net
{
    public sealed class SyncTransfrom : NetBehaviour
    {
        /// <summary>
        /// 是否需要同步服务器上的位置
        /// </summary>
        public bool IsNeedSyncTransform=true;
        private NetIdentity netIdentity;
        private new  Rigidbody rigidbody;
        private Vector3 oldPos, oldEur;
        private Vector3 oldRcvPos=Vector3.zero, oldRcvEur=Vector3.zero,oldfpos;
        private Vector3 fPos, fEur;
        private float lastRcvTime;
        private float deltaTime;
        private float lastSendtime;

        public void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            netIdentity = GetComponent<NetIdentity>();
        }

        private void Start()
        {
            if (!IsLocalPlayer)
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            oldPos = transform.position;
            oldEur = transform.eulerAngles;

            UdpRequest.Instance.OnReceiveMessage += OnReceive;
        }


        public void OnReceive(Pt.UdpFlags action,UdpRequest.Formatter formatter)
        {

            if(action == Pt.UdpFlags.UdpDownTransform && IsNeedSyncTransform /*&& !IsLocalPlayer*/)
            {
                Pt.PlayerTransformMap data = formatter.Deserailize<Pt.PlayerTransformMap>();
                foreach (var item in data.Transforms)
                {
                    if (item.Key== netIdentity.Account)
                    {
                        var cuPos = item.Value.Position.ToUnityVector();
                        var cuEur = item.Value.Rotation.ToUnityVector();

                        transform.position = cuPos;
                        transform.eulerAngles = cuEur;
                        break;
                    }
                }                         
            }
        }

        
        void Update()
        {
            if (!IsLocalPlayer)
                return;

            if(Time.time-lastSendtime>1.0f)
            {
                SyncTransform();
            }
        }

        /// <summary>
        /// 向服务器同步自己的Transfrom
        /// </summary>
        public void SyncTransform()
        {
            var pos = transform.position;
            var eur = transform.eulerAngles;
            //if (pos.x == oldPos.x && pos.y == oldPos.y && pos.z == oldPos.z &&
            //    eur.x == oldEur.x && eur.y == oldEur.y && eur.z == oldEur.z)
            //    return;


            var ts = new Pt.PlayerTransform
            {
                Account = NetManager.Instance.LoginAccount,
                Trans = new Pt.Transform
                 {
                     Position = new Pt.Vector3
                     {
                         X = pos.x,
                         Y = pos.y,
                         Z = pos.z
                     },
                     Rotation = new Pt.Vector3
                     {
                         X = eur.z,
                         Y = eur.y,
                         Z = eur.z
                     }
                 }
            };


            UdpRequest.Instance.Request(Pt.UdpFlags.UdpUpTransform, ts);
        }


        private void OnDestroy()
        {
            UdpRequest.Instance.OnReceiveMessage -= OnReceive;
        }


    }

}
