using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DelegateCharacterMove(CharacterBase mover, Vector3 velocity, float deltaTime);
public delegate void DelegateCharacterSpeedChange(Vector3 velocity);
public delegate void DelegateCharacterAim(Vector3 direction);
public delegate void DelegateCharacterAttack(Vector3 direction, bool value);
public delegate void DelegateCharacterJump();

public delegate void DelegateCharacterWeapon(int index);
public delegate void DelegateCharacterChangeWeapon(float value);

public delegate void DelegateCharacterReload();
public delegate void DelegateCharacterRun(bool value);
public delegate void DelegateCharacterDie();
public delegate void DelegateCharacterInteraction(GameObject target);
public delegate void DelegateCharacterOwnerChanged(ControllerBase newController);

public delegate int DelegateCharacterGetDamage(float damage, Vector3 direction, CharacterPartType partType, bool isCritical, GameObject causer);
public delegate void DelegateCharacterCalculateDamage(ref float totalDamage, CharacterPartType partType, ref float multiplier, ref bool isCritical);
public delegate void DelegatCharacterAnimaionPlay(AnimationType wanType);
public delegate void DelegatCharacterAnimaionTrigger(string wantTrigger);

public partial class CharacterBase : SocketMonoBehaviour, IPoolable // Delegate
{
    public event Action OnModuleLoaded;
    public event DelegateCharacterSpeedChange OnSpeedChanged;
    public event DelegateCharacterMove OnMove;
    public event DelegateCharacterAim OnAim;
    public event DelegateCharacterAttack OnAttack;
    public event DelegateCharacterJump OnJump;

    public event DelegateCharacterWeapon OnWeapon;
    public event DelegateCharacterChangeWeapon OnChangeWeapon;

    public event DelegateCharacterReload OnReload;
    public event DelegateCharacterReload OnReloadComplete;
    public event DelegateCharacterRun OnRun;
    public event DelegateCharacterDie OnDie;
    public event DelegateCharacterInteraction OnInteraction;

    public event DelegateCharacterGetDamage OnGetDamage;
    public event DelegateCharacterCalculateDamage OnCalculateDamage;

    public event DelegateCharacterOwnerChanged OnOwnerChanged;

    public event DelegatCharacterAnimaionTrigger OnAnimationTrigger;
}

public partial class CharacterBase// Data Field
{
    public ControllerBase Controller { get; protected set; }

    // ai이거나 직접 배치한 경우 생성된 Controller , 기본적으로 몬스터, 테스트 시 직접 참조할 캐릭터
    public ControllerType BaseControllerType => baseControllerType;
    [SerializeField] protected ControllerType baseControllerType;

    public Queue<GameObject> RootQueue { get; set; }

    public Vector3 MoveVelocity { get; protected set; }
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


    public float healthCurrent;
    public float healthMax;
    public float damageBase;

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
        Vector3 resultVelocity = Vector3.zero;
        // Epsilon : 0의 근사값
        if (MoveDirection.sqrMagnitude > float.Epsilon)
        {
            Vector3 originPosition = transform.position;
            OnMove?.Invoke(this, MoveDirection * (isRunning ? runSpeedBase : walkSpeedBase), deltaTime);
            // 목적지 - 출발지 : 이동한 방향
            Vector3 distance = (transform.position - originPosition) / deltaTime;
            float speed = distance.magnitude;
            // 벡터 투영
            //float speedForward = Vector3.Project(distance, Forward).magnitude;
            float speedForward = Vector3.Dot(distance, Forward);
            float speedRight = Vector3.Dot(distance, Right);
            resultVelocity.z = speedForward;
            resultVelocity.x = speedRight;

            resultVelocity = resultVelocity.normalized * speed;

            if (resultVelocity != MoveVelocity)
            {
                MoveVelocity = resultVelocity;
                OnSpeedChanged?.Invoke(MoveVelocity);
            }
        }
    }
    public void Move(Vector3 direction) => MoveDirection = direction.HorizontalNormalize();
    public void Aim(Vector3 direction) => OnAim?.Invoke(direction);
    public void Attack(Vector3 direction, bool value) => OnAttack?.Invoke(direction, value);
    public void Jump() => OnJump?.Invoke();
    public void Weapon(int index) => OnWeapon?.Invoke(index);
    public void ChangeWeapon(float value) => OnChangeWeapon?.Invoke(value);
    public void Reload() => OnReload?.Invoke();
    public void ReloadComplete() => OnReloadComplete?.Invoke();
    public void Run(bool value) => OnRun?.Invoke(value);
    public void Die()
    {
        AnimationPlay(AnimationType.Die);
        OnDie?.Invoke();
    }
    public void Interaction(GameObject target)
        => OnInteraction?.Invoke(target);

    public void CalculateDamage(ref float totalDamage, CharacterPartType partType, ref float multiplier, ref bool isCritical)
        => OnCalculateDamage?.Invoke(ref totalDamage, partType, ref multiplier, ref isCritical);

    public float GetDamage(float damage, Vector3 direction, CharacterPartType partType, bool isCritical, GameObject causer)
    {
        if (healthCurrent <= 0) return 0;

        healthCurrent -= damage;

        if (healthCurrent <= 0) Die();

        return OnGetDamage?.Invoke(damage, direction, partType, isCritical, causer) ?? 0;
    }
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

    public void SetRotation(float yaw, float pitch) => SetRotation(Quaternion.Euler(pitch, yaw, 0));

    public void SetRotation(Vector3 wantforward) => Forward = wantforward;

    public void SetRotation(Quaternion rotation) => Forward = rotation * Vector3.forward;
}