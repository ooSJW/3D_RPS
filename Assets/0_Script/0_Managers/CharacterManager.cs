using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public delegate void ProcessControllerQueue(Queue<CharacterBase> targetQueue);

public partial class CharacterManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    public static event ProcessControllerQueue OnProcessControllerQueue;
    private static Queue<CharacterBase> controllerLessCharacterQueue = new();

    private static CharacterBase[] playerCharacterArray;


    // reFresh가 필요할때 사용할 변수
    static bool isDirty = false;

}
public partial class CharacterManager // Initialize
{
    public IEnumerator Initialize()
    {
        GameManager.OnInitialUpdate -= OnInitialUpdate;
        GameManager.OnInitialUpdate += OnInitialUpdate;
        AddPlayerCharacter(CreateCharacter(WorldManager.GetPlayerCharacterType(), "PlayerSpawnArea"));

        yield return null;
    }

    public void Exit()
    {
        GameManager.OnInitialUpdate -= OnInitialUpdate;
    }

}

public partial class CharacterManager // Main
{
    public void OnInitialUpdate(float deltaTime)
    {
        if (isDirty) UpdateController();
    }
}

public partial class CharacterManager // Property
{

    public static CharacterBase CreateCharacter(CharacterType wantType, Vector3 wantPosition, Quaternion wantRotation, Vector3 wantSize)
    {
        GameObject instance = PoolManager.ClaimSpawn(wantType.ToString(), wantPosition, wantRotation, wantSize);
        if (instance is null)
        {
            GameObject prefab = FileManager.GetCharacterPrefab(wantType);
            if (prefab is null) return null;

            instance = Instantiate(prefab, wantPosition, wantRotation);
            instance.transform.localScale = wantSize;
        }

        if (instance.TryGetComponent(out CharacterBase asCharacter))
        {
            asCharacter.Initialize();
            return asCharacter;
        }

        Debug.LogError($"{instance.name} has not CharacterBase Component");
        return null;
    }


    private static void WaitForController(CharacterBase character)
    {
        /* dict나 list에도 contains가 있는데 queue나 stack은 중간 값 탐색이 성능상 불리함,
         근데 어차피 Iterator가 순회하면 자료 구조에 상관없이 contains의 성능은 똑같나?
         Iterator가 순회하는건 결국 foreach와 똑같나? linQ가 쓰는 쿼리언어(?)그걸로 돌아서 다르려나

         답변 : linq가 iterator를 통해 쿼리언어를 사용함, 결국 iterator는 쿼리언어로 동작하고, 모든 요소 순회는 어차피 성능에 불리하며
         순회는 자료구조에 큰 차이가 없다
        */
        controllerLessCharacterQueue.Enqueue(character);
        isDirty = true;
    }

    public static void UpdateController()
    {
        if (OnProcessControllerQueue is null) return;

        OnProcessControllerQueue.Invoke(controllerLessCharacterQueue);
        isDirty = false;
    }

    public static CharacterBase GetPlayerCharacter(int index = 0)
    {
        playerCharacterArray.TryGet(index, out CharacterBase characterBase);
        return characterBase;
    }

    public static CharacterBase[] GetPlayerCharacterArray() => playerCharacterArray;


    // 해당 함수를 public으로 열어뒀을 때 발생할 수 있는 문제
    // 해당 배열을 반복하며 값을 수정, 추가할 때 문제가 발생할 수 있음.
    public static void AddPlayerCharacter(CharacterBase target)
    {
        if (playerCharacterArray is null)
        {
            playerCharacterArray = new[] { target };
            return;
        }

        int targetIndex = Array.FindIndex(playerCharacterArray, current => current is null);

        if (targetIndex >= 0)
            playerCharacterArray[targetIndex] = target;
        else
        {
            Array.Resize(ref playerCharacterArray, playerCharacterArray.Length + 1);
            playerCharacterArray[^1] = target;
        }
    }


    public static void RemovePlayerCharacter(CharacterBase target)
    {
        if (playerCharacterArray is null) return;
        // 배열 : 추가 / 제거가 어려움, 값 추가 시 길이를 늘리는게 불가피할 수 있지만, 제거 시에는 길이를 줄일필요는 없음.
        int targetIndex = Array.FindIndex(playerCharacterArray, current => current == target);
        if (targetIndex >= 0) playerCharacterArray[targetIndex] = null;
    }


    public static CharacterBase CreateCharacter(CharacterType wantType, string spawnAreaName)
    {
        Transform spawnAreaTransform = WorldManager.GetRandomSpawnArea(spawnAreaName).transform;
        return CreateCharacter(wantType, spawnAreaTransform.position, spawnAreaTransform.rotation, Vector3.one);
    }
}