using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate CharacterType DelegateGetPlayerCharacterType();
public delegate ControllerType DelegateGetPlayerControllerType();

public delegate PlayerSpawnArea DelegateGetRandomSpawnArea(string key);
public delegate PlayerSpawnArea DelegateGetSpawnAreaByIndex(string key, int index);
public delegate PlayerSpawnArea[] DelegateGetSpawnAreaAray(string key);

public partial class WorldManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    protected static DelegateGetPlayerCharacterType ClaimPlayerCharacterType;
    protected static DelegateGetPlayerControllerType ClaimPlayerControllerType;

    protected static DelegateGetRandomSpawnArea ClaimRandomSpawnArea;
    protected static DelegateGetSpawnAreaByIndex ClaimSpawnAreaByIndex;
    protected static DelegateGetSpawnAreaAray ClaimSpawnAreaArray;


    private Dictionary<string, PlayerSpawnArea[]> playerSpawnAreaDict = new();
    private PoolManager poolManager;
    private CharacterManager characterManager;
    private ControllerManager controllerManager;

    [SerializeField] private CharacterType playerCharacter;
    [SerializeField] private ControllerType playerController;
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        ClaimPlayerCharacterType = () => playerCharacter;
        ClaimPlayerControllerType = () => playerController;

        ClaimRandomSpawnArea -= FindRandomSpawnArea;
        ClaimRandomSpawnArea += FindRandomSpawnArea;

        ClaimSpawnAreaByIndex -= FindSpawnArea;
        ClaimSpawnAreaByIndex += FindSpawnArea;

        ClaimSpawnAreaArray -= FindSpawnAreaArray;
        ClaimSpawnAreaArray += FindSpawnAreaArray;



        playerSpawnAreaDict.AddComponents(GameObject.FindGameObjectsWithTag("Respawn"));
        poolManager = gameObject.GetOrAddComponent<PoolManager>();
        yield return poolManager.Initialize();

        characterManager = gameObject.GetOrAddComponent<CharacterManager>();
        yield return characterManager.Initialize();

        controllerManager = gameObject.GetOrAddComponent<ControllerManager>();
        yield return controllerManager.Initialize();


        yield break;
    }

    public void Exit()
    {
        ClaimPlayerCharacterType = null;
        ClaimPlayerControllerType = null;
        ClaimRandomSpawnArea -= FindRandomSpawnArea;
        ClaimSpawnAreaByIndex -= FindSpawnArea;
        ClaimSpawnAreaArray -= FindSpawnAreaArray;

    }
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Property
{
    private PlayerSpawnArea[] FindSpawnAreaArray(string key)
    {
        if (playerSpawnAreaDict.TryGetValue(key, out PlayerSpawnArea[] result))
            return result;

        return null;
    }


    private PlayerSpawnArea FindSpawnArea(string key, int index = 0)
    {
        if (playerSpawnAreaDict.TryGetValue(key, out PlayerSpawnArea[] array))
        {
            return array.Get(index);
        }

        return null;
    }


    private PlayerSpawnArea FindRandomSpawnArea(string key)
    {
        PlayerSpawnArea[] spawnArray = FindSpawnAreaArray(key);
        if (spawnArray is null || spawnArray.Length == 0)
            return null;
        if (spawnArray.Length == 1)
            return spawnArray[0];

        return spawnArray[Random.Range(0, spawnArray.Length)];
    }

    public static CharacterType GetPlayerCharacterType() => ClaimPlayerCharacterType?.Invoke() ?? CharacterType.CharacterBase;
    public static ControllerType GetPlayerControllerType() => ClaimPlayerControllerType?.Invoke() ?? ControllerType.ControllerBase;

    public static PlayerSpawnArea GetRandomSpawnArea(string key) => ClaimRandomSpawnArea?.Invoke(key);
    public static PlayerSpawnArea GetSpawnAreaByIndex(string key, int index) => ClaimSpawnAreaByIndex?.Invoke(key, index);
    public static PlayerSpawnArea[] GetSpawnAreaArray(string key) => ClaimSpawnAreaArray?.Invoke(key);

}

public partial class WorldManager : MonoBehaviour, IManagerBase // Main
{
#if UNITY_EDITOR
    private void Awake()
    {
        if (GameManager.Instance is null)
        {
            // 현재 씬에서 시작 : 캐선이 있을 경우 기본 캐릭터 등의 예외처리가 필요함
            // 정상적인 경로로 우회 : 예외처리는 필요없지만 , 디버깅 시간이 길어질 수 있음.
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Additive);

        }
    }
#endif
}
