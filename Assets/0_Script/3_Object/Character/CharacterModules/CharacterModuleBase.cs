using UnityEngine;


public abstract partial class CharacterModuleBase : MonoBehaviour // Data Field
{
    public CharacterBase Owner { get; protected set; }
}

public abstract partial class CharacterModuleBase // Initialize
{
    public virtual void Initialize()
    {

    }
    public virtual void Attach(CharacterBase target)
    {
        if (target is null || target == Owner) return;
        if (Owner is not null) Dettach();

        Owner = target;
        Initialize();
    }
    public virtual void Dettach()
    {
        Owner = null;
    }
}

