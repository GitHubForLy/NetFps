using ServerCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace Game.Net
{
    [RequireComponent(typeof(NetIdentity))]
    public class NetBehaviour : MonoBehaviour
    {
        public NetIdentity Identity;
        public string Account => Identity.Account;

        //private IEnumerable<MethodInfo> syncMethods;

        public bool IsLocalPlayer 
        { 
            get
            {
                if(BattlegroundManager.Instance==null)
                {
                    Debug.LogError("null instance");
                    return false;
                }
                return BattlegroundManager.Instance.IsLocalPlayer(Identity);
            } 
        }

        //private void Start()
        //{
        //    syncMethods= GetType().GetMethods().Where(m=>m.IsDefined(typeof(SyncMethodAttribute),true)).Select(m=>m.de);
        //}


    }
}
