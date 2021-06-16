using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player;


public delegate void ListenedEnemyEvent(Vector3 EnemyPos);

[RequireComponent(typeof(Health),typeof(Animator))]
public class AIListen : MonoBehaviour
{
    //[Tooltip("侦听距离")]
    //public float ListenDistance=10;

    //[Tooltip("视角角度")]
    //public float SightAngle=120;

    //[Tooltip("自己眼睛高度")]
    //public float SelfEyeHeight = 2.6f;

    //[Tooltip("敌人眼睛高度")]
    //public float EnemyEyeHeight = 2.0f;

    //public float LinstenValidHeight = 2f;

    ///// <summary>
    ///// 是否侦听到敌人(实时)
    ///// </summary>
    //[HideInInspector]
    //private bool IsListenedEnemy;

    ///// <summary>
    ///// 是否看到敌人
    ///// </summary>
    //[HideInInspector]
    //public bool IsSightEnemy { get; private set; }

    //[HideInInspector]
    //public Vector3 EnemyPosition { get; private set; }

    //public event ListenedEnemyEvent OnListenedEnemy;


    //private GameObject player;
    //private SphereCollider listenCollider;
    //private PlayerMovement playerMovement;
    //private Health playerHealth;
    //private Animator animator;
    //private Health aiHealth;
    //private Weapon playerWeapon;

    //void Awake()
    //{
    //    listenCollider = GetComponentInChildren<SphereCollider>();
    //    animator = GetComponent<Animator>();
    //    player = GameObject.FindGameObjectWithTag(Tags.player);
    //    playerMovement = player.GetComponent<PlayerMovement>();
    //    playerHealth = player.GetComponent<Health>();
    //    aiHealth = GetComponent<Health>();
    //    playerWeapon = player.GetComponentInChildren<Weapon>();
    //    IsListenedEnemy = false;

    //    aiHealth.OnTakeDamage += AiHealth_OnTakeDamage;
    //    if(GlobalController.GameMode==GameModes.Normal)
    //        playerWeapon.OnFire += PlayerWeapon_OnFire;
    //}

    //private void AiHealth_OnTakeDamage(float Damage,Behaviour sender)
    //{
    //    listenedEnemy(sender.transform.position);
    //}

    //private void PlayerWeapon_OnFire()
    //{
    //    if (Vector3.Distance(transform.position, playerWeapon.gameObject.transform.position) < ListenDistance)
    //        listenedEnemy(playerWeapon.gameObject.transform.position);
    //}


    //private void listenedEnemy(Vector3 EnemyPos)
    //{
    //    if(IsSightEnemy || Mathf.Abs(EnemyPos.y-transform.position.y)< LinstenValidHeight)
    //    {
    //        IsListenedEnemy = true;
    //        EnemyPosition = player.transform.position;
    //        OnListenedEnemy?.Invoke(EnemyPosition);
    //    }
    //}

    //private void Update()
    //{
    //    //if (!playerHealth.IsDie)
    //    //{
    //    //    animator.SetBool(AnimatorParametars.ListenedEnemy, IsSightEnemy);
    //    //}
    //    if (playerHealth.IsDie)
    //        IsSightEnemy = false;
    //}


    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.gameObject== player)
    //    {
    //        IsSightEnemy = false;
    //        Vector3 selfEyePos = new Vector3(transform.position.x, transform.position.y + SelfEyeHeight, transform.position.z);
    //        Vector3 otherEyePos= new Vector3(other.transform.position.x, other.transform.position.y + EnemyEyeHeight, 
    //            other.transform.position.z);

            
    //        var eyeDirection= otherEyePos - selfEyePos;
    //        var eyeAngle = Vector3.Angle(eyeDirection, transform.forward);

    //        var direction = other.transform.position - selfEyePos;
    //        var angle = Vector3.Angle(direction, transform.forward);


    //        if (angle < SightAngle || eyeAngle< SightAngle)
    //        {
    //            RaycastHit hit;
    //            if (Physics.Raycast(selfEyePos, direction.normalized, out  hit, listenCollider.radius))
    //            {
    //                if (hit.collider != null && hit.collider.gameObject == player)
    //                {
    //                    IsSightEnemy = true;
    //                    listenedEnemy(other.transform.position);
    //                    return;
    //                }
    //            }

    //            if (Physics.Raycast(selfEyePos, eyeDirection.normalized, out hit, listenCollider.radius))
    //            {
    //                if (hit.collider != null && hit.collider.gameObject == player)
    //                {
    //                    IsSightEnemy = true;
    //                    listenedEnemy(other.transform.position);
    //                    return;
    //                }
    //            }
    //        }

    //        //判断是否侦听到
    //        var distance = Vector3.Distance(other.transform.position, transform.position);
    //        if(distance<=ListenDistance)
    //        {
    //            if (playerMovement.Status == PlayerStatus.Walk || playerMovement.Status == PlayerStatus.Runing
    //               /* || playerController.Status == PlayerStatus.Jumping*/)
    //            {
    //                listenedEnemy(other.transform.position);
    //            }
    //        }

    //    }
    //}

    

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject==player)
    //    {
    //        IsListenedEnemy = false;
    //        IsSightEnemy = false;
    //    }
    //}

    //private void OnDestroy()
    //{
    //    aiHealth.OnTakeDamage -= AiHealth_OnTakeDamage;
    //    if(GlobalController.GameMode==GameModes.Normal)
    //        playerWeapon.OnFire -= PlayerWeapon_OnFire;
    //}


}
