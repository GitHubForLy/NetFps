using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.UI.Panel;

namespace Game
{
    public class BattleUI : MonoBehaviour
    {

        private void Start()
        {
#if UNITY_EDITOR
#else
            InputController.Instance.AddButton("esc", KeyCode.Escape);
            PanelManager.Instance.OnEscape += Instance_OnEscape;
#endif
        }

        private void Instance_OnEscape()
        {
            if(!BattlegroundManager.IsReplay)
                PanelManager.Instance.OpenPanel<Game.UI.Panel.BattleMenuPanel>();
            else
            {
                PanelManager.Instance.ShowMessageBox("确定要退出回放吗?", MessageBoxButtons.OkCancel, res =>
                {
                    if(res== MessageBoxResult.Ok)
                    {
                        GameController.LoadScene<RoomListPanel>(Scenes.Start);
                    }
                });
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
#else
            //if (InputController.Instance.GetButtonDown("esc"))
            //{
            //    Debug.Log(PanelManager.Instance.IsOpen<BattleMenuPanel>());
            //    if (!PanelManager.Instance.IsOpen<BattleMenuPanel>())
            //    {
            //        PanelManager.Instance.OpenPanel<Game.UI.Panel.BattleMenuPanel>();
            //    }
            //}
#endif
        }


        private void OnDestroy()
        {
#if UNITY_EDITOR
#else
            PanelManager.Instance.OnEscape -= Instance_OnEscape;
#endif
        }
    }

}

