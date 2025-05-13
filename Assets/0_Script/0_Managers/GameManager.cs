using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

public partial class GameManager : MonoBehaviour, ManagerBase // Data Field
{
    private static GameManager instance;

    private bool isInit = false;
    public bool IsInit { get => isInit; protected set => isInit = value; }
    private IEnumerator initializer;



    private UIManager uiManager;
    private FileManager fileManager;
    private SaveManager saveManager;
    private OptionManager optionManager;
    private SoundManager soundManager;
    private CameraManager cameraManager;
    private UserInputManager userInputManager;

}

public partial class GameManager : MonoBehaviour, ManagerBase
{
    public IEnumerator Initialize()
    {
        yield return AddManager<UIManager>(uiManager);
        yield return AddManager<FileManager>(fileManager);
        yield return AddManager<SaveManager>(saveManager);
        yield return AddManager<OptionManager>(optionManager);
        yield return AddManager<SoundManager>(soundManager);
        yield return AddManager<CameraManager>(cameraManager);
        yield return AddManager<UserInputManager>(userInputManager);

        isInit = true;
    }

    public void Exit()
    {
        if (initializer is null && isInit == false)
            StopCoroutine(initializer);

        userInputManager?.Exit();
        cameraManager?.Exit();
        soundManager?.Exit();
        optionManager?.Exit();
        saveManager?.Exit();
        fileManager?.Exit();
        uiManager?.Exit();
    }
}

public partial class GameManager : MonoBehaviour, ManagerBase
{
    private IEnumerator AddManager<T>(T slot) where T : ManagerBase
    {
        yield return AddManager(typeof(T), slot);
    }

    private IEnumerator AddManager(Type componentType, ManagerBase slot)
    {
        // != , is not null의 차이
        // 1. !=은 objet의 연산자로 박싱 언박싱이 일어날 수 있음 (성능 저하의 원인이 될 여지가 있음.)
        // 2. fake null : 유니티는 none과 missing상태 모두 실제 null상태가 아닌 문자열"null"을 저장해둠, 즉 문자열 비교가 일어남.

        // is : 변환 가능한지 확인, as : 변환 실패 시 null반환
        slot = gameObject.AddComponent(componentType) as ManagerBase;

        if (slot is not null)
            yield return slot.Initialize();
    }
}

public partial class GameManager : MonoBehaviour, ManagerBase
{
    // Start만 코루틴으로 사용 가능
    // IEnumerator : 함수를 나눠서 실행하는 형식 / 어디까지 실행했고, 언제까지 기다릴지가 저장된 형식
    IEnumerator Start()
    {
        // Extensions.GenericSingleton(this, ref instance); 과 같음
        if (!this.GenericSingleton(ref instance)) Destroy(this);
        else
            yield return initializer = Initialize();
    }
}
