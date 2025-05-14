using System.Collections;
using UnityEngine;

public class UserInputManager : MonoBehaviour, IManagerBase
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
