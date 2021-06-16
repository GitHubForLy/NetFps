using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public float FadeSpeed=0.5f;
    public Text info;

    private bool isStart = true;
    private bool isEnd = false;
    private Image backgroundImage;


    // Start is called before the first frame update
    void Start()
    {
        backgroundImage = GetComponent<Image>();
        backgroundImage.color=new Color(backgroundImage.color.r, backgroundImage.color.g,
            backgroundImage.color.b, 1);
    }

    void Update()
    {
        if (isEnd)
        {
            FadeOut();
        }
        else if (isStart)
        {
            FadeIn();
        }

    }


    private void FadeIn()
    {
        backgroundImage.color = Color.Lerp(backgroundImage.color, Color.clear, FadeSpeed*Time.deltaTime);
        if (backgroundImage.color.a < 0.05f)//因为是插值算法 所以不可能等=0 只能尽可能的小
        {
            backgroundImage.color = Color.clear;
            backgroundImage.enabled = false;
            isStart = false;
        }
    }

    private void FadeOut()
    {
        backgroundImage.color = Color.Lerp(backgroundImage.color, Color.black, FadeSpeed * Time.deltaTime);
        if (backgroundImage.color.a > 0.95f)
        {
            isEnd = false;
            SceneManager.LoadScene("Demo");
        }
    }

    public void EndScene()
    {
        backgroundImage.enabled = true;
        isEnd = true;
    }
}
