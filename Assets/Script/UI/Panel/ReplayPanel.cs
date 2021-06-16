using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Panel
{
    public class ReplayPanel : PanelBase
    {
        [SerializeField]
        private Dropdown m_SpeedDropDown;
        [SerializeField]
        private Slider m_ProgressSlider;

        private float speed;
        private float timescale;
        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //m_ProgressSlider.minValue = 1;
            //m_ProgressSlider.maxValue = BattlegroundManager.ReplayOpears.Opears.Count;

            StartCoroutine(nameof(SendhisOperation));
            speed = GlobalConfig.UpOperDuration / 1000f;
            timescale = Time.timeScale;
        }

       
        private IEnumerator SendhisOperation()
        {
            Debug.Log(BattlegroundManager.ReplayOpears.Opears.Count);
            for (int i = 1; i < BattlegroundManager.ReplayOpears.Opears.Count + 1; i++)
            {
                BattlegroundManager.Instance.AddFrame(i, BattlegroundManager.ReplayOpears.Opears[i]);
                BattlegroundManager.Instance.LogicUpdate();
                yield return null;
                yield return new WaitForSeconds(speed);
            }
            PanelManager.Instance.ShowMessageBox("播放结束", MessageBoxButtons.Ok, res =>
            {
                GameController.LoadScene<RoomListPanel>(Scenes.Start);
            });
            Time.timeScale = timescale;
        }

        public void OnSpeedChange()
        {
            float rate=float.Parse( m_SpeedDropDown.options[m_SpeedDropDown.value].text);
            speed = (GlobalConfig.UpOperDuration / rate) / 1000f;
            Time.timeScale = timescale * rate;
        }

        public void OnProgressChange()
        {
            
        }

        public void Quit()
        {
            GameController.LoadScene<RoomListPanel>(Scenes.Start);
        }
    }

}
