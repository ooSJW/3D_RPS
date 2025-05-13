using System.Collections;
using UnityEngine;

public class WorldManager : MonoBehaviour, ManagerBase
{
    public bool IsInit { get;  set; }

    public void Exit()
    {

    }

    public IEnumerator Initialize()
    {
        yield break;
    }
}
