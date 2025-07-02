using System;
using UnityEngine;



public partial class CharacterAttackModule : CharacterModuleBase
{
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private AnimationCurve trajectoryCurve;
    [SerializeField] private WeaponBase currentWeapon;
}


public partial class CharacterAttackModule : CharacterModuleBase
{
    public override void Attach(CharacterBase target)
    {
        base.Attach(target);
        if (Owner is not null)
        {
            Owner.OnAttack -= OnAttack;
            Owner.OnAttack += OnAttack;
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
