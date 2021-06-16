using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Net;
using System;
using System.IO;
using System.Diagnostics;
using ProtobufProto.Model;

namespace Game.UI.Panel
{
    public class UpdatePanel : PanelBase
    {
        private void Start()
        {
            float version = float.Parse(Application.version);
            UnityEngine.Debug.Log("version:" + Application.version);
            TcpRequest.Instance.DoRequest<Respone<VersionInfo>>(version, TcpFlags.TcpCheckVersion, res =>
            {
                if (!res.IsSuccess)
                {
                    VersionInfo info = res.Data;
                    PanelManager.Instance.ShowMessageBox("有新的更新可用,要更新吗?", MessageBoxButtons.YesNo, btn =>
                      {
                          if (btn == MessageBoxResult.Yes)
                          {
                              DoUpdate(version);
                          }
                          else
                          {
                              GameController.QuiteGame();
                          }
                      });
                }
                else
                {
                    Close();
                    PanelManager.Instance.OpenPanel<LoginPanel>();
                }
            });
        }

        private void DoUpdate(float version)
        {
            string path= Application.dataPath;
            DirectoryInfo Root = new DirectoryInfo(path).Parent;
            UnityEngine.Debug.Log("root:" + Root.FullName);
            var updatePath= Root.FullName + "\\Update.exe";
            if(!File.Exists(updatePath))
            {
                PanelManager.Instance.ShowMessageBox("更新失败,文件丢失");
                return;
            }


            Process updateProcess = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = updatePath;
            info.Arguments = version + " " + Root + " " + Process.GetCurrentProcess().MainModule.FileName;
            updateProcess.StartInfo = info;
            updateProcess.Start();
            GameController.QuiteGame();
        }

    }

}
