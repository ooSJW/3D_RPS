using UnityEngine;

public partial class CharacterFullBodyMudule : CharacterMeshModule
{
    [SerializeField] private Transform lookTarget;
    private Transform cameraOffset;
}


public partial class CharacterFullBodyMudule : CharacterMeshModule
{
    public override void Attach(CharacterBase target)
    {
        base.Attach(target);
        cameraOffset = target?.GetSocket(SocketType.Eye)?.transform;
    }

    protected override void ForwardUpdate(float deltatTime)
    {
        if (Owner is null) return;

        // 바라보는 방향의 수평 방향 회전만 적용
        transform.forward = forward.HorizontalNormalize();
        lookTarget.position = (Owner.Forward * 10) + (cameraOffset?.position ?? Owner.transform.position);
    }
}
