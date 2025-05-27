using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public partial class PoolManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    private Dictionary<string, Queue<GameObject>> poolableObjectDict = new();

    private Transform rootTransform;

    public bool create;
    // TODO TEST
    private void Update()
    {
        if (create)
        {
            SpawnObject("CharacterBase",
                Random.insideUnitSphere * 5f,
                Random.rotation,
                Vector3.one * Random.Range(0.5f, 2f),
                null,
                Space.World);

            create = false;
        }
    }
}

public partial class PoolManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        rootTransform = new GameObject("[Root Pool]").transform;
        RegisterPoolObject(FileManager.CharacterPrefabDict[CharacterType.CharacterBase], 50);
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
        // IEnumerable�� ��� ������ IEnumerable�� ��ȯ�ؾ� Ȯ�� �޼��� ��� ����, �Ϲ����� Linq�Լ��� Cast�ڿ� ����� �� ����.
        // rootTransform.Cast<Transform>();

        // SQL�� ���ǹ�
        // linQ���� ���� Ư¡���� ����
        //        IEnumerable result = from Transform target                     // ã������ ���
        //                             in rootTransform                          // ���� �ڷᱸ��
        //                             where target.gameObject.name == key       // ����
        //                             orderby rootTransform.position.sqrMagnitude descending // ���� ���� asceding ���� ���� descending ���� ����
        //                             select target;                            // ���������� �� ( ��� ������ ���� ���� )

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

        instance.SetActive(true);

        return instance;
    }
}