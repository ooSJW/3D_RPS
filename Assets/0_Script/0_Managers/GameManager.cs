using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

public partial class GameManager : MonoBehaviour, IManagerBase // Data Field
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

public partial class GameManager : MonoBehaviour, IManagerBase
{
    public IEnumerator Initialize()
    {
        yield return AddManager(uiManager);
        UIManager.ClaimLoadingStart(6);

        UIManager.ClaimLoadingNext("파일");
        yield return AddManager(fileManager);

        UIManager.ClaimLoadingNext("세이브");
        yield return AddManager(saveManager);

        UIManager.ClaimLoadingNext("옵션");
        yield return AddManager(optionManager);

        UIManager.ClaimLoadingNext("사운드");
        yield return AddManager(soundManager);

        UIManager.ClaimLoadingNext("카메라");
        yield return AddManager(cameraManager);

        UIManager.ClaimLoadingNext("입력");
        yield return AddManager(userInputManager);

        UIManager.ClaimLoadingDone();
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

public partial class GameManager : MonoBehaviour, IManagerBase
{
    private IEnumerator AddManager<T>(T slot) where T : IManagerBase
    {
        yield return AddManager(typeof(T), slot);
    }

    private IEnumerator AddManager(Type componentType, IManagerBase slot)
    {
        // != , is not null의 차이
        // 1. !=은 objet의 연산자로 박싱 언박싱이 일어날 수 있음 (성능 저하의 원인이 될 여지가 있음.)
        // 2. fake null : 유니티는 none과 missing상태 모두 실제 null상태가 아닌 문자열"null"을 저장해둠, 즉 문자열 비교가 일어남.

        // is : 변환 가능한지 확인, as : 변환 실패 시 null반환
        slot = gameObject.AddComponent(componentType) as IManagerBase;

        if (slot is not null)
            yield return slot.Initialize();
    }
}

public partial class GameManager : MonoBehaviour, IManagerBase
{
    // Start만 코루틴으로 사용 가능
    // IEnumerator : 함수를 나눠서 실행하는 형식 / 어디까지 실행했고, 언제까지 기다릴지가 저장된 형식
    IEnumerator Start()
    {
        // this매개변수의 사용, this 매개변수는 첫 번째 매개변수로 만 사용 가능.
        // Extensions.GenericSingleton(this, ref instance); 과 같음
        if (!this.GenericSingleton(ref instance)) Destroy(this);
        else
            yield return initializer = Initialize();
    }
}
