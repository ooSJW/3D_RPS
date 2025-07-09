using System;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : ControllerBase, IPoolable // Data Field
{
    public Queue<GameObject> RootQueue { get ; set ; }

}

public partial class AIController// Initialize
{

    public override CharacterBase Possess(CharacterBase target)
    {
        base.Possess(target);
        if (ControlCharacterBase is not null)
        {
            GameManager.OnControllerUpdate -= ControllerUpdate;
            GameManager.OnControllerUpdate += ControllerUpdate;
        }
        return ControlCharacterBase;
    }

    public override void UnPossess(ControllerBase causedBy = null)
    {
        base.UnPossess(causedBy);
        GameManager.OnControllerUpdate -= ControllerUpdate;
    }

    public override void OnCharacterDie()
    {
        base.OnCharacterDie();
        PoolManager.ClaimDeSpawn(gameObject);
    }
}
public partial class AIController  // Property
{
    private void ControllerUpdate(float deltaTime)
    {
        CharacterBase player = CharacterManager.GetPlayerCharacter();

        if (player is not null)
        {
            Vector3 direction = player.transform.position - ControlCharacterBase.transform.position;

            //ControlCharacterBase.Move(direction);
            ControlCharacterBase.SetRotation(direction);
        }
    }

    public void Initialize()
    {

    }

    public void Return2Pool()
    {
        UnPossess(this);
    }

}