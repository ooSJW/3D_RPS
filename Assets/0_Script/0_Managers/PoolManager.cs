using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[System.Serializable]
public struct PoolRequestCharacter { public CharacterType wantType; public int amount; }

[System.Serializable]
public struct PoolRequestController { public ControllerType wantType; public int amount; }


public delegate GameObject DelegateSpawn(string name, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, Space coord);
public delegate void DelegateDespawn(GameObject target);
public partial class PoolManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }
    // event Property
    public static event DelegateSpawn OnSpawn; //{ add => OnSpawn += value; remove => OnSpawn -= value; }
    public static event DelegateDespawn OnDespawn; //{ add => OnDespawn += value; remove => OnDespawn -= value; }

    [SerializeField] private PoolRequestCharacter[] requestCharacter = new PoolRequestCharacter[0];
    [SerializeField] private PoolRequestController[] requestController = new PoolRequestController[0];

    private Dictionary<string, Queue<GameObject>> poolableObjectDict = new();

    [SerializeField]
    private Transform rootTransform;
}

public partial class PoolManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        OnSpawn -= SpawnObject;
        OnSpawn += SpawnObject;
        OnDespawn -= Release2Pool;
        OnDespawn += Release2Pool;

        rootTransform = new GameObject("[Root Pool]").transform;

        foreach (var current in requestCharacter)
        {
            RegisterPoolObject
                (
                FileManager.GetCharacterPrefab(current.wantType),
                current.amount
                );
        }

        yield break;
    }
    public void Exit()
    {

    }

}

public partial class PoolManager : MonoBehaviour, IManagerBase // Property
{
    private GameObject RegisterPoolObject(GameObject prefab)
    {
        GameObject poolObject = Instantiate(prefab);
        if (poolObject is not null)
        {
            string name = prefab.name;
            poolObject.SetActive(false);

            if (!poolableObjectDict.ContainsKey(name))
                poolableObjectDict.Add(name, new());

            poolableObjectDict[name].Enqueue(poolObject);
        }
        return poolObject;
    }

    private void RegisterPoolObject(GameObject prefab, int initialCount)
    {
        if (prefab is not null)
        {
            string currentName = prefab.name;
            Queue<GameObject> currentQueue;
            Transform currentRoot;

            if (!poolableObjectDict.ContainsKey(currentName))
            {
                poolableObjectDict.Add(currentName, new Queue<GameObject>());
                currentRoot = new GameObject(currentName).transform;
                currentRoot.SetParent(rootTransform);
            }
            else
                currentRoot = GetRoot(currentName);

            currentQueue = poolableObjectDict[currentName];

            for (int i = 0; i < initialCount; i++)
            {
                GameObject currentObject = Instantiate(prefab);
                currentObject.name = currentName;
                Registration(currentObject, currentQueue, currentRoot);
            }
        }
        else
            Debug.LogWarning($"Prefab is null [name : {prefab.name}]");
    }

    private void Registration(GameObject target, Queue<GameObject> queue, Transform root)
    {
        if (target is not null)
        {
            target.SetActive(false);
            queue.Enqueue(target);
            target.transform.SetParent(root);
        }

        if (target.TryGetComponent(out IPoolable poolComponent))
        {
            poolComponent.RootQueue = queue;
        }
    }

    private Transform GetRoot(string key)
    {
        // IEnumerable을 상속 받지만 IEnumerable로 변환해야 확장 메서드 사용 가능, 일반적인 Linq함수는 Cast뒤에 사용할 수 있음.
        // rootTransform.Cast<Transform>();

        // SQL의 질의문
        // linQ에서 가장 특징적인 구문
        //        IEnumerable result = from Transform target                     // 찾으려는 대상
        //                             in rootTransform                          // 원본 자료구조
        //                             where target.gameObject.name == key       // 조건
        //                             orderby rootTransform.position.sqrMagnitude descending // 정렬 기준 asceding 오름 차순 descending 내림 차순
        //                             select target;                            // 가져오려는 값 ( 대상 내부의 값도 가능 )

        return rootTransform.Find(key);
    }

    private GameObject GetPoolInstance(string name)
    {
        if (poolableObjectDict.TryGetValue(name, out Queue<GameObject> queue))
        {
            if (queue.TryDequeue(out GameObject result))
            {
                if (queue.Count == 0)
                    RegisterPoolObject(result, 5);

                return result;
            }

        }

        return null;
    }

    public GameObject SpawnObject(string name, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, Space coord)
    {
        GameObject instance = GetPoolInstance(name);

        if (instance is null) return null;

        Transform transform = instance.transform;
        instance.transform.SetParent(parent);

        if (coord == Space.World)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
        transform.localScale = scale;

        // IPoolable과 MonoBehaviour을 상속받는 클래스를 dictionary값으로 설정 시 
        // TryGetConponent없이 바로 초기화 가능.
        if (instance.TryGetComponent(out IPoolable poolComponent))
            poolComponent.Initialize();

        instance.SetActive(true);

        return instance;
    }

    private void Release2Pool(GameObject target)
    {
        if (target?.TryGetComponent(out IPoolable poolComponent) ?? false)
        {
            Queue<GameObject> rootQueue = poolComponent.RootQueue;
            if (rootQueue is not null)
            {
                target.SetActive(false);
                rootQueue.Enqueue(target);
                target.transform.SetParent(GetRoot(target.name));
                poolComponent.Return2Pool();
                return;
            }
        }

        Destroy(target);
    }

    public static GameObject ClaimSpawn(string key, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, Space coord)
    {
        return OnSpawn?.Invoke(key, position, rotation, scale, parent, coord);
    }
    public static GameObject ClaimSpawn(string key, Transform parent, Vector3 position)
    {
        return OnSpawn?.Invoke(key, position, Quaternion.identity, Vector3.one, parent, Space.Self);
    }
    public static GameObject ClaimSpawn(string key, Transform parent)
    {
        return OnSpawn?.Invoke(key, Vector3.zero, Quaternion.identity, Vector3.one, parent, Space.Self);
    }
    public static GameObject ClaimSpawn(string key, Vector3 position, Quaternion rotation, Vector3 size)
    {
        return OnSpawn?.Invoke(key, position, rotation, size, null, Space.World);
    }
    public static GameObject ClaimSpawn(string key, Vector3 position)
    {
        return OnSpawn?.Invoke(key, position, Quaternion.identity, Vector3.one, null, Space.World);
    }
    public static GameObject ClaimSpawn(string key)
    {
        return OnSpawn?.Invoke(key, Vector3.zero, Quaternion.identity, Vector3.one, null, Space.World);
    }


    public static void ClaimDeSpawn(GameObject target)
    {
        OnDespawn?.Invoke(target);
    }
}