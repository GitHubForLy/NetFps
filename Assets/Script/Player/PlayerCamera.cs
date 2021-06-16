using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Game.Net;

namespace Game.Player
{


    public class PlayerCamera : NetBehaviour
    {
        /// <summary>
        /// 鼠标是否锁定
        /// </summary>
        public bool LockCursor
        {
            get
            {
                return Cursor.lockState == CursorLockMode.Locked;
            }
            set
            {
                Cursor.visible = !value;
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }
        /// <summary>
        /// 鼠标灵敏度
        /// </summary>
        public Vector2 MouseSensitivity = new Vector2(3.5f, 3.5f);
        /// <summary>
        /// 绕x轴旋转限制(上下看)
        /// </summary>
        public Vector2 RotateXLimit = new Vector2(-90, 90);
        /// <summary>
        /// 绕Y轴旋转限制(左右看)
        /// </summary>
        public Vector2 RotateYLimit = new Vector2(-360, 360);
        /// <summary>
        /// 相对于父对象(玩家)的偏移
        /// </summary>
        public Vector3 PositionOffset = new Vector3(0, 1.2f, -0.2f);

        public float RotateEnemySpeed = 6;

        public float test = 10f;
        public Animator ThirdPersionAnimator;
        public Quaternion Forward;

        //鼠标移动的值
        private float xAngle = 0;//当前x旋转角
        private float yAngle = 0;//当前y旋转角
        private PlayerInput playerInput;
        private Health playerHealth;
        private bool needRotateEnemy = false;
        private Quaternion lastEnemyRotation;
        private Transform playerTransform;
        public Quaternion targetPlayerRotation, prevPlayerRotation= Quaternion.identity;
        private Quaternion targetRotation, prevRotation= Quaternion.identity;
        private PlayerTarget target;

        void Start()
        {

            transform.localPosition = PositionOffset;

            //playerHealth.OnDied += PlayerHealth_OnDied;
            //playerHealth.OnRevive += PlayerHealth_OnRevive;
            var player = transform.parent.parent;

            target = player.GetComponent<PlayerTarget>();
            //开始时设置正确瞄准方向
            Ray x = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            target.ShootTransform.position = x.origin;
            target.ShootTransform.forward = x.direction;

            if (!IsLocalPlayer)
            {
                GetComponent<AudioListener>().enabled = false;
                GetComponent<Camera>().enabled = false;
                transform.GetChild(0).GetComponent<Camera>().enabled = false;
            }
            else
            {
                player.GetComponent<Health>().OnDie += PlayerCamera_OnDie;
            }
            SetTargetPlayer(player.gameObject);
            
        }

        private void PlayerCamera_OnDie(GameObject deadTank, Behaviour killer)
        {
            transform.GetChild(0).GetComponent<WeaponUI>().enabled = false; //禁用武器瞄准
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);  //隐藏第一人称手柄和武器
        }

        public void SetTargetPlayer(GameObject player)
        {
            playerTransform = player.transform;
            playerInput = player.GetComponent<PlayerInput>();
            playerHealth = playerInput.gameObject.GetComponent<Health>();
            playerInput.OnLogicUpdate += LogicUpdate;
            //target = player.GetComponent<PlayerTarget>();

            prevRotation = transform.rotation;
            prevPlayerRotation = playerTransform.rotation;
        }


        private void PlayerHealth_OnRevive()
        {
            needRotateEnemy = false;
        }

        public void PlayerHealth_OnDied(Behaviour DamageSender, Behaviour sender)
        {
            RotateToEnemy(DamageSender.transform.position);
        }


        public void LogicUpdate(InputData data)
        {
            UpdateInfo(data);


            Quaternion yQuaternion = Quaternion.AngleAxis(yAngle, Vector3.up);
            Quaternion xQuaternion = Quaternion.AngleAxis(0, Vector3.left);
            if (!playerHealth.IsDie)
                target.TargetRotation = yQuaternion * xQuaternion;

            //计算摄像机的垂直旋转角度  并附加上player的水平旋转角度
            xQuaternion = Quaternion.AngleAxis(-xAngle, Vector3.left);
            target.TargetCameraRotation = yQuaternion * xQuaternion;     //！！这里的乘法区分先后顺序（要先旋转yQ再旋转xQ）


            prevRotation = targetRotation;
            prevPlayerRotation = targetPlayerRotation;
        }


        void Update()
        {
            //transform.position = targetTransform.position;
            //if (!playerHealth.IsDie)
            //{
                //计算父对象player的旋转角度 只计算水平方向的旋转
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, target.TargetRotation, Time.deltaTime*test);

                //计算摄像机的垂直旋转角度  并附加上player的水平旋转角度
                transform.rotation = Quaternion.Slerp(transform.rotation, target.TargetCameraRotation, Time.deltaTime*test);


                var aimy = transform.localEulerAngles.x;
                if (aimy > 90)
                    aimy -= 360;
                aimy /= -90;

                //Debug.Log("ry:"+ System.Math.Round(transform.localEulerAngles.y,3) + "  AimX:" + System.Math.Round(x,3));
                ThirdPersionAnimator.SetFloat("AimY", aimy);

            //}
            //else if (needRotateEnemy)
            //{
            //    transform.rotation = Quaternion.Slerp(transform.rotation, lastEnemyRotation, RotateEnemySpeed * Time.deltaTime);
            //    if (Mathf.Abs(Quaternion.Dot(transform.rotation, lastEnemyRotation)) > 0.98f)
            //        needRotateEnemy = false;
            //}
        }

        public void RotateToEnemy(Vector3 EnemyPos)
        {
            needRotateEnemy = true;
            lastEnemyRotation = Quaternion.LookRotation(EnemyPos - transform.position);
        }


        private void UpdateInfo(InputData data)
        {
            if (data.InputSmoothLook == Vector2.zero)//没有鼠标输入
                return;

            var currnetMouseLook=GetMouseLook(data);
            yAngle += currnetMouseLook.x;
            xAngle += currnetMouseLook.y;
            yAngle %= 360f;
            xAngle %= 360f;
            yAngle = Mathf.Clamp(yAngle, RotateYLimit.x, RotateYLimit.y);
            xAngle = Mathf.Clamp(xAngle, RotateXLimit.x, RotateXLimit.y);

        }

        /// <summary>
        /// 获取玩家的用户输入
        /// </summary>
        private Vector3 GetMouseLook(InputData data)
        {
            var currnetMouseLook  =new Vector2((int)(data.InputSmoothLook.x*GlobalConfig.ScaleRate),(int)(data.InputSmoothLook.y* GlobalConfig.ScaleRate));
            currnetMouseLook *= MouseSensitivity* GlobalConfig.ScaleRate;
            currnetMouseLook.y *= -1;
            return currnetMouseLook/ GlobalConfig.ScaleRate / GlobalConfig.ScaleRate;
        }

        //后坐力震动
        public void MakeShake(float angle)
        {
            xAngle -= angle;
        }


        private void OnDestroy()
        {
            playerInput.OnLogicUpdate -= LogicUpdate;
        }

    }

}