using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigntion : MonoBehaviour
{
    //[Tooltip("寻路点数组")]
    //public Transform[] NavWayPoints;

    //[Tooltip("追逐速度")]
    //public float ChaseSpeed=5f;

    //[Tooltip("巡逻速度")]
    //public float PatrolSpeed = 2f;

    //[Tooltip("巡逻最大等待时间")]
    //public float MaximumPatrolWaitTime = 5f;

    //[Tooltip("巡逻最小等待时间")]
    //public float MinimumPatrolWaitTime = 1f;

    //private Health playerHealth;
    //private NavMeshAgent navAgent;
    //private AIListen aiListen;
    //private int currentNavWayIndex = 0;
    //private float patralWaitTime=0;
    //private float randomPatralWaitTime;
    //private Vector3 lastEnemyPos = Vector3.zero; //最后一次侦听到或看到敌人的位置
    //private bool hasLastEnemyPos = false;//是否存在未到达的 目标位置（侦听到的敌人位置）


    //void Awake()
    //{
    //    playerHealth = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Health>();
    //    navAgent = GetComponent<NavMeshAgent>();
    //    aiListen = GetComponentInChildren<AIListen>();

    //    randomPatralWaitTime = GetPatrolWaitTime();

    //    aiListen.OnListenedEnemy += AiListen_OnListenedEnemy;

    //}

    //private void AiListen_OnListenedEnemy(Vector3 EnemyPos)
    //{
    //    hasLastEnemyPos = true;
    //    lastEnemyPos = EnemyPos;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!playerHealth.IsDie)
    //    {
    //        if (aiListen.IsSightEnemy)
    //        {
    //            //看到敌人不做事情
    //        }
    //        else if (hasLastEnemyPos)    //存在一个尚未到达的敌人位置
    //        {
    //            //追逐
    //            Chase();
    //        }
    //        else
    //        {
    //            Patrol();
    //        }
    //    }
    //    else
    //    {
    //        hasLastEnemyPos = false;
    //        //没有侦听到敌人进行巡逻
    //        Patrol();
    //    }
        
    //}


    ///// <summary>
    ///// 追逐
    ///// </summary>
    //private void Chase()
    //{
    //    navAgent.destination = lastEnemyPos;
    //    navAgent.speed = ChaseSpeed;
        
    //    if(!navAgent.pathPending && navAgent.remainingDistance<navAgent.stoppingDistance)
    //    {
    //        hasLastEnemyPos = false;
    //    }
    //}

    ///// <summary>
    ///// 巡逻
    ///// </summary>
    //private void Patrol()
    //{
    //    if(NavWayPoints.Length>0)
    //    {
    //        navAgent.destination = NavWayPoints[currentNavWayIndex].position;
    //        navAgent.speed = PatrolSpeed;

    //        if (!navAgent.pathPending && navAgent.remainingDistance<navAgent.stoppingDistance)
    //        {
    //            //到达寻路点之后等待
    //            patralWaitTime += Time.deltaTime;
    //            if(patralWaitTime>= randomPatralWaitTime)
    //            {
    //                randomPatralWaitTime = GetPatrolWaitTime();
    //                patralWaitTime = 0;


    //                //currentNavWayIndex = (currentNavWayIndex+1) % NavWayPoints.Length;
    //                currentNavWayIndex = Random.Range(0, NavWayPoints.Length);
    //                navAgent.destination = NavWayPoints[currentNavWayIndex].position;
    //            }
    //        }
    //    }
    //}

    //private float GetPatrolWaitTime()=> Random.Range(MinimumPatrolWaitTime, MaximumPatrolWaitTime);

}

