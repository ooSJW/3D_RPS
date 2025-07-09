using System;
using System.Collections.Generic;
using UnityEngine;

public enum SocketType
{
    None, CameraOffset, Hand_R, Hand_L, Foot_R, Foot_L, Eye, Muzzle, Ejection_Port
}

public partial class SocketBase : MonoBehaviour // Data Field
{
    [SerializeField] private SocketType socketTtype;
    public SocketType SocketType => socketTtype;
}

public partial class SocketBase // Initialize
{
    public void Initialize()
    {

    }

}
public partial class SocketBase : MonoBehaviour // 
{
    public void AttachTransform(Transform wantTransform)
    {
        foreach (Transform currentTransform in wantTransform)
        {
            currentTransform.gameObject.layer = gameObject.layer;
        }
        wantTransform.SetParent(transform);
        wantTransform.localPosition = Vector3.zero;
        wantTransform.localRotation = Quaternion.identity;
        wantTransform.localScale = Vector3.one;
    }
    public SocketBase GetSocket() => this;

    public void GetSockets(List<SocketBase> result) => result.Add(this);

    public void GetSockets(List<SocketBase> result, SocketType wantType) { if (SocketType == wantType) result.Add(this); }

    public void GetSockets(List<SocketBase> result, Func<SocketBase, bool> predicate) { if (predicate(this)) result.Add(this); }


    public SocketBase GetSocket(SocketType wantType) => (SocketType == wantType) ? this : null;


    // 아래 함수를 델리게이트에 넣어 사용하면, 찾으려는 값이 가장 마지막에 있을때만 반환 가능.
    public SocketBase GetSocket(Func<SocketBase, bool> predicate) => predicate(this) ? this : null;

    // ↓ 이미 탐색에 성공했다면 아무 작업도 안함.
    public void GetSocket(ref SocketBase result)
    {
        if (result is null)
            result = GetSocket();
    }
    public void GetSocket(ref SocketBase result, Func<SocketBase, bool> predicate)
    {
        if (result is null)
            result = GetSocket(predicate);
    }
    public void GetSocket(ref SocketBase result, SocketType wantType)
    {
        if (result is null)
            result = GetSocket(wantType);
    }

    public void SocketAction(Action<SocketBase> wantAction)
    { if (enabled) wantAction(this); }

    public void SocketActionByType(SocketType wantType, Action<SocketBase> wantAction)
    { if (enabled && wantType == SocketType) wantAction(this); }

    public void SocketActionByPredicate(Func<SocketBase, bool> predicate, Action<SocketBase> wantAction)
    { if (enabled && predicate(this)) wantAction(this); }
}