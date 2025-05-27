using UnityEngine;

public interface IUserInterfaceBase
{
    public bool IsOpen { get; }
    public bool Open(bool value);

    public bool Toggle();
    public bool Active(bool value);

    public void Initialize(UIManager uiManager);
    public void Exit();
}
