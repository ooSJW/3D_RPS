using JetBrains.Annotations;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.HDROutputUtils;

public partial class GameManager : MonoBehaviour, IManagerBase // Data Field
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    // private const string initializeSceneName = "ZTESTScene";
    [SerializeField] private string initializeSceneName;

    private bool isInit = false;
    public bool IsInit { get => isInit; protected set => isInit = value; }
    private IEnumerator initializer;

    public Scene CurrentScene { get; private set; }
    private AsyncOperation sceneLoadProgress;

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

    // GameManager 유니티에 연결 및 씬이 변경되었을 때 사용할 함수 등록
    public IEnumerator Initialize()
    {
        //yield return AddManager(uiManager);
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnloaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        uiManager = gameObject?.AddComponent<UIManager>();
        yield return uiManager?.Initialize();
        UIManager.ClaimLoadingStart(6);

        UIManager.ClaimLoadingNext("파일");
        fileManager = gameObject?.AddComponent<FileManager>();
        yield return fileManager?.Initialize();

        UIManager.ClaimLoadingNext("세이브");
        saveManager = gameObject?.AddComponent<SaveManager>();
        yield return saveManager?.Initialize();

        UIManager.ClaimLoadingNext("옵션");
        optionManager = gameObject?.AddComponent<OptionManager>();
        yield return optionManager?.Initialize();

        UIManager.ClaimLoadingNext("사운드");
        soundManager = gameObject?.AddComponent<SoundManager>();
        yield return soundManager?.Initialize();

        UIManager.ClaimLoadingNext("카메라");
        cameraManager = gameObject?.AddComponent<CameraManager>();
        yield return cameraManager?.Initialize();

        UIManager.ClaimLoadingNext("입력");
        userInputManager = gameObject?.AddComponent<UserInputManager>();
        yield return userInputManager?.Initialize();

        UIManager.ClaimLoadingDone();

#if UNITY_EDITOR
        // 정상적인 경로로 시작된 상황
        if (SceneManager.sceneCount == 1)

#endif
            // 저장해놓은 Task는 Start나 Task.Run을 사용해야함.
            LoadSceneAsync(initializeSceneName);

        UIManager.ClaimLoadingDone();

        isInit = true;
    }

    public void Exit()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnloaded;

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

public partial class GameManager : MonoBehaviour, IManagerBase // Property
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

    public void SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        // 최초 GameManager를 가진 씬은 항상 0번으로 유지
        if (loadedScene.buildIndex != 0)
            CurrentScene = loadedScene;

        cameraManager?.SceneLoaded(loadedScene, loadSceneMode);
    }

    public void SceneUnloaded(Scene unloadedScene)
    {

    }

    // Task : 쓰레드에게 시킬 일
    public async Task LoadSceneAsync(string sceneName)
    {
        // 비동기 씬 로드
        if (CurrentScene.isLoaded)
        {
            // await : 비동기 요청 후 완료될 때까지 대기
            await SceneManager.UnloadSceneAsync(CurrentScene.buildIndex);
        }

        StartCoroutine(LoadingScene(sceneName));
    }


    private IEnumerator LoadingProgress(AsyncOperation operation, string loadingContext)
    {
        UIManager.ClaimLoadingStart(100);

        while (!operation.isDone)
        {
            UIManager.ClaimLoadingProgress($"{loadingContext} : {operation.progress * 100f}%", operation.progress);
            yield return null;
        }

        UIManager.ClaimLoadingDone();
    }

    private IEnumerator LoadingScene(string sceneName)
    {
        UIManager.ClaimLoadingStart(100);
        sceneLoadProgress = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!sceneLoadProgress.isDone)
        {
            UIManager.ClaimLoadingProgress($"Loading {sceneName} : {sceneLoadProgress.progress * 100f}%", sceneLoadProgress.progress * 0.3f);
            yield return null;
        }

        WorldManager worldManager = FindFirstObjectByType<WorldManager>();
        if (worldManager is not null)
            yield return worldManager.Initialize();

        UIManager.ClaimLoadingDone();
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
