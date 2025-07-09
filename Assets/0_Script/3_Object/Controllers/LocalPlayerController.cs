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
            UserInputManager.OnAttackInput -= OnAttackInput;
            UserInputManager.OnAttackInput += OnAttackInput;
            UserInputManager.OnReloadInput -= OnReloadInput;
            UserInputManager.OnReloadInput += OnReloadInput;

            UserInputManager.OnWeapon0Input -= OnWeapon0Input;
            UserInputManager.OnWeapon0Input += OnWeapon0Input;
            UserInputManager.OnWeapon1Input -= OnWeapon1Input;
            UserInputManager.OnWeapon1Input += OnWeapon1Input;
            UserInputManager.OnWeapon2Input -= OnWeapon2Input;
            UserInputManager.OnWeapon2Input += OnWeapon2Input;
            UserInputManager.OnWeapon3Input += OnWeapon3Input;
            UserInputManager.OnWeapon3Input += OnWeapon3Input;
            UserInputManager.OnChangeWeaponInput -= OnChangeWeaponInput;
            UserInputManager.OnChangeWeaponInput += OnChangeWeaponInput;




            UserInputManager.OnMenuInput += () => GameManager.SetMouseLock(!GameManager.GetMouseLock());
        }

        return ControlCharacterBase;
    }

    public override void UnPossess(ControllerBase causedBy = null)
    {
        base.UnPossess(causedBy);
        UserInputManager.OnMoveInput -= OnMoveInput;
        UserInputManager.OnReloadInput -= OnReloadInput;
    }

    public override void OnCharacterDie()
    {
        base.OnCharacterDie();
        UserInputManager.OnMoveInput -= OnMoveInput;
        UserInputManager.OnReloadInput -= OnReloadInput;
    }
}

public partial class LocalPlayerController : PlayerController
{
    public void OnMoveInput(Vector2 input)
    {
        //Vector3 moveDirection = (input.y * Vector3.forward) + (input.x * Vector3.right);

        Vector3 moveDirection = (input.y * ControlCharacterBase.Forward) + (input.x * ControlCharacterBase.Right);

        ControlCharacterBase.Move(moveDirection);
    }
    private void OnLookInput(Vector2 inputValue)
    {
        // 회전 : Roll Pitch Yaw
        // Roll : 갸웃 fps의 qe
        // Pitch : 끄덕끄덕
        // Yaw : 도리도리
        // 마우스 좌우(x) : yaw
        // 마우스 상하(y) : pitch
        // -는 움직이는 주체에 따라 -하거나 그대로 전달.
        // * 감도
        inputValue *= 0.5f;
        ControlCharacterBase.AddRotation(-inputValue.x, inputValue.y);
    }

    // CharacterBase.Attack에서 OnAttack delegate 호출
    public void OnAttackInput(bool value) => ControlCharacterBase.Attack(ControlCharacterBase.Forward, value);
    public void OnReloadInput() => ControlCharacterBase.Reload();
    public void OnWeapon0Input() => ControlCharacterBase.Weapon(0);
    public void OnWeapon1Input() => ControlCharacterBase.Weapon(1);
    public void OnWeapon2Input() => ControlCharacterBase.Weapon(2);
    public void OnWeapon3Input() => ControlCharacterBase.Weapon(3);
    public void OnChangeWeaponInput(float value) => ControlCharacterBase.ChangeWeapon(value);
}