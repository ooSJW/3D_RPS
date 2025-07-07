using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public delegate void DelegateReloadComplete();

[RequireComponent(typeof(Animator))]
public partial class CharacterMeshModule : CharacterModuleBase // Property
{
    public event DelegateReloadComplete OnReloadComplete;

    [SerializeField] protected float rotateSpeed = 10;
    protected Vector3 forward;

    [SerializeField] private bool localPlayerVisible;
    [SerializeField] private bool otherPlayerVisible;

    protected Animator animator;
}


public partial class CharacterMeshModule : CharacterModuleBase // Property
{
    public override void Attach(CharacterBase target)
    {
        base.Attach(target);
        animator = GetComponent<Animator>();

        if (Owner is not null)
        {
            if (Owner.Controller is not null) OwnerChanged(Owner.Controller);

            Owner.OnOwnerChanged -= OwnerChanged;
            Owner.OnOwnerChanged += OwnerChanged;
            Owner.OnAim -= ForwardChanged;
            Owner.OnAim += ForwardChanged;
            Owner.OnAnimationTrigger -= animator.SetTrigger;
            Owner.OnAnimationTrigger += animator.SetTrigger;
            GameManager.OnCharacterUpdate -= ForwardUpdate;
            GameManager.OnCharacterUpdate += ForwardUpdate;
        }
    }

    public override void Dettach()
    {
        if (Owner is not null)
        {
            Owner.OnOwnerChanged -= OwnerChanged;
            Owner.OnAim -= ForwardChanged;
            Owner.OnAnimationTrigger -= animator.SetTrigger;
            GameManager.OnCharacterUpdate -= ForwardUpdate;
        }

        base.Dettach();
    }

    protected virtual void ForwardUpdate(float deltatTime)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), deltatTime * rotateSpeed);
    }

    private void ForwardChanged(Vector3 wantForward)
    {
        forward = wantForward;
    }

    private void OwnerChanged(ControllerBase newController)
    {
        bool isLocalPlayer = newController as LocalPlayerController;
        bool isVisible = localPlayerVisible && isLocalPlayer;
        isVisible |= otherPlayerVisible && !isLocalPlayer;

        gameObject.SetActive(isVisible);
    }

    public virtual void NoticeReloadComplete()
    {
        OnReloadComplete?.Invoke();
        Debug.Log("¿Â¿¸");
    }

}
