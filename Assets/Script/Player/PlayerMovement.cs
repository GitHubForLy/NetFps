using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{

    //实现玩家的移动，跳跃，蹲伏等基本操作
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(AudioSource))]
    public class PlayerMovement : MonoBehaviour
    {

        public float WalkSpeed = 3f;    // 行走速度
        public float RunSpeed = 5.7f;   //奔跑速度
        public float Gravity = -15f;    // 重力
        public float JumpHeight = 1f;    //跳跃高度
        public float CrouchMoveSpeed = 1.8f; //蹲伏后移动速度
        public float CrouchingSpeed = 5f;       //蹲伏速度
        public float CrouchDeltHeight = 0.5f;  //蹲伏时要减去的高度
        public AudioClip JumpAudio;     //跳跃声音


        public PlayerStatus Status      //玩家当前状态
        {
            get
            {
                if (isJumping)
                    return PlayerStatus.Jumping;
                else
                {
                    if (isCrouching)
                        return PlayerStatus.Crouching;
                    else if (isWaling)
                        return PlayerStatus.Walk;
                    else if (isRunning)
                        return PlayerStatus.Runing;
                    else
                        return PlayerStatus.Idle;
                }
            }
        }


        private AudioSource footstepAudio;
        //private Weapon currentWeapon;
        private float crouchCamY, normalCamY;
        private Transform mainCamreaTransform;
        private PlayerInput playerInput;
        private PlayerCamera playerCamera;

        private bool isJumping = false;
        private bool isRunning = false;
        private bool isWaling = false;
        private bool isCrouching = false;
        private Vector3 normalControllerCenter;
        private float normalControllerHeight;
        private float PosY = 0;
        private Transform camerTrans;
        private PlayerTarget target;
        private int scaleWalkSpeed;

        void Start()
        {
            playerCamera = GetComponentInChildren<PlayerCamera>();
            playerInput = GetComponent<PlayerInput>();
            footstepAudio = GetComponent<AudioSource>();
            //currentWeapon = GameObject.FindGameObjectWithTag(Tags.weapon).GetComponent<Weapon>();
            mainCamreaTransform = playerCamera.transform;
            crouchCamY = mainCamreaTransform.localPosition.y - CrouchDeltHeight;
            normalCamY = mainCamreaTransform.localPosition.y;

            playerInput.OnLogicUpdate += LogicMove;
            camerTrans=transform.Find("FirstPersion/Camera").transform;

            target = GetComponent<PlayerTarget>();
            normalControllerCenter = target.TargetController.center;
            normalControllerHeight = target.TargetController.height;
            GetComponent<Health>().OnDie += PlayerMovement_OnDie;

            scaleWalkSpeed = (int)(WalkSpeed * 1000);
        }

        private void PlayerMovement_OnDie(GameObject deadTank, Behaviour killer)
        {
            target.TargetTransform.Translate(0, -1.146f, 0);
        }

        private void Update()
        {
            //if (Mathf.Abs(Vector3.Distance(transform.position, target.TargetPosition)) > 0.01)
            //{
            //    var tran = playerCamera.transform.rotation;
            //    transform.rotation = Quaternion.Euler(playerCamera.targetPlayerRotation);
            //    playerCamera.Forward = tran;
            //    playerCamera.transform.rotation = tran;
            //}

            //插值到目标位置
            transform.position = Vector3.Lerp(transform.position, target.TargetPosition, Time.deltaTime*10);
            //播放声音
            PlayerAudio();
        }

        //private void FixedUpdate()
        //{
        //    PosY -= Gravity * Time.fixedDeltaTime;
        //    target.Move(new Vector3(0, PosY * Time.fixedDeltaTime, 0));
        //}

        public void LogicMove(InputData data)
        {
            if (PosY < 0 && target.IsGround)
                PosY = 0;

            if (data.InputJump && target.IsGround)
            {
                AudioSource.PlayClipAtPoint(JumpAudio, transform.position);
                //PosY = Mathf.Sqrt(JumpHeight * -3.0f * Gravity)*Time.fixedDeltaTime;
                PosY = JumpHeight;
            }
            //重力影响
            PosY -= Gravity * Time.fixedDeltaTime;
            float x = (int)(data.InputMouseVector.x * GlobalConfig.ScaleRate);
            float y = (int)(PosY * GlobalConfig.ScaleRate);
            float z = (int)(data.InputMouseVector.y * GlobalConfig.ScaleRate);



            //处理移动
            Vector3 move = new Vector3(x, y, z);
            //Vector3 move = new Vector3(data.InputMouseVector.x, 0, data.InputMouseVector.y);

            if (data.InputSprint)
                move = target.TargetTransform.TransformDirection(move) * (int)(RunSpeed * GlobalConfig.ScaleRate);
            else if (data.IsCrouch)
                move = target.TargetTransform.TransformDirection(move) * (int)(CrouchMoveSpeed * GlobalConfig.ScaleRate);
            else
                move = target.TargetTransform.TransformDirection(move) * (int)(WalkSpeed * GlobalConfig.ScaleRate);

            move = move *(int)(Time.fixedDeltaTime* GlobalConfig.ScaleRate) / GlobalConfig.ScaleRate / GlobalConfig.ScaleRate / GlobalConfig.ScaleRate;

            //逻辑移动
            target.Move(move);

            //处理蹲伏
            //ControlCrouch(data.IsCrouch);


            UpdateStatus(data, move);
        }


        //处理蹲伏
        void ControlCrouch(bool crouch)
        {
            if (crouch && target.IsGround)
            {
                isCrouching = true;

                if(BattlegroundManager.Instance.IsLocalPlayer(GetComponent<NetIdentity>()))
                {
                    if (mainCamreaTransform.localPosition.y > crouchCamY)   //没有到达蹲伏位置
                    {
                        mainCamreaTransform.localPosition = new Vector3(mainCamreaTransform.localPosition.x,
                            mainCamreaTransform.localPosition.y - CrouchingSpeed * Time.fixedDeltaTime, mainCamreaTransform.localPosition.z);
                    }
                    if (mainCamreaTransform.localPosition.y < crouchCamY) //超过了蹲伏位置
                        mainCamreaTransform.localPosition = new Vector3(mainCamreaTransform.localPosition.x, crouchCamY, mainCamreaTransform.localPosition.z);
                }

                target.TargetController.height = normalControllerHeight - CrouchDeltHeight;
                target.TargetController.center = new Vector3(normalControllerCenter.x, normalControllerCenter.y - CrouchDeltHeight / 2, normalControllerCenter.z);
            }
            else
            {
                isCrouching = false;

                if (BattlegroundManager.Instance.IsLocalPlayer(GetComponent<NetIdentity>()))
                {
                    if (mainCamreaTransform.localPosition.y < normalCamY)  //处于蹲伏状态
                    {
                        if (mainCamreaTransform.localPosition.y + CrouchingSpeed * Time.fixedDeltaTime > normalCamY)
                        {
                            mainCamreaTransform.localPosition = new Vector3(mainCamreaTransform.localPosition.x,
                                normalCamY, mainCamreaTransform.localPosition.z);
                        }
                        else
                        {
                            mainCamreaTransform.localPosition = new Vector3(mainCamreaTransform.localPosition.x,
                                mainCamreaTransform.localPosition.y + CrouchingSpeed * Time.fixedDeltaTime, mainCamreaTransform.localPosition.z);
                        }


                    }
                }
                target.TargetController.height = normalControllerHeight;
                target.TargetController.center = normalControllerCenter;
            }
        }


        void UpdateStatus(InputData data, Vector3 move)
        {
            if (data.InputJump)
                isJumping = true;
            if (target.IsGround)
                isJumping = false;
            if ((Mathf.Abs(move.x) > 0 || Mathf.Abs(move.z) > 0) && data.InputSprint)
            {
                isRunning = true;
                isWaling = false;
            }
            else if (Mathf.Abs(move.x) > 0 || Mathf.Abs(move.z) > 0)
            {
                isRunning = false;
                isWaling = true;
            }
            else
            {
                isRunning = false;
                isWaling = false;
            }
        }


        private void PlayerAudio()
        {
            switch (Status)
            {
                case PlayerStatus.Jumping:
                    footstepAudio.Stop();
                    break;
                case PlayerStatus.Idle:
                    footstepAudio.Stop();
                    break;
                case PlayerStatus.Walk:
                    footstepAudio.pitch = 1f;
                    if (!footstepAudio.isPlaying)
                        footstepAudio.Play();
                    break;
                case PlayerStatus.Runing:
                    footstepAudio.pitch = 1.3f;
                    if (!footstepAudio.isPlaying)
                        footstepAudio.Play();
                    break;
            }

        }

        private void OnDestroy()
        {
            playerInput.OnLogicUpdate -= LogicMove;
        }
    }
}