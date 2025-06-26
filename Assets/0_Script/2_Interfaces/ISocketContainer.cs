using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateGetSockets(List<SocketBase> result);
public delegate void DelegateGetSocketsByType(List<SocketBase> result, SocketType wantType);
public delegate void DelegateGetSocketsByPredicate(List<SocketBase> result, Func<SocketBase, bool> predicate);

public delegate void DelegateGetSocket(ref SocketBase result);
public delegate void DelegateGetSocketByType(ref SocketBase result, SocketType wantType);
public delegate void DelegateGetSocketByPredicate(ref SocketBase result, Func<SocketBase, bool> predicate);

public delegate void DelegateSocketAction(Action<SocketBase> wantAction);
public delegate void DelegateSocketActionByType(SocketType wantType, Action<SocketBase> wantAction);
public delegate void DelegateSocketActionByPredicate(Func<SocketBase, bool> predicate, Action<SocketBase> wantAction);

public partial interface ISocketContainer
{
    public void GetSockets(List<SocketBase> result);
    public void GetSockets(List<SocketBase> result, SocketType wantType);
    public void GetSockets(List<SocketBase> result, Func<SocketBase, bool> predicate);

    public SocketBase GetSocket();
    public SocketBase GetSocket(SocketType wantType);
    public SocketBase GetSocket(Func<SocketBase, bool> predicate);

    public void AddSocket(SocketBase target);
    public void AddSocket(params SocketBase[] target);

    public void RemoveSocket(SocketBase target);
    public void RemoveSocket(Func<SocketBase, bool> predicate);

    public void SocketAction(Action<SocketBase> wantAction);
    public void SocketActionByType(SocketType wantType, Action<SocketBase> wantAction);
    public void SocketActionByPredicate(Func<SocketBase, bool> predicate, Action<SocketBase> wantAction);
}
