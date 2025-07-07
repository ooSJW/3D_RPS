using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateCharacterMove(CharacterBase mover, Vector3 velocity, float deltaTime);
public delegate void DelegateCharacterAim(Vector3 direction);
public delegate void DelegateCharacterAttack(Vector3 direction, bool value);
public delegate void DelegateCharacterJump();
public delegate void DelegateCharacterReload();
public delegate void DelegateCharacterRun(bool value);
public delegate void DelegateCharacterDie();
public delegate void DelegateCharacterInteraction(GameObject target);
public delegate void DelegateCharacterOwnerChanged(ControllerBase newController);

public delegate int DelegateCharacterGetDamage(int damage, Vector3 direction, bool isCritical, GameObject causer);
public delegate void DelegateCharacterSendDamage(ref int totalDamage, ref float multiplier, ref bool isCritical);
public delegate void DelegatCharacterAnimaionPlay(AnimationType wanType);
public delegate void DelegatCharacterAnimaionTrigger(string wantTrigger);

public partial class CharacterBase : MonoBehaviour, IPoolable, ISocketContainer // Delegate
{
    public event Action OnModuleLoaded;
    public event DelegateCharacterMove OnMove;
    public event DelegateCharacterAim OnAim;
    public event DelegateCharacterAttack OnAttack;
    public event DelegateCharacterJump OnJump;
    public event DelegateCharacterReload OnReload;
    public event DelegateCharacterRun OnRun;
    public event DelegateCharacterDie OnDie;
    public event DelegateCharacterInteraction OnInteraction;

    public event DelegateCharacterGetDamage OnGetDamage;
    public event DelegateCharacterSendDamage OnSendDamage;

    public event DelegateGetSockets OnGetSockets;
    public event DelegateGetSocketsByType OnGetSocketsByType;
    public event DelegateGetSocketsByPredicate OnGetSocketsByPredicate;

    public event DelegateGetSocket OnGetSocket;
    public event DelegateGetSocketByType OnGetSocketByType;
    public event DelegateGetSocketByPredicate OnGetSocketByPredicate;

    public event DelegateCharacterOwnerChanged OnOwnerChanged;

    public event DelegateSocketAction OnSocketAction;
    public event DelegateSocketActionByType OnSocketActionByType;
    public event DelegateSocketActionByPredicate OnSocketActionByPredicate;

    public event DelegatCharacterAnimaionTrigger OnAnimationTrigger;
}

public partial class CharacterBase// Data Field
{
    public ControllerBase Controller { get; protected set; }

    // ai이거나 직접 배치한 경우 생성된 Controller , 기본적으로 몬스터, 테스트 시 직접 참조할 캐릭터
    public ControllerType BaseControllerType => baseControllerType;
    [SerializeField] protected ControllerType baseControllerType;

    public Queue<GameObject> RootQueue { get; set; }

    public Vector3 MoveDirection { get; protected set; }

    protected Vector3 forward = Vector3.forward;
    protected Vector3 right = Vector3.right;
    public Vector3 Forward
    {
        get => forward;
        protected set
        {
            OnAim?.Invoke(value);
            forward = value;
            right.x = forward.z;
            right.y = forward.y;
            right.z = -forward.x;
        }
    }
    public Vector3 Right
    {
        get => right;
        protected set
        {
            right = value;
            forward.x = -right.z;
            forward.y = right.y;
            forward.z = right.x;
        }
    }


    public int healthCurrent;
    public int healthMax;
    public int damageBase;

    public float criticalRate;
    public float ciriticalDamage;
    public float damageReducePercent;
    public float walkSpeedBase;
    public float runSpeedBase;

    public bool isRunning = false;
    public bool isInvincible = false;
}


public partial class CharacterBase // Initialize
{
    public void Initialize()
    {
        AddSocket(GetComponentsInChildren<SocketBase>());

        foreach (CharacterModuleBase currentModule in GetComponentsInChildren<CharacterModuleBase>())
        {
            currentModule.Attach(this);
        }

        Invoke(nameof(ModuleLoaded), 0.1f);

        GameManager.OnPhysicsUpdate -= UpdateMove;
        GameManager.OnPhysicsUpdate += UpdateMove;
    }

    public void Return2Pool()
    {
        GameManager.OnPhysicsUpdate -= UpdateMove;
    }
}


public partial class CharacterBase
{
    public virtual void ModuleLoaded()
    {
        OnModuleLoaded?.Invoke();
    }

    public virtual void PossessedBy(ControllerBase newController)
    {
        Controller = newController;
        OnOwnerChanged?.Invoke(Controller);
    }

    public virtual void UnPossessed()
    {
        Controller = null;
        OnOwnerChanged?.Invoke(Controller);
    }
    public virtual void AnimationPlay(AnimationType wantType)
    {
        AnimationPlay(wantType.ToString());
    }
    public virtual void AnimationPlay(string wantTrigger)
    {
        OnAnimationTrigger?.Invoke(wantTrigger);
    }
}


