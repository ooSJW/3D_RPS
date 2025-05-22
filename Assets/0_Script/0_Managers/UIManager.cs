using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

// Delegate : �Լ� ������ -> �Լ��� ��� ����
//                          Invoke -> ����� �Լ��� ����

// Action ��ȯ�� x / Func ��ȯ�� o
// Ž���� ��ư�, ���� �̸� ���� �Ұ���, ������ delegate��� ����.

// event : �븮���� �����ڸ� ������, �ش� �븮�ڰ� ����� Ŭ�������� �� Invoke����.

// �ش� ��Ȳ������ ��ȯ ���� ������ �Ű������� int �ϳ��� �Լ��� ���� �� �ִ� ����
public delegate void DelegateLoadingStart(int processAmount);
public delegate void DelegateLoadingNext(string loadingContext, int skipAmount);
public delegate void DelegateLoadingProgress(string loadingText, float progress);
public delegate void DelegateLoadingDone();

public partial class UIManager : MonoBehaviour, IManagerBase // Data Field
{
    public static event DelegateLoadingStart OnLoadingStart;
    public static event DelegateLoadingProgress OnLoadingProgress;
    public static event DelegateLoadingNext OnLoadingNext;
    public static event DelegateLoadingDone OnLoadingDone;

    private const string uiFilePath = "Prefabs/UIPrefabs/";
    public bool IsInit { get; protected set; }

    private GameObject mainCanvas;
    private GameObject loadingScreen;

    private RectTransform mainCanvasRectTransform;
    private GraphicRaycaster mainCanvasRaycaster;
    private CanvasScaler mainCanvasScaler;

    private Dictionary<UIType, GameObject> uiDictionary = new();
}


public partial class UIManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {

        if (mainCanvas = SignupUI(UIType.MainCanvas, mainCanvas))
        {
            mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
            mainCanvasRaycaster = mainCanvas.GetComponent<GraphicRaycaster>();
            mainCanvasScaler = mainCanvas.GetComponent<CanvasScaler>();
        }

        loadingScreen = SignupUI(UIType.Loading, loadingScreen, mainCanvasRectTransform);

        yield break;
    }
    public void Exit()
    {

    }
}


public partial class UIManager : MonoBehaviour, IManagerBase // Property
{

    // ���� ��ġ => ��� ���¿� ���� ���� �����ִ� �Լ�.
    private static GameObject GetUIPrefab(UIType uiType) => uiType switch
    {
        // ���� ��ġ �Ŀ����� _�� default
        UIType.MainCanvas => FileManager.GetPrefab(uiFilePath + "MainCanvas"),
        UIType.Loading => FileManager.GetPrefab(uiFilePath + "LoadingScreen"),

        // ���� ���� ũ�Ⱑ LENGTH���� Ŭ ���.
        (>= UIType._LENGTH) or (< 0) => null,
        _ => null,
    };

    private GameObject InstantiatePrefab(UIType uiType, Transform parent = null)
    {
        GameObject uiObject = GetUIPrefab(uiType);
        return Instantiate(uiObject, parent);
    }

    private GameObject SignupUI(UIType uiType, GameObject uiObject, Transform parent = null)
    {
        if (uiObject is null)
            uiObject = InstantiatePrefab(uiType, parent);

        if (uiObject is null) return null;


        if (!uiDictionary.TryAdd(uiType, uiObject))
        {
            GameObject originValue = uiDictionary[uiType];

            if (uiObject != originValue)
            {
                Destroy(originValue);
                uiDictionary[uiType] = uiObject;
            }
        }

        if (uiObject.TryGetComponent(out IUserInterfaceBase userInterface))
            userInterface.Initialize(this);

        return uiObject;
    }

    public GameObject GetUI(UIType uiType)
    {
        if (uiDictionary.TryGetValue(uiType, out GameObject uiObject))
            return uiObject;
        else
            return null;
    }
}
public partial class UIManager : MonoBehaviour, IManagerBase // Delegate
{
    public static void ClaimLoadingStart(int processAmount)
    {
        OnLoadingStart?.Invoke(processAmount);
    }

    public static void ClaimLoadingProgress(string loadingContext, float progress)
    {
        OnLoadingProgress?.Invoke(loadingContext, progress);
    }

    public static void ClaimLoadingNext(string loadingContext, int skipAmount = 1)
    {
        OnLoadingNext?.Invoke(loadingContext, skipAmount);
    }

    public static void ClaimLoadingDone()
    {
        OnLoadingDone?.Invoke();
    }
}