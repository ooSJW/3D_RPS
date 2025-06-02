using UnityEngine;

public partial class ControllerBase : MonoBehaviour // Data Field
{
    public CharacterBase ControlCharacterBase { get; protected set; }
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
        return ControlCharacterBase;
    }

    public virtual void UnPossess(ControllerBase causedBy = null)
    {
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
