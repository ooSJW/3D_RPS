using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IManagerBase  // Data Field
{
    // ���� Synchronized
    // �� ���� ASynchronized
    public IEnumerator Initialize();
    public void Exit();
    public bool IsInit { get; }

    // IManagerBase���°� �ƴ� ��ӹ��� Ŭ������ ���·� �����ϸ� �ڵ� �������̵� �Ǿ� ����.


    public void SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode) { }
    public void SceneUnloaded(Scene unloadedScene) { }
}

