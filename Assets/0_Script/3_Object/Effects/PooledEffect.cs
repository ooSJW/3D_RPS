using System.Collections.Generic;
using UnityEngine;

public partial class PooledEffect : MonoBehaviour, IPoolable // Data Field
{
    public Queue<GameObject> RootQueue { get; set; }

    // 일반적으로 시간을 언급할 때 0 또는 음수일 경우 무한으로 인식.
    [SerializeField] private float removeTime;

}
public partial class PooledEffect  // Initialize
{
    public void Initialize()
    {
        if (removeTime > 0)
        {
            Invoke(nameof(ClaimDeSpawn), removeTime);
        }
    }

}
public partial class PooledEffect  // 
{
    public void ClaimDeSpawn() => PoolManager.ClaimDeSpawn(gameObject);

    public void Return2Pool()
    {

    }
}
