using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AopCore;

namespace Game.Net
{

    ///// <summary>
    ///// 广播目标方法的调用
    ///// </summary>
    //public class SyncMethodAttribute : MethodHookAttribute
    //{
    //    private BroadcastFlag  _flag;
    //    public SyncMethodAttribute(BroadcastFlag flag = BroadcastFlag.Global)
    //    {
    //        _flag = flag;
    //    }
    //    public override void OnMethodEnter(MethodExecuteArgs args)
    //    {
    //        var behaviour = args.Instance as NetBehaviour;
    //        if(behaviour==null)
    //        {
    //            Debug.LogWarning("必须继承自Netbehaviour才能使该特性生效,目标类:"+args.Instance.ToString());
    //            return;
    //        }

    //        if(behaviour.IsLocalPlayer)
    //        {
    //            var data = new BroadcastMethod
    //            {
    //                ClassFullName = args.Method.DeclaringType.FullName,
    //                MethodName = args.Method.Name,
    //                Parameters = args.ParameterValues,
    //                Flag = _flag
    //            };
    //            TcpRequest.Instance.BroadcastMethod(data);
    //        }
    //    }
    //}

}

