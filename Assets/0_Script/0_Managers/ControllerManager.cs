using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ControllerManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

}
public partial class ControllerManager // Initialize
{
    public IEnumerator Initialize()
    {
        CharacterManager.OnProcessControllerQueue -= ProcessControllerQueue;
        CharacterManager.OnProcessControllerQueue += ProcessControllerQueue;

        CreateController(WorldManager.GetPlayerControllerType());
        yield return null;
    }

    public void Exit()
    {
        CharacterManager.OnProcessControllerQueue -= ProcessControllerQueue;
    }
}
public partial class ControllerManager // Property
{
    private void ProcessControllerQueue(Queue<CharacterBase> targetQueue)
    {
        if (targetQueue is null) return;

        while (targetQueue.TryDequeue(out CharacterBase character))
        {
            ImbuingController(character);
        }
    }

    public void ImbuingController(CharacterBase targetCharacter)
    {
        if (targetCharacter is null) return;

        ControllerBase controller = CreateController(targetCharacter.BaseControllerType);

        if (controller is null) return;

        controller.Possess(targetCharacter);
    }

    public ControllerBase CreateController(ControllerType wantType)
    {
        GameObject instance = PoolManager.ClaimSpawn(wantType.ToString());

        if (instance is null)
        {
            GameObject prefab = FileManager.GetControllerPrefab(wantType);
            if (prefab is null) return null;

            instance = Instantiate(prefab);
        }

        if (instance.TryGetComponent(out ControllerBase controller))
            return controller;

        return null;
    }
}