using System;
using UnityEngine;




public partial class LocalPlayerController : PlayerController // Initialize
{
    public override CharacterBase Possess(CharacterBase target)
    {
        base.Possess(target);

        if (ControlCharacterBase is not null)
        {
            UserInputManager.OnMoveInput -= OnMoveInput;
            UserInputManager.OnMoveInput += OnMoveInput;
            UserInputManager.OnLookInput -= OnLookInput;
            UserInputManager.OnLookInput += OnLookInput;
        }

        return ControlCharacterBase;
    }

    public override void UnPossess(ControllerBase causedBy = null)
    {
        base.UnPossess(causedBy);
        UserInputManager.OnMoveInput -= OnMoveInput;
    }

    public override void OnCharacterDie()
    {
        base.OnCharacterDie();
        UserInputManager.OnMoveInput -= OnMoveInput;
    }
}

public partial class LocalPlayerController : PlayerController
{
    public void OnMoveInput(Vector2 input)
    {
        Vector3 moveDirection = (input.y * ControlCharacterBase.Forward) + (input.x * ControlCharacterBase.Right);

        ControlCharacterBase.Move(moveDirection);
    }
    private void OnLookInput(Vector2 inputValue)
    {
        // Roll Pitch Yaw
        // Roll : °¼¿ô fpsÀÇ qe
        // Pitch : ²ô´ö²ô´ö
        // Yaw : µµ¸®µµ¸®
        // ¸¶¿ì½º ÁÂ¿ì(x) : yaw
        // ¸¶¿ì½º »óÇÏ(y) : pitch
        ControlCharacterBase.AddRotation(inputValue.x, inputValue.y);
    }
}