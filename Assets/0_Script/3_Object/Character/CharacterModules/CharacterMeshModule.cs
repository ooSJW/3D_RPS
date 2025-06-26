using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public partial class CharacterMeshModule : CharacterModuleBase // Property
{
    [SerializeField] protected float rotateSpeed = 10;
    protected Vector3 forward;

    [SerializeField] private bool localPlayerVisible;
    [SerializeField] private bool otherPlayerVisible;
}


public partial class CharacterMeshModule : CharacterModuleBase // Property
{
    public override void Attach(CharacterBase target)
    {
        base.Attach(target);

        if (Owner is not null)
        {
            if (Owner.Controller is not null) OwnerChanged(Owner.Controller);

            Owner.OnOwnerChanged -= OwnerChanged;
            Owner.OnOwnerChanged += OwnerChanged;
            Owner.OnAim -= ForwardChanged;
            Owner.OnAim += ForwardChanged;
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
}
