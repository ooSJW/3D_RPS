using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public interface IManagerBase  // Data Field
{
    // 동기 Synchronized
    // 비 동기 ASynchronized
    public IEnumerator Initialize();
    public void Exit();
    public bool IsInit { get; }
}

