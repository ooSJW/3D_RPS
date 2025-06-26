using System;
using UnityEngine;

public partial class AIController : ControllerBase // Data Field
{

}

public partial class AIController : ControllerBase // Initialize
{
    public override CharacterBase Possess(CharacterBase target)
    {
        base.Possess(target);
        if (ControlCharacterBase is not null)
        {
            GameManager.OnControllerUpdate -= ControllerUpdate;
            GameManager.OnControllerUpdate += ControllerUpdate;
        }
        return ControlCharacterBase;
    }

    public override void UnPossess(ControllerBase causedBy = null)
    {
        base.UnPossess(causedBy);
        GameManager.OnControllerUpdate -= ControllerUpdate;
    }
}
public partial class AIController : ControllerBase // Property
{
    private void ControllerUpdate(float deltaTime)
    {
        CharacterBase player = CharacterManager.GetPlayerCharacter();

        if (player is not null)
        {
            Vector3 direction = player.transform.position - ControlCharacterBase.transform.position;

            //ControlCharacterBase.Move(direction);
            ControlCharacterBase.SetRotation(direction);
        }
    }
}