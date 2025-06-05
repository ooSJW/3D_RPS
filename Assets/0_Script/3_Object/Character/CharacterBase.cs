using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateCharacterMove(Vector3 direction);
public delegate void DelegateCharacterAim(Vector3 direction);
public delegate void DelegateCharacterAttack(Vector3 direction, bool value);
public delegate void DelegateCharacterJump();
public delegate void DelegateCharacterRun(bool value);
public delegate void DelegateCharacterDie();
public delegate void DelegateCharacterInteraction(GameObject target);

public delegate int DelegateCharacterGetDamage(int damage, Vector3 direction, bool isCritical, GameObject causer);
public delegate void DelegateCharacterSendDamage(ref int totalDamage, ref float multiplier, ref bool isCritical);


public partial class CharacterBase : MonoBehaviour, IPoolable // Delegate Field
{
    public event DelegateCharacterMove OnMove;
    public event DelegateCharacterAim OnAim;
    public event DelegateCharacterAttack OnAttack;
    public event DelegateCharacterJump OnJump;
    public event DelegateCharacterRun OnRun;
    public event DelegateCharacterDie OnDie;
    public event DelegateCharacterInteraction OnInteraction;

    public event DelegateCharacterGetDamage OnGetDamage;
    public event DelegateCharacterSendDamage OnSendDamage;

}

public partial class CharacterBase : MonoBehaviour, IPoolable // Data Field
{
    public ControllerBase Controller { get; protected set; }

    // ai이거나 직접 배치한 경우 생성된 Controller , 기본적으로 몬스터, 테스트 시 직접 참조할 캐릭터
    public ControllerType BaseControllerType => baseControllerType;
    [SerializeField] protected ControllerType baseControllerType;
    
    public Queue<GameObject> RootQueue { get; set; }

    protected Vector3 forward;
    protected Vector3 right;
    public Vector3 Forward
    {
        get => forward;
        protected set
        {
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
        foreach (CharacterModuleBase currentModule in GetComponents<CharacterModuleBase>())
        {
            currentModule.Attach(this);
        }

        //TODO TEST
        Forward = transform.forward;
    }

    public void Return2Pool()
    {

    }
}

public partial class CharacterBase
{
    public virtual void PossessedBy(ControllerBase newController)
    {
        Controller = newController;
    }

    public virtual void UnPossessed()
    {
        Controller = null;
    }
}


public partial class CharacterBase // Delegate Property
{
    public void Move(Vector3 direction) => OnMove?.Invoke(direction);
    public void Aim(Vector3 direction) => OnAim?.Invoke(direction);
    public void Attack(Vector3 direction, bool value) => OnAttack?.Invoke(direction, value);
    public void Jump() => OnJump?.Invoke();
    public void Run(bool value) => OnRun?.Invoke(value);
    public void Die() => OnDie?.Invoke();
    public void Interaction(GameObject target) => OnInteraction?.Invoke(target);

    public void SendDamage(ref int totalDamage, ref float multiplier, ref bool isCritical) => OnSendDamage?.Invoke(ref totalDamage, ref multiplier, ref isCritical);
    public int GetDamage(int damage, Vector3 direction, bool isCritical, GameObject causer) => OnGetDamage?.Invoke(damage, direction, isCritical, causer) ?? 0;

}