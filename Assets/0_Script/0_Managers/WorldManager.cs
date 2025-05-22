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
            // ���� ������ ���� : ĳ���� ���� ��� �⺻ ĳ���� ���� ����ó���� �ʿ���
            // �������� ��η� ��ȸ : ����ó���� �ʿ������ , ����� �ð��� ����� �� ����.
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Additive);

        }
    }
#endif
}