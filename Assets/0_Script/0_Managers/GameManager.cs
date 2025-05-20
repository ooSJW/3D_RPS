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
        //yield return AddManager(uiManager);

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
        // != , is not null�� ����
        // 1. !=�� objet�� �����ڷ� �ڽ� ��ڽ��� �Ͼ �� ���� (���� ������ ������ �� ������ ����.)
        // 2. fake null : ����Ƽ�� none�� missing���� ��� ���� null���°� �ƴ� ���ڿ�"null"�� �����ص�, �� ���ڿ� �񱳰� �Ͼ.

        // is : ��ȯ �������� Ȯ��, as : ��ȯ ���� �� null��ȯ
        slot = gameObject.AddComponent(componentType) as IManagerBase;

        if (slot is not null)
            yield return slot.Initialize();
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
