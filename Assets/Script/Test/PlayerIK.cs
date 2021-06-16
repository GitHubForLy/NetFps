using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    Animator ThirdPersionAnimator;
    public GameObject t;
    // Start is called before the first frame update
    void Awake()
    {
        ThirdPersionAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 0)
        {
            ThirdPersionAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            ThirdPersionAnimator.SetIKPosition(AvatarIKGoal.LeftHand, t.transform.position);
            ThirdPersionAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            ThirdPersionAnimator.SetIKPosition(AvatarIKGoal.RightHand, t.transform.position);
        }
    }
}
