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

    private bool isVisible = false;
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
            Owner.OnSpeedChanged -= SpeedChanged;
            Owner.OnSpeedChanged += SpeedChanged;
            Owner.OnAnimationTrigger -= animator.SetTrigger;
            Owner.OnAnimationTrigger += animator.SetTrigger;
            OnReloadComplete -= Owner.ReloadComplete;
            OnReloadComplete += Owner.ReloadComplete;
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
            Owner.OnSpeedChanged -= SpeedChanged;
            Owner.OnAnimationTrigger -= animator.SetTrigger;
            OnReloadComplete -= Owner.ReloadComplete;
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

    private void SpeedChanged(Vector3 newVelocity)
    {
        animator.SetFloat("MovementSpeed", newVelocity.magnitude);
        animator.SetFloat("MovementForward", newVelocity.z);
        animator.SetFloat("MovementRight", newVelocity.x);
    }

    private void OwnerChanged(ControllerBase newController)
    {
        bool isLocalPlayer = newController as LocalPlayerController;
        isVisible = localPlayerVisible && isLocalPlayer;
        isVisible |= otherPlayerVisible && !isLocalPlayer;
        foreach (Renderer currentRenderer in GetComponentsInChildren<Renderer>())
        {
            currentRenderer.enabled = isVisible;
        }
        foreach (SocketBase currentSocket in GetComponentsInChildren<SocketBase>())
        {
            currentSocket.enabled = isVisible;
        }
        //gameObject.SetActive(isVisible);
    }

    public virtual void NoticeReloadComplete()
    {
        OnReloadComplete?.Invoke();
    }

    public virtual void OnDrawStart() { if (isVisible) Debug.Log(" OnDrawStart "); }
    public virtual void OnDrawEnd() { if (isVisible) Debug.Log(" OnDrawEnd "); }

    public virtual void OnHolsteringStart() { if (isVisible) Debug.Log(" OnHolsteringStart "); }
    public virtual void OnHolsteringEnd() { if (isVisible) Debug.Log(" OnHolsteringEnd "); }
}
