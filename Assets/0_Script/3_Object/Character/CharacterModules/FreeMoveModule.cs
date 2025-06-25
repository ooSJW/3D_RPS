using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial class FreeMoveModule : CharacterModuleBase // Data Field
{
    // private Vector3 moveDirection;
}

public partial class FreeMoveModule : CharacterModuleBase // Initialize
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
    private void OnMove(CharacterBase mover, Vector3 velocity, float deltaTime)
    {
        // Translate : 기본적으로 로컬 좌표 기준으로 움직임.
        mover.transform.Translate(velocity * deltaTime, Space.World);
    }
}

public partial class FreeMoveModule : CharacterModuleBase // 
{
    //private void FixedUpdate()
    //{
    //    if (Owner is not null)
    //    {
    //        // 1. 월드 기준으로 입력을 받아, 이동
    //        // Vector3 moveResult = (moveDirection.z * Owner.Forward + moveDirection.x * Owner.Right).HorizontalNormalize();

    // 2. 로컬 기준으로 입력을 받아 이동
    //        Vector3 moveResult = moveDirection;
    //        transform.Translate(moveResult * Time.fixedDeltaTime * Owner.walkSpeedBase);
    //    }
    //}
}
