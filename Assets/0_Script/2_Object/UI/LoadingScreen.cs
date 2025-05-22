using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class LoadingScreen : MonoBehaviour, IUserInterfaceBase // Data Field
{
    public bool IsOpen { get; protected set; }

    [SerializeField] private Image loadingImage;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;

    private int currentAmount;
    private int maxAmount;

    public float Progress
    {
        get => maxAmount <= 0 ? 1.0f : currentAmount / (float)maxAmount;
        set => currentAmount = Mathf.RoundToInt(value * maxAmount);
        // 반올림 할 때 Mathf.Round()를 사용하거나 값에 +0.5를 하는 방법도 있음
        // 아마 Round() 내부적으로 +0.5하지않을까?
    }
}

public partial class LoadingScreen : MonoBehaviour, IUserInterfaceBase// Initialize
{
    public void Initialize(UIManager uiManager)
    {
        UIManager.OnLoadingStart -= LoadingStart;
        UIManager.OnLoadingStart += LoadingStart;

        UIManager.OnLoadingProgress -= LoadingProgress;
        UIManager.OnLoadingProgress += LoadingProgress;

        UIManager.OnLoadingNext -= LoadingNext;
        UIManager.OnLoadingNext += LoadingNext;

        UIManager.OnLoadingDone -= LoadingDone;
        UIManager.OnLoadingDone += LoadingDone;
    }

    public bool Active(bool value)
    {
        return IsOpen;
    }
    public bool Open(bool value)
    {
        gameObject.SetActive(value);
        IsOpen = value;
        return value;
    }

    public bool Toggle()
    {
        return Open(!IsOpen);
    }

    public void Exit()
    {
        UIManager.OnLoadingStart -= LoadingStart;
        UIManager.OnLoadingProgress -= LoadingProgress;
        UIManager.OnLoadingNext -= LoadingNext;
        UIManager.OnLoadingDone -= LoadingDone;
    }
}
public partial class LoadingScreen : MonoBehaviour, IUserInterfaceBase // Property
{
    private void LoadingStart(int processAmount)
    {
        currentAmount = 0;
        maxAmount = processAmount;
        Visualize("TEST", Progress);

        Open(true);
    }

    private void LoadingProgress(string loadingText, float progress)
    {
        Progress = progress;
        Visualize(loadingText, progress);
    }

    private void LoadingNext(string loadingContext, int skipAmount)
    {
        currentAmount += skipAmount;
        Visualize(loadingContext, Progress);
    }

    private void LoadingDone()
    {
        Open(false);
    }

    private void Visualize(string context, float progress)
    {
        loadingText.text = context;
        loadingSlider.value = progress;

        // Quaternion.AngleAxis(회전시킬 각도, 기준 축);
        loadingImage.transform.rotation = Quaternion.AngleAxis(progress * 1080.0f, Vector3.forward);
    }
}