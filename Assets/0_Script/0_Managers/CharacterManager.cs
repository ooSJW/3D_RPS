using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void ProcessControllerQueue(Queue<CharacterBase> targetQueue);

public partial class CharacterManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }

    public static event ProcessControllerQueue OnProcessControllerQueue;
    private static Queue<CharacterBase> controllerLessCharacterQueue = new();

    // reFresh�� �ʿ��Ҷ� ����� ����
    static bool isDirty = false;

}
public partial class CharacterManager // Initialize
{
    public IEnumerator Initialize()
    {
        GameManager.OnInitialUpdate -= OnInitialUpdate;
        GameManager.OnInitialUpdate += OnInitialUpdate;
        CreateCharacter(WorldManager.GetPlayerCharacterType(), "PlayerSpawnArea");

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
        /* dict�� list���� contains�� �ִµ� queue�� stack�� �߰� �� Ž���� ���ɻ� �Ҹ���,
         �ٵ� ������ Iterator�� ��ȸ�ϸ� �ڷ� ������ ������� contains�� ������ �Ȱ���?
         Iterator�� ��ȸ�ϴ°� �ᱹ foreach�� �Ȱ���? linQ�� ���� �������(?)�װɷ� ���Ƽ� �ٸ�����

         �亯 : linq�� iterator�� ���� ������ �����, �ᱹ iterator�� �������� �����ϰ�, ��� ��� ��ȸ�� ������ ���ɿ� �Ҹ��ϸ�
         ��ȸ�� �ڷᱸ���� ū ���̰� ����
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

    public static CharacterBase CreateCharacter(CharacterType wantType, string spawnAreaName)
    {
        Transform spawnAreaTransform = WorldManager.GetRandomSpawnArea(spawnAreaName).transform;
        return CreateCharacter(wantType, spawnAreaTransform.position, spawnAreaTransform.rotation, Vector3.one);
    }
}