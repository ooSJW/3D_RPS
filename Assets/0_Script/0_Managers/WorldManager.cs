using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class WorldManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    private Dictionary<string, PlayerSpawnArea[]> playerSpawnAreaDict = new();
    private PoolManager poolManager;

    [SerializeField] private CharacterType playerCharacter;
    [SerializeField] private ControllerType playerController;
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        playerSpawnAreaDict.AddComponents(GameObject.FindGameObjectsWithTag("Respawn"));
        poolManager = gameObject.AddComponent<PoolManager>();
        yield return poolManager.Initialize();

        yield break;
    }
    public void Exit()
    {

    }
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Property
{
    public PlayerSpawnArea[] GetSpawnAreaArray(string key)
    {
        if (playerSpawnAreaDict.TryGetValue(key, out PlayerSpawnArea[] result))
            return result;

        return null;
    }


    public PlayerSpawnArea GetSpawnArea(string key, int index = 0)
    {
        if (playerSpawnAreaDict.TryGetValue(key, out PlayerSpawnArea[] array))
        {
            return array.Get(index);
        }

        return null;
    }


    public PlayerSpawnArea GetRandomSpawnArea(string key)
    {
        PlayerSpawnArea[] spawnArray = GetSpawnAreaArray(key);

        if (spawnArray is null || spawnArray.Length == 0)
            return null;
        if (spawnArray.Length == 1)
            return spawnArray[0];

        return spawnArray[Random.Range(0, spawnArray.Length)];
    }
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Main
{
#if UNITY_EDITOR
    private void Awake()
    {
        if (GameManager.Instance is null)
        {
            // ���� ������ ���� : ĳ���� ���� ��� �⺻ ĳ���� ���� ����ó���� �ʿ���
            // �������� ��η� ��ȸ : ����ó���� �ʿ������ , ����� �ð��� ����� �� ����.
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Additive);

        }
    }
#endif
}
