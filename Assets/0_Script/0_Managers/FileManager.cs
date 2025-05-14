using System.Collections;
using UnityEngine;

public partial class FileManager : MonoBehaviour, IManagerBase // Data Field
{
    public bool IsInit { get; protected set; }
}

public partial class FileManager : MonoBehaviour, IManagerBase // Initialize
{
    public IEnumerator Initialize()
    {
        yield break;
    }

    public void Exit()
    {

    }
}
public partial class FileManager : MonoBehaviour, IManagerBase // Property
{
    public static GameObject GetPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }
}
