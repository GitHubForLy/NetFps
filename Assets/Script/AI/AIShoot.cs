using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
//    [Tooltip("根据离敌人计算出的最大伤害")]
//    public float MaximumDamage=80;
//    [Tooltip("根据离敌人计算出的最小伤害")]
//    public float MinimumDamage=15;
//    [Tooltip("射击动画的名称")]
//    public string ShootStateName;
//    [Tooltip("开枪时闪光强度")]
//    public float ShotLightIntensity;
//    [Tooltip("射速")]
//    public float ShootSpeed=1.2f;
//    [Tooltip("每次丢失敌人视野后 再次看到敌人时首次开枪等待最大时间"),Range(0, 10)]
//    public float SightEnemyWaitShootMaxTime=1.2f;
//    [Tooltip("每次丢失敌人视野后 再次看到敌人时首次开枪等待最小时间"),Range(0,10)]
//    public float SightEnemyWaitShootMinTime = 0.3f;
//    [Tooltip("射击精准度（圆锥半角）")]
//    public float MaxShootHalfAngle=5f;
//    public AudioClip ShotAudio;
//    public float ShotLightFadeSpeed=10;

//    public GameObject WeaponObj;

//    private Animator animator;
//    private Transform playerTransform;
//    private SphereCollider listenCollider;
//    private Health playerHealth;
//    private LineRenderer laserShotLine;//射击特效
//    private Light shotLight;
//    private AIListen aiListen;
//    private float firstSightEnemyTime;
//    private float waitShootTime;
//    private float LastFireTime;
//    private bool isShooting = false;
//    private bool SightEnemyPre = false;

//    // Start is called before the first frame update
//    void Awake()
//    {
//        playerTransform = GameObject.FindGameObjectWithTag(Tags.player).transform;
//        animator = GetComponent<Animator>();
//        playerHealth = playerTransform.gameObject.GetComponent<Health>();
//        listenCollider = GetComponentInChildren<SphereCollider>();
//        laserShotLine = GetComponentInChildren<LineRenderer>();
//        shotLight = GetComponentInChildren<Light>();
//        aiListen = GetComponent<AIListen>();

//        shotLight.intensity = 0;
//        laserShotLine.enabled = false;
//    }

//    private void Update()
//    {
//        if (aiListen.IsSightEnemy && !playerHealth.IsDie)
//        {
//            if(Time.time>=LastFireTime+ShootSpeed)//射速判断
//            {
//                if(!SightEnemyPre)//再次看到敌人时
//                {
//                    firstSightEnemyTime = Time.time;
//                    waitShootTime = Random.Range(SightEnemyWaitShootMinTime, SightEnemyWaitShootMaxTime);
//                }
//                if (Time.time >= waitShootTime+ firstSightEnemyTime) //每次丢失敌人视野 再次看到敌人时 进行射击要等待的时间
//                {
//                    Shoot();
//                    LastFireTime = Time.time;
//                }
//            }
//        }
//        SightEnemyPre = aiListen.IsSightEnemy;

//        float shotCure = animator.GetFloat(AnimatorParametars.Shot);
//        if (shotCure < 0.1f)
//        {
//            //laserShotLine.enabled = false;
//            isShooting = false;
//        }

//        shotLight.intensity = Mathf.Lerp(shotLight.intensity, 0, ShotLightFadeSpeed * Time.deltaTime);
//    }


//    private void OnAnimatorIK(int layerIndex)
//    {
//        if (aiListen.IsSightEnemy)
//        {
//            float weight = animator.GetFloat(AnimatorParametars.AnimWeight);
//            //设置右手的朝向
//            animator.SetIKPosition(AvatarIKGoal.RightHand, playerTransform.position);
//            //设置右手ik的权重
//            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
//        }
//    }

//    public void Shoot(/*Vector3 ShootDir*/)
//    {
//        animator.Play(ShootStateName,1);

//        isShooting = true;
//        //根据距离得到伤害比率
//        float DamageRate = (listenCollider.radius - Vector3.Distance(playerTransform.position, transform.position)) / listenCollider.radius;
//        //实际伤害
//        float damage = MinimumDamage + (MaximumDamage - MinimumDamage) * DamageRate;

//        Health hitHealth;
//        RaycastHit hit;

//        float u = 1f / 90f;
//        float p = u * MaxShootHalfAngle;


//        var dir = (playerTransform.position+Vector3.up - WeaponObj.transform.position).normalized;
//        dir.y += Random.Range(-p, p);
//        dir.z += Random.Range(-p, p);
//        dir.x += Random.Range(-p, p);
//        if (Physics.Raycast(WeaponObj.transform.position, dir, out hit))
//        {
//            if(hit.collider is CharacterController)
//            {
//                hitHealth = hit.collider.gameObject.GetComponent<Health>();
//                if(!hitHealth.IsDie)
//                    hitHealth.TakeDamage(damage, this);
//            }
//        }
//        SpawnShotEffects(dir*20);
//    }

//    private void SpawnShotEffects(Vector3 pos)
//    {
//        //laserShotLine.SetPosition(0, laserShotLine.transform.position); //线起点默认是0 所以不设置了
//        //laserShotLine.SetPosition(1, laserShotLine.transform.InverseTransformPoint(playerTransform.position + Vector3.up * 1.2f));//线终点
//        laserShotLine.SetPosition(1, pos);

//        laserShotLine.enabled = true;

//        shotLight.intensity = ShotLightIntensity;
//        AudioSource.PlayClipAtPoint(ShotAudio, laserShotLine.transform.position);
//    }
}
