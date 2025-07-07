using System;
using System.Collections;
using UnityEngine;

public enum WeaponType
{
    M4a1, Aug, Pistol, SniperRifle, ShootGun, SubMachineGun, Melee
}
public enum ShotType
{
    Single, Burst, Automatic, Charge
}

public enum BulletHitType
{
    HitScan, Projectile
}

public delegate void DelegateShotTypeChanged(ShotType newType);
public delegate void DelegateShotFunction(SocketBase from, Ray ray);
public delegate void DelegateShotAnimation(AnimationType wantType);

[RequireComponent(typeof(Animator))]
public partial class WeaponBase : MonoBehaviour // Data Field
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private LayerMask attackMask = -1;
    [SerializeField] private AnimationCurve trajectoryCurve;

    public event DelegateShotTypeChanged OnShotTypeChanged;
    public event DelegateShotFunction OnShotFunction;
    public event DelegateShotAnimation OnShotAnimation;

    [SerializeField] private ShotType currentShotType;
    public ShotType CurrentShotType
    {
        get => currentShotType;
        set
        {
            int index = Array.IndexOf(availableShotTypes, value);
            if (index < 0) return;
            currentShotIndex = index;
            currentShotType = value;
            OnShotTypeChanged?.Invoke(CurrentShotType);
        }
    }

    [SerializeField] private ShotType[] availableShotTypes = new ShotType[] { ShotType.Single };
    private int currentShotIndex;
    public int CurrentShotIndex
    {
        get => currentShotIndex;
        set
        {
            currentShotIndex = value % availableShotTypes.Length;
            CurrentShotType = availableShotTypes[currentShotIndex];
            OnShotTypeChanged?.Invoke(CurrentShotType);
        }
    }

    [SerializeField] private BulletHitType bulletHitType;
    public BulletHitType BulletHitType
    {
        get => bulletHitType;
        set
        {
            bulletHitType = value;
            switch (value)
            {
                case BulletHitType.HitScan:
                    OnShotFunction = HitScan;
                    break;
                case BulletHitType.Projectile:
                    OnShotFunction = Projectile;
                    break;
            }
        }
    }

    [SerializeField] private AmmoType requireAmmo;
    public AmmoType RequireAmmo => requireAmmo;


    [SerializeField] private float shotDelayMax;
    [SerializeField] private float burstDelay;
    protected float shotDelayLeft;

    [SerializeField] private float damageMin;
    [SerializeField] private float damageMax;

    // 탄 퍼짐 각도
    [SerializeField] private float shotVerticalSpread;
    [SerializeField] private float shotHorizontalSpread;

    [SerializeField] private int shotAmount = 1;
    [SerializeField] private int burstAmount = 3;
    [SerializeField] private int magazineMax;
    protected int magazineCurren;

    protected float accumulateShotTime;
    protected float startShotTime;
    protected float currentTime;

    [SerializeField] string reloadFailAnimation = "ReloadFail";
    [SerializeField] string reloadStartAnimation = "Reload";

    public bool Attackable { get; protected set; }
    private bool isFiring = false;

    private Animator animator;
}
public partial class WeaponBase : MonoBehaviour // Initialize
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
        OnShotAnimation -= TriggerWeaponAnimation;
        OnShotAnimation += TriggerWeaponAnimation;
        // 에디터에 설정한 값 적용, index반영, 예외처리, ShotType변경 이벤트 호출
        CurrentShotType = CurrentShotType;
        BulletHitType = BulletHitType;
    }
    private void Allocate()
    {

    }
    public void Initialize()
    {
        Allocate();
        Setup();
    }
    private void Setup()
    {

    }
}
public partial class WeaponBase : MonoBehaviour // 
{
    public virtual AnimationType GetWeaponAnimationType()
    {
        switch (weaponType)
        {
            case WeaponType.Aug:
                return AnimationType.Aug;
            case WeaponType.ShootGun:
                return AnimationType.ShotGun;
            default:
                return AnimationType.M4a1;
        }
    }


    /// <summary>장전 시작</summary>
    /// <param name="spareAmmo">남은 장탄 수</param>
    /// <returns>시작할 애니메이션 이름</returns>
    public virtual string ReloadStart(int spareAmmo)
    {
        if (spareAmmo <= 0 || magazineCurren >= magazineMax)
            return reloadFailAnimation;
        else
            return reloadStartAnimation;
    }

