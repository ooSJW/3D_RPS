using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SocketMonoBehaviour : MonoBehaviour, ISocketContainer // Data Field
{
    public event DelegateGetSockets OnGetSockets;
    public event DelegateGetSocketsByType OnGetSocketsByType;
    public event DelegateGetSocketsByPredicate OnGetSocketsByPredicate;

    public event DelegateGetSocket OnGetSocket;
    public event DelegateGetSocketByType OnGetSocketByType;
    public event DelegateGetSocketByPredicate OnGetSocketByPredicate;

    public event DelegateSocketAction OnSocketAction;
    public event DelegateSocketActionByType OnSocketActionByType;
    public event DelegateSocketActionByPredicate OnSocketActionByPredicate;
}
public partial class SocketMonoBehaviour
{

    public virtual SocketBase GetSocket()
    {
        SocketBase result = null;
        OnGetSocket?.Invoke(ref result);
        return result;
    }

    public virtual SocketBase GetSocket(SocketType wantType)
    {
        SocketBase result = null;
        OnGetSocketByType?.Invoke(ref result, wantType);
        return result;
    }

    public virtual SocketBase GetSocket(Func<SocketBase, bool> predicate)
    {
        SocketBase result = null;
        OnGetSocketByPredicate?.Invoke(ref result, predicate);
        return result;
    }

    public virtual SocketBase[] GetSockets()
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSockets?.Invoke(result);
        return result.ToArray();
    }

    public virtual SocketBase[] GetSockets(SocketType wantType)
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSocketsByType?.Invoke(result, wantType);
        return result.ToArray();
    }
    public virtual SocketBase[] GetSockets(Func<SocketBase, bool> predicate)
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSocketsByPredicate?.Invoke(result, predicate);
        return result.ToArray();
    }

    public virtual void GetSockets(List<SocketBase> result)
        => OnGetSockets?.Invoke(result);


    public virtual void GetSockets(List<SocketBase> result, SocketType wantType)
        => OnGetSocketsByType?.Invoke(result, wantType);


    public virtual void GetSockets(List<SocketBase> result, Func<SocketBase, bool> predicate)
        => OnGetSocketsByPredicate?.Invoke(result, predicate);



    public virtual void AddSocket(SocketBase target)
    {
        if (target is null) return;

        OnGetSockets -= target.GetSockets;
        OnGetSocketsByType -= target.GetSockets;
        OnGetSocketsByPredicate -= target.GetSockets;
        OnGetSockets += target.GetSockets;
        OnGetSocketsByType += target.GetSockets;
        OnGetSocketsByPredicate += target.GetSockets;


        OnGetSocket -= target.GetSocket;
        OnGetSocketByType -= target.GetSocket;
        OnGetSocketByPredicate -= target.GetSocket;
        OnGetSocket += target.GetSocket;
        OnGetSocketByType += target.GetSocket;
        OnGetSocketByPredicate += target.GetSocket;


        OnSocketAction -= target.SocketAction;
        OnSocketActionByType -= target.SocketActionByType;
        OnSocketActionByPredicate -= target.SocketActionByPredicate;
        OnSocketAction += target.SocketAction;
        OnSocketActionByType += target.SocketActionByType;
        OnSocketActionByPredicate += target.SocketActionByPredicate;

    }

    public virtual void AddSocket(params SocketBase[] target)
    {
        foreach (SocketBase current in target) AddSocket(current);
    }

    public virtual void RemoveSocket(SocketBase target)
    {
        if (target is null) return;

        OnGetSockets -= target.GetSockets;
        OnGetSocketsByType -= target.GetSockets;
        OnGetSocketsByPredicate -= target.GetSockets;

        OnGetSocket -= target.GetSocket;
        OnGetSocketByType -= target.GetSocket;
        OnGetSocketByPredicate -= target.GetSocket;

        OnSocketAction -= target.SocketAction;
        OnSocketActionByType -= target.SocketActionByType;
        OnSocketActionByPredicate -= target.SocketActionByPredicate;
    }


    public virtual void RemoveSocket(Func<SocketBase, bool> predicate)
    {
        foreach (SocketBase current in GetSockets(predicate))
            RemoveSocket(current);
    }

    public virtual void SocketAction(Action<SocketBase> wantAction)
        => OnSocketAction?.Invoke(wantAction);
    public virtual void SocketActionByType(SocketType wantType, Action<SocketBase> wantAction)
        => OnSocketActionByType?.Invoke(wantType, wantAction);

    public virtual void SocketActionByPredicate(Func<SocketBase, bool> predicate, Action<SocketBase> wantAction)
        => OnSocketActionByPredicate?.Invoke(predicate, wantAction);


}
