using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InputController : MonoBehaviour {


    public static InputController Instance { get; private set; }

    public class fps_InputAxis
    {
        public KeyCode positive;
        public KeyCode negative;
    }

    private Dictionary<string, KeyCode> buttons = new Dictionary<string, KeyCode>();


    private Dictionary<string, fps_InputAxis> axis = new Dictionary<string, fps_InputAxis>();


    private List<string> unityAxis = new List<string>();


    private void Awake()
    {
        if (Instance != null && Instance == this)
            return;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupDefaults();
    }

    private void SetupDefaults(string type = "")
    {
        if (type == "" || type == "buttons")
        {
            if (buttons.Count == 0)
            {
                AddButton("Fire", KeyCode.Mouse0);
                AddButton("Reload", KeyCode.R);
                AddButton("Jump", KeyCode.Space);
                AddButton("Crouch", KeyCode.LeftControl);
                AddButton("Sprint", KeyCode.LeftShift);
                AddButton("Cancel", KeyCode.Escape);
            }
        }

        if (type == "" || type == "Axis")
        {
            if (axis.Count == 0)
            {
                AddAxis("Horizontal", KeyCode.W, KeyCode.S);
                AddAxis("Vertical", KeyCode.A, KeyCode.D);
            }
        }

        if (type == "" || type == "UnityAxis")
        {
            if (unityAxis.Count == 0)
            {
                AddUnityAxis("Mouse X");
                AddUnityAxis("Mouse Y");
                AddUnityAxis("Horizontal");
                AddUnityAxis("Vertical");
            }
        }
    }

    public void AddButton(string n, KeyCode k)
    {
        if (buttons.ContainsKey(n))
            buttons[n] = k;
        else
            buttons.Add(n, k);

    }

    public void AddAxis(string n, KeyCode pk, KeyCode nk)
    {
        if (axis.ContainsKey(n))
            axis[n] = new fps_InputAxis() { positive = pk, negative = nk };
        else
            axis.Add(n, new fps_InputAxis() { positive = pk, negative = nk });
    }

    public void AddUnityAxis(string n)
    {
        if (!unityAxis.Contains(n))
            unityAxis.Add(n);
    }

    public bool GetButton(string button)
    {
        if (buttons.ContainsKey(button))
        {
#if MOBILE_INPUT    
            return CrossPlatformInputManager.GetButton(button);
#else
            return Input.GetKey(buttons[button]);
#endif
        }
        return false;
    }

    public bool GetButtonDown(string button)
    {
        if (buttons.ContainsKey(button))
            return Input.GetKeyDown(buttons[button]);
            //return CrossPlatformInputManager.GetButtonDown(button);
        return false;
    }

    public float GetAxis(string axis)
    {
        if (this.unityAxis.Contains(axis))
            return Input.GetAxis(axis);
            //return CrossPlatformInputManager.GetAxis(axis);
        else
            return 0;
    }

    public float GetAxisRaw(string axis)
    {
        if (this.axis.ContainsKey(axis))
        {
            float x = CrossPlatformInputManager.GetAxis(axis);
            float val = 0;
            //if (Input.GetKey(this.axis[axis].positive))
            if(x > 0)
                return 1;
            //if (Input.GetKey(this.axis[axis].negative))
            if (x < 0)
                return -1;
            return val;
        }

        else if (unityAxis.Contains(axis))
        {
            return Input.GetAxisRaw(axis);
            //return CrossPlatformInputManager.GetAxisRaw(axis);
        }
        else
        {
            return 0;
        }
    }
}
