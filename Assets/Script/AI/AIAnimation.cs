using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator),typeof(NavMeshAgent),typeof(AIListen))]
public class AIAnimation : MonoBehaviour
{
    //[Tooltip("角色转向目标的过程中 若小于此角度则直接转向目标")]
    //public float DiedAngle=5;

    //private Animator animator;
    //private NavMeshAgent navAgent;
    //private AIListen aiListen;
    //private Health playerHealth;

    //void Awake()
    //{
    //    animator = GetComponent<Animator>();
    //    navAgent = GetComponent<NavMeshAgent>();
    //    aiListen = GetComponent<AIListen>();
    //    playerHealth =GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Health>();

    //    //设置这两个动画层的权重为1  状态机中默认为0 (权重为0时 动画将不起作用)
    //    animator.SetLayerWeight(1, 1f); //Shoot layer  
    //    animator.SetLayerWeight(2, 1f); //Gun Layer  
    //    navAgent.updateRotation = false;    //不使用 导航组件的方向 
    //}

    //private void OnAnimatorMove()
    //{
    //    //根据动画机中的移动距离除以时间得到 动画中的移动的速度
    //    navAgent.velocity = Time.deltaTime==0?Vector3.zero:(animator.deltaPosition / Time.deltaTime);
    //    //navAgent.velocity = new Vector3(navAgent.velocity.x,0, navAgent.velocity.z);
    //    transform.rotation = animator.rootRotation;//不使用 导航组件的方向 而使用动画机中的方向

    //    //if(navAgent.velocity.sqrMagnitude>0)
    //    //{
    //    //播放走路声音
    //    //}
    //}


    //private void Update()
    //{
    //    SetNavParameter();
    //}

    //public void SetSightEnemy(bool sigth)
    //{
    //    if(!playerHealth.IsDie)
    //    animator.SetBool(AnimatorParametars.SightEnemy, sigth);
    //}

    //private void SetNavParameter()
    //{
    //    float angle, speed;
    //    bool EnemyAlive;
    //    EnemyAlive = !playerHealth.IsDie;

    //    if(/*EnemyAlive*/ true)
    //    {
    //        if (aiListen.IsSightEnemy)
    //        {
    //            speed = 0;
    //            angle = GetSignAngle(transform.forward, aiListen.EnemyPosition - transform.position, transform.up);
    //        }
    //        else
    //        {

    //            //Project 投影为使预期速度(desiredVelocity)的方向转为 当前正反向(transform.forward) 但此处只是为了获取速度的值(magnitude) 所以也可不投影
    //            //speed = Vector3.Project(navAgent.desiredVelocity, transform.forward).magnitude;
    //            speed = navAgent.desiredVelocity.magnitude;

    //            //当前方向和 导航预期速度(方向)之间的夹角(弧度).  (Vector3.Angle函数不区分方向)
    //            angle = GetSignAngle(transform.forward, navAgent.desiredVelocity, transform.up);

    //            if (Mathf.Abs(angle) < DiedAngle)
    //            {
    //                //直接转向目标方向
    //                transform.LookAt(transform.position+navAgent.desiredVelocity);
    //                angle = 0;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        speed = 0;
    //        angle = animator.GetFloat(AnimatorParametars.AngleSpeed);
    //    }

        

    //    UpdateAnimatorParameter(angle, speed, EnemyAlive);
    //}

    //private void UpdateAnimatorParameter(float angle,float speed,bool EnemyAlive)
    //{
    //    animator.SetFloat(AnimatorParametars.AngleSpeed, angle,0.7f,Time.deltaTime);
    //    animator.SetFloat(AnimatorParametars.Speed, speed, 0.1f, Time.deltaTime);
    //    animator.SetBool(AnimatorParametars.EnemyAlive, EnemyAlive);
    //}

    ///// <summary>
    ///// 根据两个向量和一个正方向 获取带有方向的角度（弧度）
    ///// </summary>
    ///// <param name="from">向量1</param>
    ///// <param name="to">向量2</param>
    ///// <param name="up">正方向</param>
    //private float GetSignAngle(Vector3 from,Vector3 to,Vector3 up)
    //{
    //    if (to == Vector3.zero)
    //        return 0;

    //    float angle=Vector3.Angle(from, to);    //得到两量之前的夹角
    //    Vector3 normal = Vector3.Cross(from, to);//法向量 (法向量与两个向量的平面垂直)
    //    angle*=Mathf.Sign(Vector3.Dot(normal, up)); //拿法向量与正(上)方向相乘得到符号 再与角度向乘便可得到角度的符号
    //    angle *= Mathf.Deg2Rad;                 //转为弧度

    //    return angle;
    //}
}
