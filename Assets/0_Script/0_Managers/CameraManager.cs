using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CameraManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }

    private Camera mainCamera;

    private CharacterBase targetCharacter;
}
public partial class CameraManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        ControllerManager.OnLocalControllerPossessed -= CharacterChanged;
        ControllerManager.OnLocalControllerPossessed += CharacterChanged;
        OptionManager.OnGraphicOptionChanged -= OptionChanged;
        OptionManager.OnGraphicOptionChanged += OptionChanged;
        GameManager.OnPostUpdate -= UpdateCameraPosition;
        GameManager.OnPostUpdate += UpdateCameraPosition;

        UpdateCamera();

        yield break;
    }

    public void Exit()
    {
        ControllerManager.OnLocalControllerPossessed -= CharacterChanged;
        OptionManager.OnGraphicOptionChanged -= OptionChanged;
        GameManager.OnPostUpdate -= UpdateCameraPosition;
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

    private void CharacterChanged(CharacterBase newTarget)
    {
        targetCharacter = newTarget;
        DisplayCharacterView(targetCharacter);
    }

    private void DisplayCharacterView(CharacterBase wantCharacter)
    {
        mainCamera.transform.position = wantCharacter.transform.position;
        mainCamera.transform.forward = wantCharacter.Forward;
    }

    private void UpdateCameraPosition(float deltaTime)
    {
        DisplayCharacterView(targetCharacter);
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
