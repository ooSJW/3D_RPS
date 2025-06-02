using UnityEngine;



public partial class PossessableController : ControllerBase // Data Field
{
    private ControllerBase targetOriginController;
}


public partial class PossessableController : ControllerBase // Initialize
{
    public override CharacterBase Possess(CharacterBase target)
    {
        if (target is null || ControlCharacterBase == target) return ControlCharacterBase;

        if (target.Controller is not null && target.Controller != this)
        {
            targetOriginController = target.Controller;
            targetOriginController.UnPossess(this);
        }

        if (ControlCharacterBase is not null) UnPossess();

        target.PossessedBy(this);
        ControlCharacterBase = target;
        return ControlCharacterBase;
    }

    public override void UnPossess(ControllerBase causedBy = null)
    {
        if (ControlCharacterBase is null)
            ControlCharacterBase.UnPossessed();

        if (causedBy is not null)
            ReturnOriginController();

        ControlCharacterBase = null;
    }
}

public partial class PossessableController : ControllerBase // 
{
    public override void OnCharacterDie()
    {
        base.OnCharacterDie();
        targetOriginController?.OnCharacterDie();
    }

    public virtual void ReturnOriginController()
    {
        if (targetOriginController is not null)
        {
            targetOriginController.Possess(ControlCharacterBase);
            targetOriginController = null;
        }
    }
}
