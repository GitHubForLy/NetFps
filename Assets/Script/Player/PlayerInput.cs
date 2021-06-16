using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public delegate void LogicUpdateEventHandle(InputData data);

    public enum PlayerStatus
    {
        Idle,
        Walk,
        Runing,
        Crouching,
        Jumping
    }

    public class InputData
    {

        /// <summary>
        /// 按下跳起
        /// </summary>
        [HideInInspector]
        public bool InputJump { get; set; }

        /// <summary>
        /// 是否蹲伏
        /// </summary>
        [HideInInspector]
        public bool IsCrouch { get; set; }

        /// <summary>
        /// 是否加速
        /// </summary>
        [HideInInspector]
        public bool InputSprint { get; set; }

        /// <summary>
        /// 是否正在开火
        /// </summary>
        [HideInInspector]
        public bool IsFire { get; set; }

        /// <summary>
        /// 是否在装弹
        /// </summary>
        [HideInInspector]
        public bool IsReload { get; set; }

        /// <summary>
        /// 人物移动值
        /// </summary>
        [HideInInspector]
        public Vector2 InputMouseVector { get; set; } = Vector2.zero;

        /// <summary>
        /// 移动鼠标的值
        /// </summary>
        [HideInInspector]
        public Vector2 InputSmoothLook { get; set; } = Vector2.zero;

        internal void Reset()
        {
            InputMouseVector = Vector2.zero;
            InputSmoothLook = Vector2.zero;
            InputJump = false;
            InputSprint = false;
            IsReload = false;
            //IsFire = false;
            IsCrouch = false;
        }
    }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerInput : MonoBehaviour
    {
        /// <summary>
        /// 输入数据
        /// </summary>
        public InputData Data { get; set; } = new InputData();

        /// <summary>
        /// 返回
        /// </summary>
        [HideInInspector]
        public bool InputCancel { get; set; }

        public event LogicUpdateEventHandle OnLogicUpdate;


        [HideInInspector]
        public bool IsEnableInput
        {
            get
            {
                return isEnableInput;
            }
            set
            {
                if (isEnableInput != value)
                {
                    isEnableInput = value;
                    //if (value)
                    //    EnableInput();
                    //else
                    //    DisableInput();
                }
            }
        }


        private InputController input;
        private GameObject Player;
        private GameObject playerWeaponObj;
        private bool isEnableInput = true;


        void Start()
        {
            input = InputController.Instance;
            //playerWeaponObj = GameObject.FindGameObjectWithTag(Tags.weapon);
        }

        private void Update()
        {
            GetPlayerInput();
        }
        int i = 0;
        private void GetPlayerInput()
        {
            //更新移动向量
            if (isEnableInput)
            {
                var fx = input.GetAxis("Horizontal");
                var fy = input.GetAxis("Vertical");
                if (fx != 0 || fy != 0)
                    Data.InputMouseVector = new Vector2(fx, fy);

                fx = input.GetAxis("Mouse X");
                fy = input.GetAxis("Mouse Y");
                if (fx != 0 || fy != 0)
                    Data.InputSmoothLook = new Vector2(fx, fy);

                var bf = input.GetButton("Fire");
                if (!Data.IsFire)
                    Data.IsFire = bf;

                //Debug.Log("Data.IsFire:"+ Data.IsFire);

                bf = input.GetButton("Crouch");
                if (!(Data.IsCrouch && !bf))
                    Data.IsCrouch = bf;

                bf = input.GetButton("Sprint");
                if (!(Data.InputSprint && !bf))
                    Data.InputSprint = bf;

                bf = input.GetButton("Reload");

                if (!(Data.IsReload && !bf))
                    Data.IsReload = bf;

                bf = input.GetButton("Jump");
                if (!(Data.InputJump && !bf))
                    Data.InputJump = bf;
            }
            InputCancel = input.GetButtonDown("Cancel");
        }




        //float xAngle=0, yAngle = 0;
        public void LogicUpdate(InputData data)
        {
            OnLogicUpdate?.Invoke(data);

            #region 测试
            //var ta = GetComponent<PlayerTarget>();
            //var ca = GetComponentInChildren<PlayerCamera>();
            //var moe = GetComponent<PlayerMovement>();

            //var currnetMouseLook = new Vector2((int)(data.InputSmoothLook.x * 1000), (int)(data.InputSmoothLook.y * 1000));
            //currnetMouseLook *= ca.MouseSensitivity * 1000;
            //currnetMouseLook.y *= -1;
            //currnetMouseLook= currnetMouseLook / 1000 / 1000;

            //yAngle += currnetMouseLook.x;
            //xAngle += currnetMouseLook.y;
            //yAngle %= 360f;
            //xAngle %= 360f;
            //yAngle = Mathf.Clamp(yAngle, ca.RotateYLimit.x, ca.RotateYLimit.y);
            //xAngle = Mathf.Clamp(xAngle, ca.RotateXLimit.x, ca.RotateXLimit.y);

            //Quaternion yQuaternion = Quaternion.AngleAxis(yAngle, Vector3.up);
            //Quaternion xQuaternion = Quaternion.AngleAxis(0, Vector3.left);
            //ta.TargetRotation = yQuaternion * xQuaternion;

            ////计算摄像机的垂直旋转角度  并附加上player的水平旋转角度
            //xQuaternion = Quaternion.AngleAxis(-xAngle, Vector3.left);
            //ta.TargetCameraRotation = yQuaternion * xQuaternion;     //！！这里的乘法区分先后顺序（要先旋转yQ再旋转xQ）




            ///*******/



            //float x = (int)(data.InputMouseVector.x * 1000);
            //float y = (int)(data.InputMouseVector.y * 1000);

            //float scaleWalkSpeed = (int)(moe.WalkSpeed * 1000);
            ////处理移动
            //Vector3 move = new Vector3(x, 0, y);
            //move =ta.TargetTransform.TransformDirection(move) * scaleWalkSpeed;
            //move = move * 20 / 1000 / 1000 / 1000;
            //ta.Move(move);

            //return move.ToRawString();

            #endregion
        }
























        /// <summary>
        /// 禁用输入 禁用摄像机和控制
        /// </summary>
        private void DisableInput()
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            //playerWeaponObj.SetActive(false);
        }

        private void EnableInput()
        {
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            //playerWeaponObj.SetActive(true);
        }
    }

}