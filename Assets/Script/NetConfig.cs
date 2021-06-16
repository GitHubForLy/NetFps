using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GlobalConfig
    {
        //public const string ServerIp = "127.0.0.1";
        public const string ServerIp = "47.117.130.65";
        public const short ServerTcpPort = 4789;
        public const short ServerUdpPort = 4790;
        /// <summary>
        /// 发送操作的频率 毫秒
        /// </summary>
        public const int UpOperDuration = 100;
        /// <summary>
        /// 逻辑更新 毫秒
        /// </summary>
        public const int LogicUpdateDuration = 30;
        /// <summary>
        /// 单次最多请求的丢失帧数
        /// </summary>
        public const int ReqLackFrameCount = 5;
        /// <summary>
        /// 缩放因子
        /// </summary>
        public const int ScaleRate = 1000;
    }

}
