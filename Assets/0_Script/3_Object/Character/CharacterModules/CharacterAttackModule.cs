using UnityEngine;



public partial class CharacterAttackModule : CharacterModuleBase
{

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
        }
    }

    public override void Dettach()
    {
        base.Dettach();
        if (Owner is not null)
        {
            Owner.OnAttack -= OnAttack;
        }
    }

    public void OnAttack(Vector3 direction, bool isDown)
    {
        Owner?.SocketActionByType(SocketType.Muzzle, (socket) => { Debug.Log($"{socket.name}: Attack"); });
    }
}
