using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

// Delegate : 함수 포인터 -> 함수를 담는 변수
//                          Invoke -> 저장된 함수를 실행

// Action 반환값 x / Func 반환값 o
// 탐색이 어렵고, 변수 이름 지정 불가능, 때문에 delegate사용 지향.

// event : 대리자의 소유자를 지정함, 해당 대리자가 선언된 클래스에서 만 Invoke가능.

// 해당 상황에서는 반환 값이 없으며 매개변수가 int 하나인 함수를 담을 수 있는 형태
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

    // 패턴 일치 => 대상 상태에 따른 값을 돌려주는 함수.
    private static GameObject GetUIPrefab(UIType uiType) => uiType switch
    {
        // 패턴 일치 식에서의 _는 default
        UIType.MainCanvas => FileManager.GetPrefab(uiFilePath + "MainCanvas"),
        UIType.Loading => FileManager.GetPrefab(uiFilePath + "LoadingScreen"),

        // 들어온 값의 크기가 LENGTH보다 클 경우.
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