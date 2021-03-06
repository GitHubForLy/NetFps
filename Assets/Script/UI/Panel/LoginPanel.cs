using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Net;
using DataModel;
using System;

namespace Game.UI.Panel
{
    public class LoginPanel : PanelBase
    {
        public InputField AccountInput;
        public InputField PasswordInput;

        private void Start()
        {
            AccountInput.text = PlayerPrefs.GetString("user_account");
            PasswordInput.text = PlayerPrefs.GetString("passowrd");
        }
        public void Login()
        {
            if (string.IsNullOrEmpty(AccountInput.text))
            {
                PanelManager.Instance.ShowMessageBox("必须输入账号");
                return;
            }
            if (string.IsNullOrEmpty(PasswordInput.text))
            {
                PanelManager.Instance.ShowMessageBox("必须输入密码");
                return;
            }

            var lod= PanelManager.Instance.OpenPanel<LoadingPanel>();
            NetManager.Instance.Login(AccountInput.text, PasswordInput.text, res =>
            {
                lod.Close();
                if(res.IsSuccess)
                {
                    PlayerPrefs.SetString("user_account", AccountInput.text);
                    PlayerPrefs.SetString("passowrd", PasswordInput.text);
                    //SceneManager.LoadSceneAsync("TankScene");
                    Close();
                    PanelManager.Instance.OpenPanel<RoomListPanel>();
                }
                else
                {
                    PanelManager.Instance.ShowMessageBox("登录失败:" + res.Message);
                }
            });         
        }

        public void Register()
        {
            PanelManager.Instance.OpenPanel<RegisterPanel>();
        }

    }
}