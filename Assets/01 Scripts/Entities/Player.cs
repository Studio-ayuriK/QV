using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : SkeletalEntity {
    public enum PlayerState {
        Idle,
        Run,
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

    private PlayerState _currentState;
    private Dictionary<PlayerState, StateBase<Player>> _stateClasses = new();

    private float rayLength = 0.1f;


    //==================== Unity Internal Method
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponslotRight = transform.Find(WeaponslotRightPath);
        SetWeapone((Instantiate(Resources.Load("Prefabs/Weapons/sword_1handed")) as GameObject).GetComponent<WeaponBase>());

        _currentState = PlayerState.Idle;
        _stateClasses.Add(PlayerState.Idle, new IdleState(this));
        _stateClasses.Add(PlayerState.Run, new RunState(this));
        _stateClasses.Add(PlayerState.Attack, new AttackState(this));
    }

    void Update() {
        _stateClasses[_currentState].Update();
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

    public void SetState(PlayerState state) {
        if (_currentState == state) return;

        _currentState = state;
        _stateClasses[_currentState].Start();
    }


    //==================== State Classes
    private class IdleState : StateBase<Player> {
        public IdleState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHIdle, 0.1f);
            Debug.Log("Idle");
        }

        public override void Update() {
            if (_owner._moveDir.magnitude > 0.0f) {
                _owner.SetState(PlayerState.Run);
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame) {
                _owner.SetState(PlayerState.Attack);
                return;
            }
        }
    }

    private class RunState : StateBase<Player> {
        public RunState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHRun, 0.1f);
            Debug.Log("Run");
        }

        public override void Update() {
            if (_owner._moveDir.magnitude <= 0.0f) {
                _owner.SetState(PlayerState.Idle);
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame) {
                _owner.SetState(PlayerState.Attack);
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

    private class AttackState : StateBase<Player> {
        public AttackState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AH1HAttackSliceHorizontal, 0.1f);
            Debug.Log("Attack");
        }

        public override void Update() {
            AnimatorStateInfo stateInfo = _owner._animator.GetCurrentAnimatorStateInfo(0);

            if (!_owner._animator.IsInTransition(0)) {
                if (stateInfo.normalizedTime >= 0.5 && _owner._moveDir.magnitude > 0.0f) {
                    _owner.SetState(PlayerState.Run);
                }

                if (stateInfo.normalizedTime >= 1.0f) {
                    _owner.SetState(PlayerState.Idle);
                }
            }
        }
    }
}
