using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour, IManagerBase
{
    public bool IsInit { get; set; }

    public void Exit()
    {

    }

    public IEnumerator Initialize()
    {
        yield break;
    }
}
