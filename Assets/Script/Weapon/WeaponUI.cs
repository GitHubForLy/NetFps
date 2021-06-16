using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{

    [Tooltip("准心中间距离"),Header("CrossHair")]
    public int CrossDistance=35;

    [Tooltip("准心矩形的宽")]
    public int CrossWidth =25;

    [Tooltip("准心矩形的高")]
    public int CrossHeight =3;

    [Tooltip("准心矩形背景")]
    public Texture2D CrossTexture;

    private GUIStyle CrossStyle;

    // Start is called before the first frame update
    void Start()
    {
        CrossStyle = new GUIStyle();
        CrossStyle.normal.background = CrossTexture;
    }

    private void OnGUI()
    {
        DrawCrosshair();
    }

    private void DrawCrosshair()
    {
        int screenWidthHalf = Screen.width/2;
        int screenHeightHalf = Screen.height/2;

        GUI.Box(new Rect(screenWidthHalf - CrossDistance / 2 - CrossWidth,
            screenHeightHalf - CrossHeight / 2, CrossWidth, CrossHeight), CrossTexture, CrossStyle);
        GUI.Box(new Rect(screenWidthHalf + CrossDistance / 2,
            screenHeightHalf - CrossHeight / 2, CrossWidth, CrossHeight), CrossTexture, CrossStyle);
        GUI.Box(new Rect(screenWidthHalf - CrossHeight / 2,
            screenHeightHalf-CrossDistance/2 - CrossWidth , CrossHeight, CrossWidth), CrossTexture, CrossStyle);
        GUI.Box(new Rect(screenWidthHalf - CrossHeight / 2,
            screenHeightHalf + CrossDistance / 2, CrossHeight, CrossWidth), CrossTexture, CrossStyle);

    }
}
