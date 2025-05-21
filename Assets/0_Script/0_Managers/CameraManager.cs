using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CameraManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    private Camera mainCamera;
}
public partial class CameraManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        OptionManager.OnGraphicOptionChanged -= OptionChanged;
        OptionManager.OnGraphicOptionChanged += OptionChanged;
        UpdateCamera();
        yield break;
    }

    public void Exit()
    {
        OptionManager.OnGraphicOptionChanged -= OptionChanged;
    }
}
public partial class CameraManager : MonoBehaviour, IManagerBase // Property
{
    private void UpdateCamera()
    {
        Camera currentCamera = Camera.main;
        if (mainCamera != currentCamera)
        {
            mainCamera = currentCamera;
            OptionChanged(OptionManager.appliedGraphicOption);
        }
    }

    private void OptionChanged(GraphicOptionValues value)
    {
        if (mainCamera is not null)
            mainCamera.fieldOfView = value.fieldOfView;
    }

    public void SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        UpdateCamera();
    }
}
