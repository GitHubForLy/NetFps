using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player;
using Game;

public class test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Ray x = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Camera.main.GetComponent<LineRenderer>().SetPosition(0, x.origin);
        Camera.main.GetComponent<LineRenderer>().SetPosition(1, x.GetPoint(3000));

        Camera.main.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, Camera.main.transform.position);
        Camera.main.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, Camera.main.transform.position+ Camera.main.transform.forward*3000);
    }

    // Update is called once per frame
    void OnGUI()
    {

        Ray x = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        GUILayout.Label(x.origin.ToRawString());
        GUILayout.Label(Camera.main.transform.position.ToRawString());
    }

    private void Update()
    {
        //Ray x = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        //Camera.main.GetComponent<LineRenderer>().SetPosition(0, x.origin);
        //Camera.main.GetComponent<LineRenderer>().SetPosition(1, x.GetPoint(3000));

        //Debug.DrawRay(x.origin, x.direction,Color.red);
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.blue);
    }
}
