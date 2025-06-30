using Unity.VisualScripting;
using UnityEngine;



public partial class CharacterAttackModule : CharacterModuleBase
{
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private AnimationCurve trajectoryCurve;
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

    public virtual void OnAttack(Vector3 direction, bool isDown)
    {
        Owner?.SocketActionByType(SocketType.Muzzle, TriggerAttackAtSocket);
    }
    public virtual void TriggerAttackAtSocket(SocketBase targetSocket)
    {
        Ray ray = new(targetSocket.transform.position, targetSocket.transform.forward);
        if (ray.CurveCastWithDebug(out RaycastHit hit, 20.0f, attackMask, trajectoryCurve, 8, 1.0f))
        {
            Debug.Log(hit.collider.name);
        }
    }

}
