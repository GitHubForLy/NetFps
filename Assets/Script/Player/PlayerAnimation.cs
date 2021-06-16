using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public enum ThirdPersionAnimationParameter
    {
        Speed,
        Angle,
        AimX,
        AimY
    }

    public class PlayerAnimation : MonoBehaviour
    {
        public Animator ThirdPersionAnimator;
        public float text = 0.8f;
        public GameObject t;

        private PlayerInput playerinput;
        private Weapon.Weapon weapon;


        // Start is called before the first frame update
        void Start()
        {
            playerinput = GetComponent<PlayerInput>();
            weapon = GetComponent<Weapon.Weapon>();
            playerinput.OnLogicUpdate += Playerinput_OnLogicUpdate;
            GetComponent<Health>().OnDie += Health_OnDie;
        }

        private void Health_OnDie(GameObject deadTank, Behaviour killer)
        {
            ThirdPersionAnimator.SetBool("Died", true);
        }

        private void Update()
        {
            ThirdPersionAnimator.SetBool("Reload", weapon.IsReloading);
        }
        private void Playerinput_OnLogicUpdate(InputData data)
        {
            float angle = 0;
            float speed = 0;

            speed = Mathf.Max(Mathf.Max(Mathf.Abs(data.InputMouseVector.x), Mathf.Abs(data.InputMouseVector.y)),Mathf.Abs(data.InputSmoothLook.x));
            //Debug.Log("speed:"+speed);

            if (data.InputMouseVector.x== 0)
                angle = data.InputMouseVector.y==0?0:Mathf.Sign(data.InputMouseVector.y);
            else if(data.InputMouseVector.x>0)
                angle = data.InputMouseVector.x / 2 - data.InputMouseVector.y / 2;
            else if(data.InputMouseVector.x<0)
                angle = data.InputMouseVector.x / 2 + data.InputMouseVector.y / 2;

            ThirdPersionAnimator.SetFloat("Speed", speed);
            ThirdPersionAnimator.SetFloat("Angle", angle, text, Time.deltaTime);
        }




        public void UpdateAimX(float value)
        {
            ThirdPersionAnimator.SetFloat("AimX", value);
        }
        public void UpdateAimY(float value)
        {
            ThirdPersionAnimator.SetFloat("AimY", value);
        }

        private void OnDestroy()
        {
            GetComponent<Health>().OnDie -= Health_OnDie;
            playerinput.OnLogicUpdate -= Playerinput_OnLogicUpdate;
        }

    }

}
