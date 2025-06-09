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
            =>
            ControlCharacterBase.Move(
            (input.y * ControlCharacterBase.Forward)
            +
            (input.x * ControlCharacterBase.Right));
}