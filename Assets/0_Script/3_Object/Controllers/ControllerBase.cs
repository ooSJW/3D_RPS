using UnityEngine;

public delegate void DelegatePossess(CharacterBase target);
public delegate void DelegatePossessWithID(CharacterBase target, int id);

public delegate void DelegateUnPossess(CharacterBase target);
public delegate void DelegateUnPossessWithID(CharacterBase target, int id);

public partial class ControllerBase : MonoBehaviour // Data Field
{
    public CharacterBase ControlCharacterBase { get; protected set; }

    // 컨트롤러의 식별 코드
    public int controllerId;

    public event DelegatePossess OnPossess;
    public event DelegatePossessWithID OnPossessWithID;

    public event DelegateUnPossess OnUnPossess;
    public event DelegateUnPossessWithID OnUnPossessWithID;
}

public partial class ControllerBase : MonoBehaviour // Initialize
{
    // possess : 빙의 -> controller는 오브젝트 없이 캐릭터 오브젝트를 움직임.
    public virtual CharacterBase Possess(CharacterBase target)
    {
        if (target is null || ControlCharacterBase == target ||
            (target.Controller is not null && target.Controller != this))
            return ControlCharacterBase;

        if (ControlCharacterBase is not null) UnPossess();

        target.PossessedBy(this);
        ControlCharacterBase = target;

        OnPossess?.Invoke(ControlCharacterBase);
        OnPossessWithID?.Invoke(ControlCharacterBase, controllerId);

        return ControlCharacterBase;
    }

    public virtual void UnPossess(ControllerBase causedBy = null)
    {
        if (ControlCharacterBase is not null)
        {
            OnUnPossess?.Invoke(ControlCharacterBase);
            OnUnPossessWithID?.Invoke(ControlCharacterBase, controllerId);
        }

        ControlCharacterBase?.UnPossessed();
        ControlCharacterBase = null;
    }
}

public partial class ControllerBase : MonoBehaviour // 
{
    public virtual void OnCharacterDie()
    {

    }
}
