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
        // != , is not null�� ����
        // 1. !=�� objet�� �����ڷ� �ڽ� ��ڽ��� �Ͼ �� ���� (���� ������ ������ �� ������ ����.)
        // 2. fake null : ����Ƽ�� none�� missing���� ��� ���� null���°� �ƴ� ���ڿ�"null"�� �����ص�, �� ���ڿ� �񱳰� �Ͼ.

        // is : ��ȯ �������� Ȯ��, as : ��ȯ ���� �� null��ȯ
        slot = gameObject.AddComponent(componentType) as ManagerBase;

        if (slot is not null)
            yield return slot.Initialize();
    }
}

public partial class GameManager : MonoBehaviour, ManagerBase
{
    // Start�� �ڷ�ƾ���� ��� ����
    // IEnumerator : �Լ��� ������ �����ϴ� ���� / ������ �����߰�, �������� ��ٸ����� ����� ����
    IEnumerator Start()
    {
        // Extensions.GenericSingleton(this, ref instance); �� ����
        if (!this.GenericSingleton(ref instance)) Destroy(this);
        else
            yield return initializer = Initialize();
    }
}
