using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunGui : MonoBehaviour
{
    public GameObject gunDirection;
    public Texture tex;

    private Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        Ray x= Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2));
        gunDirection.transform.rotation =Quaternion.LookRotation(x.direction);
    }

    // Update is called once per frame
    void Update()
    {
        


    }
    private void OnGUI()
    {
        Vector3 pt;
        Ray s = new Ray(gunDirection.transform.position, gunDirection.transform.forward);

        pt = s.GetPoint(1000);



        //Camera.main.ViewportToScreenPoint

        var vt = Camera.main.WorldToScreenPoint(pt);

        GUI.DrawTexture(new Rect(vt.x - tex.width / 2, vt.y - tex.height / 2, tex.width, tex.height), tex);

    }
}
