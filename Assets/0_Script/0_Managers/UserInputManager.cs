using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public delegate void DelegateInputAction();
public delegate void DelegateInputPress(bool inputValue);
public delegate void DelegateInputFlot(float inputValue);
public delegate void DelegateInput2D(Vector2 inputValue);


public partial class UserInputManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    public static event DelegateInput2D OnMoveInput;
    public static event DelegateInput2D OnLookInput;

    public static event DelegateInputAction OnJumpInput;
    public static event DelegateInputAction OnInteractionInput;
    public static event DelegateInputAction OnInventoryInput;
    public static event DelegateInputAction OnReloadInput;
    public static event DelegateInputAction OnFlashLightInput;
    public static event DelegateInputAction OnChangeModeInput;
    public static event DelegateInputAction OnCancelInput;
    public static event DelegateInputAction OnConfirmInput;
    public static event DelegateInputAction OnMenuInput;
    public static event DelegateInputAction OnBackInput;
    public static event DelegateInputAction OnWeapon0Input;
    public static event DelegateInputAction OnWeapon1Input;
    public static event DelegateInputAction OnWeapon2Input;
    public static event DelegateInputAction OnWeapon3Input;

    public static event DelegateInputFlot OnChangeWeaponInput;

    public static event DelegateInputPress OnRunInput;
    public static event DelegateInputPress OnAttackInput;
    public static event DelegateInputPress OnCrouchInput;
    public static event DelegateInputPress OnZoomInput;


}

public partial class UserInputManager : MonoBehaviour, IManagerBase
{
    public IEnumerator Initialize()
    {
        yield break;
    }
    public void Exit()
    {

    }
}

public partial class UserInputManager : MonoBehaviour, IManagerBase
{
    // move , look, jump, iteraction,inventory, changeMode, reload, flashLight, cancel, back,
    // confirm, menu, weapon0~3, changeWeapon, run, attack, crouch, zoom
    private void OnMove(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>();
        OnMoveInput?.Invoke(inputValue);
    }
    private void OnLook(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>();
        OnLookInput?.Invoke(inputValue);
    }
    private void OnJump()
    {
        OnJumpInput?.Invoke();
    }
    private void OnInventory()
    {
        OnInventoryInput?.Invoke();
    }
    private void OnInteraction()
    {
        OnInteractionInput?.Invoke();
    }
    private void OnChangeMode()
    {
        OnChangeModeInput?.Invoke();
    }
    private void OnReload()
    {
        OnReloadInput?.Invoke();
    }
    private void OnFlashLight()
    {
        OnFlashLightInput?.Invoke();
    }
    private void OnCancel()
    {
        OnCancelInput?.Invoke();
    }
    private void OnBack()
    {
        OnBackInput?.Invoke();
    }
    private void OnMenu()
    {
        OnMenuInput?.Invoke();
    }
    private void OnConfirm()
    {
        OnConfirmInput?.Invoke();
    }
    private void OnWeapon0()
    {
        OnWeapon0Input?.Invoke();
    }
    private void OnWeapon1()
    {
        OnWeapon1Input?.Invoke();
    }
    private void OnWeapon2()
    {
        OnWeapon2Input?.Invoke();
    }
    private void OnWeapon3()
    {
        OnWeapon3Input?.Invoke();
    }
    private void OnChangeWeapon(InputValue value)
    {
        float inputValue = value.Get<float>();
        OnChangeWeaponInput?.Invoke(inputValue);
    }
    private void OnRun(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnRunInput?.Invoke(isPressed);
    }
    private void OnAttack(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnAttackInput?.Invoke(isPressed);
    }
    private void OnCrouch(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnCrouchInput?.Invoke(isPressed);
    }
    private void OnZoom(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnZoomInput?.Invoke(isPressed);
    }
}

