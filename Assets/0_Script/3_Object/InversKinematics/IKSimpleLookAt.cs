using UnityEngine;

public partial class IKSimpleLookAt : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private Transform target;

    [SerializeField] private Quaternion additionalRotation = Quaternion.identity;
}



public partial class IKSimpleLookAt : MonoBehaviour
{
    private void LateUpdate()
    {
        IKUpdate(Time.deltaTime);
    }

    private void IKUpdate(float deltaTime)
    {

        if (tip && target)
        {
            if (additionalRotation != Quaternion.identity)
            {

                Quaternion result = Quaternion.LookRotation(target.position - tip.position);
                tip.rotation = result * additionalRotation;
            }
            else
            {
                tip.LookAt(target);
            }
        }
        
    }
}