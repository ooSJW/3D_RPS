using UnityEngine;

public partial class IKSimpleLookAt : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform pointer;
    [SerializeField] private Transform target;
    private Quaternion initialRotation;
    // [SerializeField] private Quaternion additionalRotation = Quaternion.identity;
}



public partial class IKSimpleLookAt : MonoBehaviour
{
    private void Start()
    {
        // 목적지 - 출발지 : 쿼터니온은 -가 없고, +는 *이기 때문에
        // 목적지 * (-출발지)는 목적지 - 출발지와 같음.
        initialRotation = root.rotation * Quaternion.Inverse(pointer.rotation);
    }

    private void LateUpdate()
    {
        IKUpdate(Time.deltaTime);
    }

    private void IKUpdate(float deltaTime)
    {

        if (root && pointer && target)
        {
            Quaternion result = Quaternion.LookRotation(target.position - root.position);
            // Quaternion diff = result * Quaternion.Inverse(pointer.rotation);


            // 두 쿼터니온을 더할때는 *사용 
            // Inverse : 부호 반전
            // result *= initialRotation;
            //  root.rotation = result;
            root.rotation = result * initialRotation;
        }
    }
}