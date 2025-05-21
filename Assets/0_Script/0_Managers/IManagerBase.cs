using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IManagerBase  // Data Field
{
    // 동기 Synchronized
    // 비 동기 ASynchronized
    public IEnumerator Initialize();
    public void Exit();
    public bool IsInit { get; }

    // IManagerBase형태가 아닌 상속받은 클래스의 형태로 실행하면 자동 오버라이딩 되어 실행.


    public void SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode) { }
    public void SceneUnloaded(Scene unloadedScene) { }
}

