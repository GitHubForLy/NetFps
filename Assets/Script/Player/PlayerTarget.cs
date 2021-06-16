using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerTarget : MonoBehaviour
    {

        public Transform ShootTransform => shootTransform;

        /// <summary>
        /// 目标旋转
        /// </summary>
        public Quaternion TargetRotation
        {
            get
            { 
                return targetobj.transform.rotation; 
            }
            set
            {
                targetobj.transform.rotation = value;
            }
        }
        /// <summary>
        /// 目标位置
        /// </summary>
        public Transform TargetTransform => targetobj.transform;

        /// <summary>
        /// 目标相机的选择
        /// </summary>
        public Quaternion TargetCameraRotation
        {
            get
            {
                return targetCameraTrans.rotation;
            }
            set
            {
                targetCameraTrans.rotation = value;
            }
        }

        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 TargetPosition => targetobj.transform.position;
        /// <summary>
        /// 是否在地面上
        /// </summary>
        public bool IsGround { get; set; }
        /// <summary>
        /// 目标角色控制器
        /// </summary>
        public CharacterController TargetController => controller;


        [SerializeField]
        private GameObject targetPrefab;

        private Transform shootTransform;
        private Transform targetCameraTrans;
        private GameObject targetobj;
        private CharacterController controller;
        private Health health;
        private bool enable = true;

        // Start is called before the first frame update
        void Awake()
        {
            if(targetobj==null)
            {
                targetobj = Instantiate(targetPrefab, transform.position, transform.rotation);
                targetobj.GetComponent<Target>().RenderGameObject = this.gameObject;
                controller = targetobj.GetComponent<CharacterController>();
                shootTransform = targetobj.transform.Find("CameraTarget/ShootDirection");
                targetCameraTrans = targetobj.transform.Find("CameraTarget");
                health = GetComponent<Health>();
                health.OnDie += Health_OnDie;
            }
        }

        private void Health_OnDie(GameObject deadTank, Behaviour killer)
        {
            enable = false;
            controller.enabled = false;
        }

        public void Move(Vector3 move)
        {
            if (!enable)
                return;
            //targetobj.transform.Translate(move);
            controller.Move(move);
            IsGround = controller.isGrounded;
        }

        private void OnDestroy()
        {
            Destroy(targetobj);
        }

    }
}
