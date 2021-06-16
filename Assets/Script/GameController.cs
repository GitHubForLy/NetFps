using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Game.UI;
using Game.UI.Panel;
using System;

namespace Game
{
    public enum Scenes
    {
        Start,
        Battle
    }

    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        public static string CurrentSceneName => SceneManager.GetActiveScene().name;

        private static object[] panelParameters=new object[0];

#if UNITY_EDITOR
        private static System.Type StartOpenPanel = typeof(LoginPanel);
#else
        private static System.Type StartOpenPanel=typeof(UpdatePanel);
#endif

        /// <summary>
        /// 全局的待执行的委托列表
        /// </summary>
        private static List<Action> Delegates { get; } = new List<Action>();


        // Start is called before the first frame update
        void Awake() 
        {
            if (Instance != null && Instance == this)
                return;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (StartOpenPanel != null)
                PanelManager.Instance.OpenPanel(StartOpenPanel, panelParameters);
        }

        private void Init()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PanelManager.Instance.DefaultCursroMode = CursorLockMode.None;
            PanelManager.Instance.DefaultCursorVisble = true;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        void Start()
        {
            Debug.Log("game start");
        }

        /// <summary>
        /// 加载场景并打开指定的 panel
        /// </summary>
        public static void LoadScene(Scenes SceneName,System.Type PanelType,params object[] parameters)
        {
            panelParameters = parameters;
            StartOpenPanel = PanelType;
            switch(SceneName)
            {
                case Scenes.Start:
                    SceneManager.LoadScene("StartScene");
                    return;
                case Scenes.Battle:
                    SceneManager.LoadScene("Alone");
                    return;
            }
        }
        /// <summary>
        /// 加载场景并打开指定的 panel
        /// </summary>
        public static void LoadScene<T>(Scenes SceneName, params object[] parameters)
        {
            LoadScene(SceneName, typeof(T), parameters);
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        public static void LoadScene(Scenes SceneName)
        {
            LoadScene(SceneName, null);
        }

        public static void QuiteGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void Update()
        {
            InvokeEventDelegate();
        }

        public static void AddDelegate(Action action)
        {
            lock(Delegates)
            {
                Delegates.Add(action);
            }
        }

        //调用网络请求回调方法
        private void InvokeEventDelegate()
        {
            lock (Delegates)
            {
                for (int i = Delegates.Count - 1; i >= 0; i--)
                {
                    var dega = Delegates[i];
                    Delegates.RemoveAt(i);
                    dega.Invoke();
                }
            }
        }
    }

}
