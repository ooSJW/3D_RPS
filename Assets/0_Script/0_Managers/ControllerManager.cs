using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateLocalControllerPossessed(CharacterBase target);
public delegate void DelegateOtherControllerPossessed(CharacterBase target, int index);

public partial class ControllerManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    // 외부 입력의 종류 : 로컬, 네트워크
    // 카메라가 변경되어야하는 시점 : 로컬 컨트롤러가 어떤 오브젝트에 possess되었을 때
    public static event DelegateLocalControllerPossessed OnLocalControllerPossessed;
    public static event DelegateOtherControllerPossessed OnOtherControllerPossessed;



    private static ControllerBase[] playerControllerArray;
}
public partial class ControllerManager // Initialize
{
    public IEnumerator Initialize()
    {
        CharacterManager.OnProcessControllerQueue -= ProcessControllerQueue;
        CharacterManager.OnProcessControllerQueue += ProcessControllerQueue;

        SetPlayerControllers(CharacterManager.GetPlayerCharacterArray());

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

    public void SetPlayerControllers(CharacterBase[] characterArray)
    {
        if (characterArray == null) return;

        int wantLength = characterArray.Length;
        int originalLength;

        if (playerControllerArray is null)
        {
            originalLength = wantLength;
            playerControllerArray = new ControllerBase[wantLength];
        }
        else
        {
            originalLength = playerControllerArray.Length;
            if (originalLength < wantLength) Array.Resize(ref playerControllerArray, wantLength);
        }

        for (int i = wantLength; i < originalLength; i++)
        {
            ControllerBase currentController = playerControllerArray[i];
            if (currentController is null) continue;
            currentController.OnPossess -= LocalPlayerPossessed;
            currentController.OnPossessWithID -= OtherPlayerPossessed;
            playerControllerArray[i] = null;
        }

        for (int i = 0; i < wantLength; i++)
        {
            CharacterBase current = characterArray[i];

            if (current is null)
                playerControllerArray[i] = null;
            else
            {
                ControllerBase newController = current.Controller;

                if (newController is null)
                {
                    newController = CreateController(WorldManager.GetPlayerControllerType());
                    newController.Possess(current);
                }
                playerControllerArray[i] = newController;
                newController.controllerId = i;

                newController.OnPossess -= LocalPlayerPossessed;
                newController.OnPossessWithID -= OtherPlayerPossessed;

                // i == 0이라면 newControllerId == 0;  로컬 컨트롤러의 ID로 0을 사용하기로 결정했음.
                if (i == 0)
                {
                    newController.OnPossess += LocalPlayerPossessed;
                    LocalPlayerPossessed(newController.ControlCharacterBase);
                }
                else
                {
                    newController.OnPossessWithID += OtherPlayerPossessed;
                    OtherPlayerPossessed(newController.ControlCharacterBase, newController.controllerId);
                }
            }
        }
    }

    public static ControllerBase GetPlayerController(int index = 0)
    {
        playerControllerArray.TryGet(index, out ControllerBase controllerBase);
        return controllerBase;
    }

    public static ControllerBase[] GetPlayerControllerArray() => playerControllerArray;


    // 해당 함수를 public으로 열어뒀을 때 발생할 수 있는 문제
    // 해당 배열을 반복하며 값을 수정, 추가할 때 문제가 발생할 수 있음.
    public static void AddPlayerController(ControllerBase target)
    {
        if (playerControllerArray is null)
        {
            playerControllerArray = new[] { target };
            return;
        }

        int targetIndex = Array.FindIndex(playerControllerArray, current => current is null);

        if (targetIndex >= 0)
            playerControllerArray[targetIndex] = target;
        else
        {
            targetIndex = playerControllerArray.Length;

            Array.Resize(ref playerControllerArray, targetIndex + 1);
            playerControllerArray[targetIndex] = target;
        }
        target.controllerId = targetIndex;

        target.OnPossess -= LocalPlayerPossessed;
        target.OnPossessWithID -= OtherPlayerPossessed;

        if (targetIndex == 0)
        {
            target.OnPossess += LocalPlayerPossessed;
            LocalPlayerPossessed(target.ControlCharacterBase);
        }
        else
        {
            target.OnPossessWithID += OtherPlayerPossessed;
            OtherPlayerPossessed(target.ControlCharacterBase, target.controllerId);
        }

    }


    public static void RemovePlayerController(ControllerBase target)
    {
        if (playerControllerArray is null) return;
        // 배열 : 추가 / 제거가 어려움, 값 추가 시 길이를 늘리는게 불가피할 수 있지만, 제거 시에는 길이를 줄일필요는 없음.
        int targetIndex = Array.FindIndex(playerControllerArray, current => current == target);
        if (targetIndex >= 0) playerControllerArray[targetIndex] = null;

        target.OnPossess -= LocalPlayerPossessed;
        target.OnPossessWithID -= OtherPlayerPossessed;
    }


    public static void LocalPlayerPossessed(CharacterBase target) => OnLocalControllerPossessed?.Invoke(target);
    public static void OtherPlayerPossessed(CharacterBase target, int index) => OnOtherControllerPossessed?.Invoke(target, index);

}