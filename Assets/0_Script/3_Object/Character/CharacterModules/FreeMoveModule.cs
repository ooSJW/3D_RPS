using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial class FreeMoveModule : CharacterModuleBase // override
{
    private Vector3 moveDirection;
}

public partial class FreeMoveModule : CharacterModuleBase // override
{
    public override void Initialize()
    {
        base.Initialize();
        Owner.OnMove -= OnMove;
        Owner.OnMove += OnMove;
    }

    public override void Dettach()
    {
        base.Dettach();
        Owner.OnMove -= OnMove;
    }
}

public partial class FreeMoveModule : CharacterModuleBase // Property
{
    private void OnMove(Vector3 direction)
    {
        moveDirection = direction;
    }
}

public partial class FreeMoveModule : CharacterModuleBase // 
{
    private void FixedUpdate()
    {
        transform.Translate(moveDirection * Time.fixedDeltaTime * Owner.walkSpeedBase);
    }
}
