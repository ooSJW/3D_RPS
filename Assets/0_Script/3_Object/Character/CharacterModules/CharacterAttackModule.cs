using System;
using UnityEngine;


public partial class CharacterAttackModule : CharacterModuleBase
{
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private AnimationCurve trajectoryCurve;
    [SerializeField] private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon
    {
        get => currentWeapon;
        set
        {
            currentWeapon = value;
            if (Owner)
            {
                currentWeapon.OnShotAnimation += Owner.AnimationPlay;
                Owner.AnimationPlay(currentWeapon.GetWeaponAnimationType());
            }
        }
    }
}


public partial class CharacterAttackModule : CharacterModuleBase
{
    public void RefreshWeapon() => CurrentWeapon = CurrentWeapon;


    public override void Attach(CharacterBase target)
    {
        base.Attach(target);
        if (Owner is not null)
        {
            Owner.OnAttack -= OnAttack;
            Owner.OnAttack += OnAttack;
            Owner.OnReload -= OnReloadStart;
            Owner.OnReload += OnReloadStart;
            Owner.OnModuleLoaded -= RefreshWeapon;
            Owner.OnModuleLoaded += RefreshWeapon;
            GameManager.OnObjectUpdate -= OnUpdate;
            GameManager.OnObjectUpdate += OnUpdate;
        }
    }

    public override void Dettach()
    {
        base.Dettach();
        if (Owner is not null)
        {
            Owner.OnAttack -= OnAttack;
            Owner.OnReload -= OnReloadStart;
            Owner.OnModuleLoaded -= RefreshWeapon;
            GameManager.OnObjectUpdate -= OnUpdate;
        }
    }

    public virtual void OnUpdate(float deltaTime)
    {
        if (Owner && currentWeapon)
        {
            currentWeapon.TimeUpdate();
            Owner.SocketActionByType(SocketType.Muzzle, currentWeapon.ShotUpdate);
        }
    }

    public virtual void OnReloadStart()
    {
        if (Owner && currentWeapon)
        {
            string animationName = currentWeapon.ReloadStart(Owner.GetSpareAmmo(currentWeapon.RequireAmmo));
            Owner.AnimationPlay(animationName);
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
        if (Owner is not null && currentWeapon is not null)
        {
            Action<SocketBase> attackAction = isDown ? currentWeapon.ShotStart : currentWeapon.ShotEnd;
            currentWeapon.TimeUpdate();
            Owner.SocketActionByType(SocketType.Muzzle, attackAction);
        }
    }
}
