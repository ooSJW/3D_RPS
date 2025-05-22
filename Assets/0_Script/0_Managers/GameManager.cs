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

    // GameManager ����Ƽ�� ���� �� ���� ����Ǿ��� �� ����� �Լ� ���
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

        UIManager.ClaimLoadingNext("����");
        fileManager = gameObject?.AddComponent<FileManager>();
        yield return fileManager?.Initialize();

        UIManager.ClaimLoadingNext("���̺�");
        saveManager = gameObject?.AddComponent<SaveManager>();
        yield return saveManager?.Initialize();

        UIManager.ClaimLoadingNext("�ɼ�");
        optionManager = gameObject?.AddComponent<OptionManager>();
        yield return optionManager?.Initialize();

        UIManager.ClaimLoadingNext("����");
        soundManager = gameObject?.AddComponent<SoundManager>();
        yield return soundManager?.Initialize();

        UIManager.ClaimLoadingNext("ī�޶�");
        cameraManager = gameObject?.AddComponent<CameraManager>();
        yield return cameraManager?.Initialize();

        UIManager.ClaimLoadingNext("�Է�");
        userInputManager = gameObject?.AddComponent<UserInputManager>();
        yield return userInputManager?.Initialize();

        UIManager.ClaimLoadingDone();

#if UNITY_EDITOR
        // �������� ��η� ���۵� ��Ȳ
        if (SceneManager.sceneCount == 1)

#endif
            // �����س��� Task�� Start�� Task.Run�� ����ؾ���.
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
        // != , is not null�� ����
        // 1. !=�� objet�� �����ڷ� �ڽ� ��ڽ��� �Ͼ �� ���� (���� ������ ������ �� ������ ����.)
        // 2. fake null : ����Ƽ�� none�� missing���� ��� ���� null���°� �ƴ� ���ڿ�"null"�� �����ص�, �� ���ڿ� �񱳰� �Ͼ.

        // is : ��ȯ �������� Ȯ��, as : ��ȯ ���� �� null��ȯ
        slot = gameObject.AddComponent(componentType) as IManagerBase;

        if (slot is not null)
            yield return slot.Initialize();
    }

    public void SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        // ���� GameManager�� ���� ���� �׻� 0������ ����
        if (loadedScene.buildIndex != 0)
            CurrentScene = loadedScene;

        cameraManager?.SceneLoaded(loadedScene, loadSceneMode);
    }

    public void SceneUnloaded(Scene unloadedScene)
    {

    }

    // Task : �����忡�� ��ų ��
    public async Task LoadSceneAsync(string sceneName)
    {
        // �񵿱� �� �ε�
        if (CurrentScene.isLoaded)
        {
            // await : �񵿱� ��û �� �Ϸ�� ������ ���
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
    // Start�� �ڷ�ƾ���� ��� ����
    // IEnumerator : �Լ��� ������ �����ϴ� ���� / ������ �����߰�, �������� ��ٸ����� ����� ����
    IEnumerator Start()
    {
        // this�Ű������� ���, this �Ű������� ù ��° �Ű������� �� ��� ����.
        // Extensions.GenericSingleton(this, ref instance); �� ����
        if (!this.GenericSingleton(ref instance)) Destroy(this);
        else
            yield return initializer = Initialize();
    }
}
