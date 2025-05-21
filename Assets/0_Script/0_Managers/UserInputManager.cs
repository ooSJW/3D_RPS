using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class UserInputManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }
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
        Debug.Log("Move Input");
    }
    private void OnLook(InputValue value)
    {
        Debug.Log("Look Input");
    }
    private void OnJump()
    {
        Debug.Log("Jump Input");
    }
    private void OnInventory()
    {
        Debug.Log("Invectory Input");
    }
    private void OnInteraction()
    {
        Debug.Log("Interaction Input");
    }
    private void OnChangeMode()
    {
        Debug.Log("CahngeMode Input");
    }
    private void OnReload()
    {
        Debug.Log("Reload Input");
    }
    private void OnFlashLight()
    {
        Debug.Log("FlashLigh Input");
    }
    private void OnCancel()
    {
        Debug.Log("Cancel Input");
    }
    private void OnBack()
    {
        Debug.Log("Back Input");
    }
    private void OnMenu()
    {
        Debug.Log("Menu Input");
    }
    private void OnConfirm()
    {
        Debug.Log("Confirm Input");
    }
    private void OnWeapon0()
    {
        Debug.Log("Weapon0 Input");
    }
    private void OnWeapon1()
    {
        Debug.Log("Weapon1 Input");
    }
    private void OnWeapon2()
    {
        Debug.Log("Weapon2 Input");
    }
    private void OnWeapon3()
    {
        Debug.Log("Weapon3 Input");
    }
    private void OnChangeWeapon(InputValue value)
    {
        Debug.Log("ChangeWeapon Input");
    }
    private void OnRun(InputValue value)
    {
        Debug.Log("Run Input");
    }
    private void OnAttack(InputValue value)
    {
        Debug.Log("Attack Input");
    }
    private void OnCrouch(InputValue value)
    {
        Debug.Log("Crouch Input");
    }
    private void OnZoom(InputValue value)
    {
        Debug.Log("Zoom Input");
    }
}
