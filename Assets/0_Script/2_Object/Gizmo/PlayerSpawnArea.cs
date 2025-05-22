using UnityEngine;

public partial class PlayerSpawnArea : MonoBehaviour // Data Field
{
#if UNITY_EDITOR
    [SerializeField] private Mesh gizmoMesh;
    private void OnDrawGizmos()
    {
        if (gizmoMesh)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireMesh(gizmoMesh, transform.position, transform.rotation, transform.lossyScale);
        }
    }

#endif
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, 5f);
    //}

}
public partial class PlayerSpawnArea : MonoBehaviour // Initialize
{
    private void Allocate()
    {

    }
    public void Initialize()
    {
        Allocate();
        Setup();
    }
    private void Setup()
    {

    }
}
public partial class PlayerSpawnArea : MonoBehaviour // 
{

}