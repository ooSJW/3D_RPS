using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public interface IManagerBase  // Data Field
{
    // ���� Synchronized
    // �� ���� ASynchronized
    public IEnumerator Initialize();
    public void Exit();
    public bool IsInit { get; }
}

