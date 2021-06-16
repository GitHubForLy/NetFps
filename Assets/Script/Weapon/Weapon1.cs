//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public delegate void FireEventHandle1();

//[RequireComponent(typeof(Animation))]
//public class Weapon1 : MonoBehaviour
//{
//    [Tooltip("弹夹容量"),Header("WeaponConfig")]
//    public int ClipSize=40;

//    [Tooltip("最大子弹数量")]
//    public int MaxAmmoCount = 240;

//    [Tooltip("射速(s)")]
//    public float ShootSpeed = 0.5f;

//    [Tooltip("伤害(s)")]
//    public int Damage = 25;

//    [Tooltip("射击垂直幅度半径")]
//    public float RangeRadius = 10f;

//    [Tooltip("开火声音"), Header("Audio"),Space(15)]
//    public AudioClip FireAudio;

//    [Tooltip("造成伤害声音")]
//    public AudioClip TakeDamageAudio;

//    [Tooltip("装弹声音")]
//    public AudioClip ReloadAudio;

//    [Tooltip("没有子弹声音")]
//    public AudioClip EmptyAudio;

//    [Tooltip("开火动画名称"), Header("Animation"), Space(15)]
//    public string FireAnimName = "Single_Shot";

//    [Tooltip("装弹动画名称")]
//    public string ReloadAnimName = "Reload";

//    [Tooltip("空闲动画名称")]
//    public string IdleAnimName = "Idle";

//    [Tooltip("走路动画名称")]
//    public string WalkAnimName = "Walk";

//    [Tooltip("奔跑动画名称")]
//    public string RunAnimName = "Run";

//    [Tooltip("设置显示子弹数量文字"),Space(15)]
//    public GameObject AmmoText;

//    [Tooltip("击中敌人特效")]
//    public GameObject FireEnemyImpact;

//    [Tooltip("击中墙壁特效")]
//    public GameObject FireStaticImpact;

//    [Tooltip("震动幅度")]
//    public float ShakeAngle = 0.35f;

//    [Tooltip("射击火焰")]
//    public GameObject FireFlash;

//    //开火时
//    public event FireEventHandle OnFire;

//    //当前弹夹的子弹数量
//    private int currentAmmoCount;
//    //所有可用子弹数量 包括当前弹夹子弹数
//    private int enableAmmoCount;
//    private float lastFireTime;
//    private bool isFiring;
//    private bool isReloading;

//    private Animation weaponAnimation;
//    private PlayerController playerController;
//    private PlayerInput playerInput;
//    private Coroutine reloadCorutine;
//    private PlayerCamera pcamera;

//    // Start is called before the first frame update
//    void Start()
//    {   
//        weaponAnimation = GetComponent<Animation>();
//        playerInput = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerInput>();
//        playerController = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerController>();
//        pcamera= GameObject.FindGameObjectWithTag(Tags.player).GetComponentInChildren<PlayerCamera>();
//        currentAmmoCount = ClipSize;
//        enableAmmoCount = MaxAmmoCount - currentAmmoCount;
//        UpdateAmmoText();
//    }

//    //重置武器子弹数和状态
//    public void ReSet()
//    {
//        weaponAnimation.Stop();
//        currentAmmoCount = ClipSize;
//        enableAmmoCount = MaxAmmoCount - currentAmmoCount;
//        UpdateAmmoText();
//        isFiring = false;
//        isReloading = false;
//    }

//    private void Update()
//    {
//        if (isFiring && !playerInput.IsFire)
//        {
//            StopFire();
//        }
//        else if (!isFiring && playerInput.IsFire)
//        {
//            StartFire();
//        }
//        if (!isReloading && playerInput.IsReload)
//        {
//            Reload();
//        }

//        if (!isFiring && !isReloading)
//            PlayAnimWithPlayer();
//    }


//    public void StartFire()
//    {
//        if(CanFire() && !isFiring)
//        {
//            isFiring = true;
//            if (currentAmmoCount>0)
//            {
//                if(isReloading)
//                {
//                    isReloading = false;
//                    StopCoroutine(reloadCorutine);
//                }

