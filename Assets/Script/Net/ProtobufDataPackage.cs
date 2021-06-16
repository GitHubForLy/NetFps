using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtobufProto.Model;
using System;
using System.Linq;

namespace Game.Net
{
    class ProtobufDataPackage
    {
        public static byte[] PackageData(TcpFlags type, byte[] data)
        {
            var typebits = BitConverter.GetBytes((short)type);
            return typebits.Concat(data).ToArray();
        }

        public static byte[] UnPackageData(byte[] data, out TcpFlags type)
        {
            type = (TcpFlags)BitConverter.ToInt16(data, 0);
            return data.Skip(2).ToArray();
        }

        /// <summary>
        /// 使用指定的UdpFlags打包数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PackageData(UdpFlags type, byte[] data)
        {
            var typebits = BitConverter.GetBytes((short)type);
            return typebits.Concat(data).ToArray();
        }

        public static byte[] UnPackageData(byte[] data, out UdpFlags type)
        {
            type = (UdpFlags)BitConverter.ToInt16(data, 0);
            return data.Skip(2).ToArray();
        }
    }
}