using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

public class Player : SkeletalEntity {
    public enum PlayerStateBase {
        Idle,
        Run
    }

    public enum PlayerStateCombat {
        Idle,
        Attack
    }


    //==================== Private Field
    private static readonly int AHIdle = Animator.StringToHash("Idle");
    private static readonly int AHRun = Animator.StringToHash("Running");
    private static readonly int AH1HAttackSliceDiagonal = Animator.StringToHash("1H_Attack_Slice_Diagonal");
    private static readonly int AH1HAttackSliceHorizontal = Animator.StringToHash("1H_Attack_Slice_Horizontal");

    private static readonly string WeaponslotRightPath = "Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/weaponslot.r";

    private CharacterController _characterController;
    private Transform _weaponslotRight;
    [SerializeField] private WeaponBase _currentWeapon;

    private Vector2 _inputVec;

    private PlayerStateBase _currentStateBase;
    private PlayerStateCombat _currentStateCombat;

    private Dictionary<PlayerStateBase, StateBase<Player>> _stateClassesBase = new();
    private Dictionary<PlayerStateCombat, StateBase<Player>> _stateClassesCombat = new();


    private float rayLength = 0.1f;


    //==================== Unity Internal Method
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponslotRight = transform.Find(WeaponslotRightPath);
        SetWeapone((Instantiate(Resources.Load("Prefabs/Weapons/sword_1handed")) as GameObject).GetComponent<WeaponBase>());

        _currentStateBase = PlayerStateBase.Idle;
        _currentStateCombat = PlayerStateCombat.Idle;

        _stateClassesBase.Add(PlayerStateBase.Idle, new IdleState(this));
        _stateClassesBase.Add(PlayerStateBase.Run, new RunState(this));

        _stateClassesCombat.Add(PlayerStateCombat.Idle, new IdleStateCombat(this));
        _stateClassesCombat.Add(PlayerStateCombat.Attack, new AttackState(this));
    }

    void Update() {
        _stateClassesBase[_currentStateBase].Update();
        _stateClassesCombat[_currentStateCombat].Update();
    }


    //==================== Input Action
    void OnMove(InputValue value) {
        _inputVec = value.Get<Vector2>().normalized * moveSpeed;
        Vector3 forward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z).normalized;
        Vector3 right = new Vector3(Camera.main.transform.right.x, 0.0f, Camera.main.transform.right.z).normalized;

        _moveDir = (right * _inputVec.x + forward * _inputVec.y).normalized;
    }

    void OnInteract() {
        Manager.Interact.Interact();
    }

    void OnClick() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            // SetState(PlayerState.Attack);
        }
    }


    //==================== Custom Method
    public void SetWeapone(WeaponBase weaponBase) {
        if (weaponBase == _currentWeapon) return;

        _currentWeapon = weaponBase;
        _currentWeapon.transform.SetParent(_weaponslotRight);
        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.transform.localRotation = Quaternion.identity;
    }

    private bool IsOnGround() {
        if (_characterController.isGrounded) return true;

        bool flag = Physics.Raycast(new Ray(transform.position, Vector3.down), rayLength, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector3.down, Color.red, rayLength);
        return flag;
    }

    public void SetStateBase(PlayerStateBase state) {
        if (_currentStateBase == state) return;

        _currentStateBase = state;
        _stateClassesBase[_currentStateBase].Start();
    }

    public void SetStateCombat(PlayerStateCombat state) {
        if (_currentStateCombat == state) return;

        _currentStateCombat = state;
        _stateClassesCombat[_currentStateCombat].Start();
    }


    //==================== State Classes
    //=================================== Base States
    private class IdleState : StateBase<Player> {
        public IdleState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHIdle, 0.1f);
        }

        public override void Update() {
            if (_owner._moveDir.magnitude > 0.0f) {
                _owner.SetStateBase(PlayerStateBase.Run);
                return;
            }
        }
    }

    private class RunState : StateBase<Player> {
        public RunState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHRun, 0.1f);
        }

        public override void Update() {
            if (_owner._moveDir.magnitude <= 0.0f) {
                _owner.SetStateBase(PlayerStateBase.Idle);
                return;
            }

            _owner._velocity = _owner._moveDir * _owner.moveSpeed;

            // Ground Check
            if (_owner.IsOnGround()) {
                _owner._velocity.y = 0.0f;
            }
            else {
                RaycastHit hit;
                if (Physics.Raycast(_owner.transform.position, Vector3.down, out hit, 2.5f, LayerMask.GetMask("Ground"))) {
                    _owner._velocity.y = -5.0f;
                }
            }

            _owner._characterController.Move(_owner._velocity * Time.deltaTime);

            _owner._targetRotation = Quaternion.LookRotation(_owner._moveDir);
            _owner.transform.rotation =  Quaternion.Slerp(
                    _owner.transform.rotation, 
                    _owner._targetRotation, 
                    _owner.rotationSpeed * Time.deltaTime);
        }
    }


    //=================================== Combat States
    private class IdleStateCombat : StateBase<Player> {
        public IdleStateCombat(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHIdle, 0.1f, 1);
        }

        public override void Update() {
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                _owner.SetStateCombat(PlayerStateCombat.Attack);
                return;
            }
        }
    }

    private class AttackState : StateBase<Player> {
        public AttackState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AH1HAttackSliceHorizontal, 0.1f, 1);
        }

        public override void Update() {
            AnimatorStateInfo stateInfo = _owner._animator.GetCurrentAnimatorStateInfo(1);

            if (!_owner._animator.IsInTransition(1)) {
                if (stateInfo.normalizedTime >= 0.5) {
                    _owner.SetStateCombat(PlayerStateCombat.Idle);
                    return;
                }
            }
        }
    }
}