    /// <summary> 재장전 완료 시 탄창 회복 </summary>
    /// <param name="spareAmmo">남은 장탄 수</param>
    /// <returns>장전에 사용한 탄 수</returns>
    public virtual int ReloadComplete(int spareAmmo)
    {
        int filledCount = Mathf.Min(magazineMax - magazineCurren, spareAmmo);
        magazineCurren += filledCount;
        return filledCount;
    }



    public virtual void ChangeShotType() => CurrentShotIndex++;

    public virtual void ShotStart(SocketBase from)
    {
        isFiring = true;
        startShotTime = Time.time;
        switch (CurrentShotType)
        {
            case ShotType.Single:
                Shot(from);
                break;
        }
    }

    public virtual void ShotUpdate(SocketBase from)
    {
        switch (CurrentShotType)
        {
            case ShotType.Automatic:
                if (isFiring) Shot(from);
                break;
            case ShotType.Burst:
                if (isFiring) BurstShot(from);
                break;
        }
    }
    public virtual void ShotEnd(SocketBase from)
    {
        isFiring = false;
        accumulateShotTime = currentTime - startShotTime;
        switch (CurrentShotType)
        {
            case ShotType.Charge:
                Shot(from);
                break;
        }
    }

    public virtual void TimeUpdate()
    {
        currentTime = Time.time;
    }

    public virtual void TriggerWeaponAnimation(AnimationType wantType) => animator.SetTrigger(wantType.ToString());
    public virtual void ShotAnimation()
    {
        OnShotAnimation?.Invoke(AnimationType.Shot);
    }

    // force : "강제하다" 라는 의미로 사용
    // or : 왼쪽 부터 true를 만나는 순간 종료 => true가능성이 가장 높고 빠른 연산을 왼쪽에 두는 것이 성능에 유리함
    // and : 왼쪽 부터 flat를 만나는 순간 종료 => false가능성이 가장 높고 빠른 연산을 왼쪽에 두는 것이 성능에 유리함
    public virtual void Shot(SocketBase from, bool force = false)
    {
        if (force || shotDelayLeft == currentTime || shotDelayLeft + shotDelayMax <= currentTime)
        {
            ShotAnimation();
            accumulateShotTime = currentTime - startShotTime;
            Vector3 originDirection = from.transform.forward;
            Ray ray = new(from.transform.position, originDirection);

            for (int i = 0; i < shotAmount; i++)
            {
                Vector2 rand = UnityEngine.Random.insideUnitCircle * new Vector2(shotVerticalSpread, shotHorizontalSpread);
                ray.direction = originDirection.Rotation(rand.x, rand.y);

                OnShotFunction(from, ray);
            }
            shotDelayLeft = currentTime;
        }
    }

    public virtual void BurstShot(SocketBase from)
    {
        if (shotDelayLeft + shotDelayMax > currentTime) return;

        StartCoroutine(BurstCoroutine(from));

        shotDelayLeft = currentTime + (burstAmount * burstDelay);
    }

    public virtual void HitScan(SocketBase from, Ray ray)
    {
        if (ray.CurveCastWithDebug(out RaycastHit hit, 20.0f, attackMask, trajectoryCurve, 8, 1.0f))
        {
            Hit(hit.rigidbody?.gameObject, hit.point, hit.normal);
        }
    }

    public virtual void Projectile(SocketBase from, Ray ray)
    {

    }

    public virtual void Hit(GameObject hitTarget, Vector3 hitPoint, Vector3 hitNormal)
    {
        PoolManager.ClaimSpawn(EffectType.BulletHitEffect.ToString(), hitPoint + (hitPoint * 0.01f), Quaternion.LookRotation(-hitNormal));
    }
}

public partial class WeaponBase : MonoBehaviour // Coroutine
{
    public virtual IEnumerator BurstCoroutine(SocketBase from)
    {
        for (int i = burstAmount - 1; i >= 0; i--)
        {
            Shot(from, true);

            if (i == 0) yield break;

            yield return new WaitForSeconds(burstDelay);
        }
    }
}