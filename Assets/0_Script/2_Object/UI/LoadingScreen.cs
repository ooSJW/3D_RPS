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

    public float Progress => maxAmount <= 0 ? 1.0f : currentAmount / (float)maxAmount;
}

public partial class LoadingScreen : MonoBehaviour, IUserInterfaceBase// Initialize
{
    public void Initialize(UIManager uiManager)
    {
        UIManager.OnLoadingStart -= OnLoadingStart;
        UIManager.OnLoadingStart += OnLoadingStart;

        UIManager.OnLoadingNext -= OnLoadingNext;
        UIManager.OnLoadingNext += OnLoadingNext;

        UIManager.OnLoadingDone -= OnLoadingDone;
        UIManager.OnLoadingDone += OnLoadingDone;
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
        UIManager.OnLoadingStart -= OnLoadingStart;
        UIManager.OnLoadingNext -= OnLoadingNext;
        UIManager.OnLoadingDone -= OnLoadingDone;
    }
}
public partial class LoadingScreen : MonoBehaviour, IUserInterfaceBase // Property
{
    private void OnLoadingStart(int processAmount)
    {
        Open(true);
    }

    private void OnLoadingNext(string loadingContext, int skipAmount)
    {

    }

    private void OnLoadingDone()
    {
        Open(false);
    }
}