using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class WorldManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; set; }
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        yield break;
    }
    public void Exit()
    {

    }
}

public partial class WorldManager : MonoBehaviour, IManagerBase // Main
{
#if UNITY_EDITOR
    private void Awake()
    {
        if (GameManager.Instance is null)
        {
            // 현재 씬에서 시작 : 캐선이 있을 경우 기본 캐릭터 등의 예외처리가 필요함
            // 정상적인 경로로 우회 : 예외처리는 필요없지만 , 디버깅 시간이 길어질 수 있음.
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Additive);

        }
    }
#endif
}