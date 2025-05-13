using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial interface ManagerBase  // Data Field
{
    // ���� Synchronized
    // �� ���� ASynchronized
    public IEnumerator Initialize();
    public void Exit();
    public bool IsInit { get; }
}

