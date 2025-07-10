using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


public partial class CharacterAttackModule : CharacterModuleBase
{
    [SerializeField] private AnimationCurve trajectoryCurve;
    [SerializeField] private LayerMask attackMask;

    [SerializeField] private ObjectType[] weaponTypes;
    private Vector3? attackDirection = null;

    private WeaponBase[] weapons;
    int weaponIndex = -1;
    protected int WeaponIndex
    {
        get => weaponIndex;
        set
        {
            if (weaponIndex == value || weapons is null) return;

            value = value % weapons.Length;
            if (value < 0) value += weapons.Length;

            NextWeapon = weapons[weaponIndex = value];
        }
    }
    private WeaponBase nextWeapon;
    public WeaponBase NextWeapon
    {
        get => nextWeapon;
        set
        {
            if (nextWeapon == value || currentWeapon == value) return;

            StartWeaponSwap(value?.GetWeaponAnimationType().ToString(), nextWeapon?.GetWeaponAnimationType().ToString());

            nextWeapon = value;
        }
    }

    private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon
    {
        get => currentWeapon;
        set
        {
            if (currentWeapon != value)
            {
                if (currentWeapon) DisConnectWeapon(currentWeapon);

                currentWeapon = value;
                ConnectWeapon(currentWeapon);
            }
        }
    }
}


public partial class CharacterAttackModule : CharacterModuleBase
{
    public void DisConnectWeapon(WeaponBase oldWeapon)
    {
        oldWeapon.gameObject.SetActive(false);
        if (Owner)
        {
            oldWeapon.OnShotAnimation -= Owner.AnimationPlay;
            oldWeapon.OnGetShotPosition -= GetShotPosition;
        }
        oldWeapon.OnHitCharacter -= OnHitCharacter;
    }

    public void ConnectWeapon(WeaponBase newWeapon)
    {
        if (newWeapon is null) return;

        newWeapon.gameObject.SetActive(true);
        if (Owner)
        {
            newWeapon.OnShotAnimation -= Owner.AnimationPlay;
            newWeapon.OnShotAnimation += Owner.AnimationPlay;
            newWeapon.OnGetShotPosition -= GetShotPosition;
            newWeapon.OnGetShotPosition += GetShotPosition;
        }
        newWeapon.OnHitCharacter -= OnHitCharacter;
        newWeapon.OnHitCharacter += OnHitCharacter;
        if (attackDirection is not null) OnAttack(attackDirection.Value, true);
    }

    public void RefreshWeapon() => CurrentWeapon = CurrentWeapon;
    public void SetWeapon(int wantIndex) => WeaponIndex = wantIndex;

    public void InitializeWeapon()
    {
        if (weaponTypes is null) return;
        weapons = new WeaponBase[weaponTypes.Length];

        for (int i = 0; i < weaponTypes.Length; i++)
        {
            if (PoolManager.ClaimSpawn(weaponTypes[i].ToString()).TryGetComponent(out WeaponBase asWeapon))
            {
                Owner.SocketActionByType(SocketType.Hand_R, targetSocket => targetSocket.AttachTransform(asWeapon.transform));
                weapons[i] = asWeapon;
                weapons[i].gameObject.SetActive(false);
            }
        }
        SetWeapon(0);
    }

    public override void Attach(CharacterBase target)
    {
        base.Attach(target);
        if (Owner is not null)
        {
            Owner.OnModuleLoaded -= InitializeWeapon;
            Owner.OnModuleLoaded += InitializeWeapon;
            Owner.OnWeapon -= SetWeapon;
            Owner.OnWeapon += SetWeapon;
            Owner.OnAttack -= OnAttack;
            Owner.OnAttack += OnAttack;
            Owner.OnReload -= OnReloadStart;
            Owner.OnReload += OnReloadStart;
            Owner.OnWeaponSwap -= OnWeaponSwap;
            Owner.OnWeaponSwap += OnWeaponSwap;
            Owner.OnReloadComplete -= OnReloadComplete;
            Owner.OnReloadComplete += OnReloadComplete;
            GameManager.OnObjectUpdate -= OnUpdate;
            GameManager.OnObjectUpdate += OnUpdate;
        }
    }