//                currentAmmoCount--;
//                enableAmmoCount--;
//                DoFire();
//                lastFireTime = Time.time;
//                AudioSource.PlayClipAtPoint(FireAudio, transform.position);
//                PlayAnim(FireAnimName);
//                pcamera.MakeShake(ShakeAngle);
//                FireFlash.SetActive(true);
//                //UpdateAmmoText();
//                OnFire?.Invoke();
//                StartCoroutine(FireFinished(ShootSpeed));                
//            }
//            else if(enableAmmoCount>0)
//            {
//                Reload();
//            }
//        }
//        else if(enableAmmoCount<=0)
//        {
//            AudioSource.PlayClipAtPoint(EmptyAudio, transform.position);
//        }
//    }

//    //获取射击的射线
//    private Ray GetShootRay()
//    {
//        float hdelt=Random.Range(-RangeRadius, RangeRadius);
//        float vdelt = Random.Range(-RangeRadius, RangeRadius);
//        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 + hdelt, Screen.height / 2 + vdelt));
//        return ray;
//    }

//    private void DoFire()
//    {
//        Ray ray = GetShootRay();
//        GameObject ImpactEffect;
//        if(Physics.Raycast(ray, out RaycastHit hit))
//        {
//            if (hit.collider is CapsuleCollider &&
//                hit.collider.gameObject .CompareTag(Tags.enemy))
//            {
//                var robatHealth = hit.collider.gameObject.GetComponent<AIHealth>();
//                robatHealth?.TakeDamage(Damage,this);

//                AudioSource.PlayClipAtPoint(TakeDamageAudio, hit.point);
//                ImpactEffect = FireEnemyImpact;
//            }
//            else
//            {
//                ImpactEffect = FireStaticImpact;
//            }
//            var explose = Instantiate(ImpactEffect, hit.point, Quaternion.identity);
//            Destroy(explose, 1.5f);
//        }
        
//    }

//    public void StopFire()
//    {
//        isFiring = false;
//    }

//    //开火完成
//    private IEnumerator FireFinished(float duration)
//    {
//        yield return new WaitForSeconds(duration);
//        fireFlash.SetActive(false);
//        isFiring = false;
//    }

//    //装子弹完成
//    private IEnumerator ReloadFinished(float duration)
//    {
//        yield return new WaitForSeconds(duration);

//        currentAmmoCount += Mathf.Min(ClipSize - currentAmmoCount, enableAmmoCount - currentAmmoCount);
//        UpdateAmmoText();
//        isReloading = false;

//        if (isFiring)
//            StartFire();
//    }

//    /// <summary>
//    /// 装弹
//    /// </summary>
//    public void Reload()
//    {
//        if(enableAmmoCount>currentAmmoCount && currentAmmoCount<ClipSize)
//        {
//            isReloading = true;
//            AudioSource.PlayClipAtPoint(ReloadAudio, transform.position);
//            float Duration=PlayAnim(ReloadAnimName,true);
//            reloadCorutine=StartCoroutine(ReloadFinished(Duration));
//        }
//    }

//    private bool CanFire()
//    {
//        return enableAmmoCount > 0 && Time.time-lastFireTime>ShootSpeed;
//    }

//    private void UpdateAmmoText()
//    {
//        AmmoText.GetComponent<Text>().text = $"{currentAmmoCount}/{enableAmmoCount - currentAmmoCount}";
//    }

//    //根据人物状态播放武器动画 不包括开火及装弹
//    private void PlayAnimWithPlayer()
//    {
//        if (isFiring || isReloading)
//            return;
//        switch (playerController.Status)
//        {
//            case PlayerStatus.Idle:
//            case PlayerStatus.Jumping:
//                PlayAnim(IdleAnimName);
//                break;
//            case PlayerStatus.Runing:
//                PlayAnim(RunAnimName);
//                break;
//            case PlayerStatus.Walk:
//                PlayAnim(WalkAnimName);
//                break;
//        }
//    }

//    private float PlayAnim(string AnimName,bool isRestart = false)
//    {
//        if(isRestart)
//        {
//            weaponAnimation.Stop(AnimName);
//            weaponAnimation.Rewind(AnimName);//倒回动画
//        }
//        if(!weaponAnimation.IsPlaying(AnimName))
//            weaponAnimation.Play(AnimName);
//        return weaponAnimation[AnimName].clip.length;
//    }

//    private void OnDisable()
//    {
//        fireFlash.SetActive(false);
//    }

//}
