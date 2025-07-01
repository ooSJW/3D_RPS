using UnityEngine;

public enum WeaponType
{
    AssaultRifle, Pistol, SniperRifle, ShootGun, SubMachineGun, Melee
}
public enum ShotType
{
    Single, Burst, Automatic, Charge
}
public partial class WeaponBase : MonoBehaviour // Data Field
{
    [SerializeField] private WeaponType weaponType;

    [SerializeField] private ShotType CurrentShotType => availableShotTypes[currentShotIndex];
    [SerializeField] private ShotType[] availableShotTypes;
    private int currentShotIndex;

    [SerializeField] private float shotDelayMax;
    protected float shotDelayLeft;

    [SerializeField] private float damageMin;
    [SerializeField] private float damageMax;

    [SerializeField] private int magazineMax;
    protected int magazineCurren;

    protected float accumulateShotTime;

    [SerializeField] string reloadFailAnimation;
    [SerializeField] string reloadStartAnimation;

    public bool Attackable { get; protected set; }
}
public partial class WeaponBase : MonoBehaviour // Initialize
{
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

    public virtual void ChangeShotType()
    {
        currentShotIndex = (currentShotIndex + 1) % availableShotTypes.Length;
    }

    public virtual void ShotStart()
    {

    }

    public virtual void ShotEnd()
    {

    }

    public virtual void Shot()
    {

    }

    public virtual void Hit()
    {

    }
}