public partial class CharacterBase // Delegate
{
    public void UpdateMove(float deltaTime)
    {
        // Epsilon : 0의 근사값
        if (MoveDirection.sqrMagnitude > float.Epsilon)
            OnMove?.Invoke(this, MoveDirection * (isRunning ? runSpeedBase : walkSpeedBase), deltaTime);
    }
    public void Move(Vector3 direction) => MoveDirection = direction.HorizontalNormalize();
    public void Aim(Vector3 direction) => OnAim?.Invoke(direction);
    public void Attack(Vector3 direction, bool value) => OnAttack?.Invoke(direction, value);
    public void Jump() => OnJump?.Invoke();
    public void Reload() => OnReload?.Invoke();
    public void Run(bool value) => OnRun?.Invoke(value);
    public void Die() => OnDie?.Invoke();
    public void Interaction(GameObject target)
        => OnInteraction?.Invoke(target);

    public void SendDamage(ref int totalDamage, ref float multiplier, ref bool isCritical)
        => OnSendDamage?.Invoke(ref totalDamage, ref multiplier, ref isCritical);

    public int GetDamage(int damage, Vector3 direction, bool isCritical, GameObject causer)
        => OnGetDamage?.Invoke(damage, direction, isCritical, causer) ?? 0;

    public SocketBase GetSocket()
    {
        SocketBase result = null;
        OnGetSocket?.Invoke(ref result);
        return result;
    }

    public SocketBase GetSocket(SocketType wantType)
    {
        SocketBase result = null;
        OnGetSocketByType?.Invoke(ref result, wantType);
        return result;
    }

    public SocketBase GetSocket(Func<SocketBase, bool> predicate)
    {
        SocketBase result = null;
        OnGetSocketByPredicate?.Invoke(ref result, predicate);
        return result;
    }

    public void GetSockets(List<SocketBase> result)
        => OnGetSockets?.Invoke(result);


    public void GetSockets(List<SocketBase> result, SocketType wantType)
        => OnGetSocketsByType?.Invoke(result, wantType);


    public void GetSockets(List<SocketBase> result, Func<SocketBase, bool> predicate)
        => OnGetSocketsByPredicate?.Invoke(result, predicate);

    public void AddSocket(SocketBase target)
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

    public void AddSocket(params SocketBase[] target)
    {
        foreach (SocketBase current in target) AddSocket(current);
    }

    public void RemoveSocket(SocketBase target)
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


    public void RemoveSocket(Func<SocketBase, bool> predicate)
    {
        foreach (SocketBase current in GetSockets(predicate))
            RemoveSocket(current);
    }


    public void SocketAction(Action<SocketBase> wantAction)
        => OnSocketAction?.Invoke(wantAction);
    public void SocketActionByType(SocketType wantType, Action<SocketBase> wantAction)
        => OnSocketActionByType?.Invoke(wantType, wantAction);

    public void SocketActionByPredicate(Func<SocketBase, bool> predicate, Action<SocketBase> wantAction)
        => OnSocketActionByPredicate?.Invoke(predicate, wantAction);


}
public partial class CharacterBase // Property
{
    public void AddRotation(float yaw, float pitch)
    {
        //Forward = Forward.RotationHorizontal(yaw);
        //Forward = Forward.RotationVertical(pitch);
        Forward = Forward.RotationVerticalClamped(yaw, pitch);
        MoveDirection = MoveDirection.RotationHorizontal(yaw);
        //AddRotation(Quaternion.Euler(pitch, yaw, 0));
    }
    public void AddRotationWithoutChangeDirection(float yaw, float pitch)
    {
        Forward = Forward.RotationVerticalClamped(yaw, pitch);
    }
    public void AddRotation(Quaternion rotation)
    {
        //Quaternion currentRotation = Quaternion.LookRotation(Forward);
        //// Quaternion sum = rotation * currentRotation; // 두 쿼터니온 회전을 하나로 합칠 때에는 곱해야함.
        //Vector3 euler = currentRotation.eulerAngles + rotation.eulerAngles;
        //euler.x = euler.x.ClampAngle(-89.9f, 89.9f);
        //rotation = Quaternion.Euler(euler);

        SetRotation(Quaternion.LookRotation(Forward).Add(rotation));
    }

    public virtual int GetSpareAmmo(AmmoType wantType) => 200;
    public virtual void AddSpareAmmo(AmmoType wantType, int delta) { }
    public virtual void SetSpareAmmo(AmmoType wantType, int amount) { }


    public SocketBase[] GetSockets()
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSockets?.Invoke(result);
        return result.ToArray();
    }

    public SocketBase[] GetSockets(SocketType wantType)
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSocketsByType?.Invoke(result, wantType);
        return result.ToArray();
    }
    public SocketBase[] GetSockets(Func<SocketBase, bool> predicate)
    {
        if (OnGetSockets is null) return null;
        List<SocketBase> result = new();
        OnGetSocketsByPredicate?.Invoke(result, predicate);
        return result.ToArray();
    }
    public void SetRotation(float yaw, float pitch) => SetRotation(Quaternion.Euler(pitch, yaw, 0));

    public void SetRotation(Vector3 wantforward) => Forward = wantforward;

    public void SetRotation(Quaternion rotation) => Forward = rotation * Vector3.forward;
}