    public override void Dettach()
    {
        base.Dettach();
        if (Owner is not null)
        {
            Owner.OnModuleLoaded -= InitializeWeapon;
            Owner.OnWeapon -= SetWeapon;
            Owner.OnAttack -= OnAttack;
            Owner.OnReload -= OnReloadStart;
            Owner.OnWeaponSwap -= OnWeaponSwap;
            Owner.OnReloadComplete -= OnReloadComplete;
            GameManager.OnObjectUpdate -= OnUpdate;
        }
    }

    public virtual void OnUpdate(float deltaTime)
    {
        if (Owner && currentWeapon)
        {
            currentWeapon.TimeUpdate();
            currentWeapon.SocketActionByType(SocketType.Muzzle, currentWeapon.ShotUpdate);
        }
    }

    public virtual void StartWeaponSwap(string? newWeaponTrigger, string? cancelWeaponTrigger)
    {
        if (string.IsNullOrEmpty(cancelWeaponTrigger))
        {
            if (currentWeapon is not null)
                Owner.AnimationPlay(AnimationType.Holstering);
        }
        else
        {
            Owner.AnimationCancel(cancelWeaponTrigger);
        }
        if (!string.IsNullOrEmpty(newWeaponTrigger)) Owner.AnimationPlay(newWeaponTrigger);
    }

    public virtual Vector3 GetShotPosition(SocketBase from)
    {
        return Owner.focusLocation;
    }

    public virtual void OnWeaponSwap(SwapState currentState)
    {
        switch (currentState)
        {
            case SwapState.HolsteringStart:
                CurrentWeapon?.HolsteringStart();
                break;
            case SwapState.HolsteringEnd:
                CurrentWeapon?.HolsteringComplete();
                break;
            case SwapState.DrawStart:
                NextWeapon?.DrawStart();
                CurrentWeapon = nextWeapon;
                NextWeapon = null;
                break;
            case SwapState.DrawEnd:
                CurrentWeapon?.DrawComplete();
                break;
        }
    }


    public virtual void OnReloadStart()
    {
        if (Owner && currentWeapon)
        {
            string animationName = currentWeapon.ReloadStart(Owner.GetSpareAmmo(currentWeapon.RequireAmmo));
            if (!string.IsNullOrEmpty(animationName)) Owner.AnimationPlay(animationName);
        }
    }

    public virtual void OnReloadComplete()
    {
        if (Owner && currentWeapon)
        {
            int ammoUse = currentWeapon.ReloadComplete(Owner.GetSpareAmmo(currentWeapon.RequireAmmo));
            Owner.AddSpareAmmo(currentWeapon.RequireAmmo, -ammoUse);
        }
    }

    public virtual void OnAttack(Vector3 direction, bool isDown)
    {
        attackDirection = isDown ? direction : null;

        if (Owner is not null && currentWeapon is not null)
        {
            Action<SocketBase> attackAction = isDown ? currentWeapon.ShotStart : currentWeapon.ShotEnd;
            currentWeapon.TimeUpdate();
            currentWeapon.SocketActionByType(SocketType.Muzzle, attackAction);
        }
    }

    public virtual void OnHitCharacter(CharacterBase hitCharacter, Collider hitCollider, Vector3 hitPoint, Vector3 hitNormal, CharacterPartType hitPartType, float resultDamage)
    {
        float damageMultiplier = CharacterParts.GetDamageMultiplier(hitPartType);
        bool isCritical = hitPartType == CharacterPartType.Head;
        Vector3 originPosition;
        if (Owner)
        {
            originPosition = Owner.transform.position;
            Owner.CalculateDamage(ref resultDamage, hitPartType, ref damageMultiplier, ref isCritical);
        }
        else
            originPosition = transform.position;

        hitCharacter.GetDamage
            (
            resultDamage * damageMultiplier,
            (hitCharacter.transform.position - originPosition).normalized,
            hitPartType,
            isCritical,
            Owner?.gameObject
            );
    }

}
