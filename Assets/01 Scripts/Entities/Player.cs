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

    public enum MoveDirection {
        Forward,
        Backward,
        Right,
        Left
    }


    //==================== Private Field
    private static readonly int AHIdle = Animator.StringToHash("Idle");
    private static readonly int AHRun = Animator.StringToHash("Running");
    private static readonly int AHWalkBack = Animator.StringToHash("Walking_Backward");
    private static readonly int AHRunRight = Animator.StringToHash("Running_Right");
    private static readonly int AHRunLeft = Animator.StringToHash("Running_Left");
    private static readonly int AH1HAttackSliceDiagonal = Animator.StringToHash("1H_Attack_Slice_Diagonal");
    private static readonly int AH1HAttackSliceHorizontal = Animator.StringToHash("1H_Attack_Slice_Horizontal");

    private static readonly string WeaponslotRightPath = "Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/weaponslot.r";
    private static readonly string SpineTransformPath = "Rig_Medium/root/hips/spine";

    private static readonly float NomrmalizedTime1HAttackSliceHorizontal = 0.5f;

    private CharacterController _characterController;
    private Transform _weaponslotRight;
    private Transform _spineTransform;
    [SerializeField] private WeaponBase _currentWeapon;

    private Vector2 _inputVec;
    private Vector3 _lookDir;

    private PlayerStateBase _currentStateBase;
    private PlayerStateCombat _currentStateCombat;
    private MoveDirection _moveDir;

    private Quaternion _upperRotationDiff;
    [SerializeField] private float _rotationThreshhold = 80.0f;

    private Dictionary<PlayerStateBase, StateBase<Player>> _stateClassesBase = new();
    private Dictionary<PlayerStateCombat, StateBase<Player>> _stateClassesCombat = new();


    private float rayLength = 0.1f;


    //==================== Unity Internal Method
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponslotRight = transform.Find(WeaponslotRightPath);
        _spineTransform = transform.Find(SpineTransformPath);
        SetWeapone((Instantiate(Resources.Load("Prefabs/Weapons/sword_1handed")) as GameObject).GetComponent<WeaponBase>());

        _currentStateBase = PlayerStateBase.Idle;
        _currentStateCombat = PlayerStateCombat.Idle;
        _moveDir = MoveDirection.Forward;

        _stateClassesBase.Add(PlayerStateBase.Idle, new IdleState(this));
        _stateClassesBase.Add(PlayerStateBase.Run, new RunState(this));

        _stateClassesCombat.Add(PlayerStateCombat.Idle, new IdleStateCombat(this));
        _stateClassesCombat.Add(PlayerStateCombat.Attack, new AttackState(this));
    }

    void Update() {
        _stateClassesBase[_currentStateBase].Update();
        _stateClassesCombat[_currentStateCombat].Update();
    }

    void LateUpdate() {
        if (_currentStateCombat != PlayerStateCombat.Idle) {
            _spineTransform.rotation *= _upperRotationDiff;
        }
    }


    //==================== Input Action
    void OnMove(InputValue value) {
        _inputVec = value.Get<Vector2>().normalized * moveSpeed;
        Vector3 forward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z).normalized;
        Vector3 right = new Vector3(Camera.main.transform.right.x, 0.0f, Camera.main.transform.right.z).normalized;

        _moveDirVec = (right * _inputVec.x + forward * _inputVec.y).normalized;
    }

    void OnInteract() {
        Manager.Interact.Interact();
    }

    void OnClick() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            // SetState(PlayerState.Attack);
        }
    }


    void AttackOneHand() {
        Debug.Log("Attack");
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

    // SetState~ Methods returns true when it changes state
    public bool SetStateBase(PlayerStateBase state) {
        if (_currentStateBase == state) return false;

        _currentStateBase = state;
        _stateClassesBase[_currentStateBase].Start();
        return true;
    }

    public bool SetStateCombat(PlayerStateCombat state) {
        if (_currentStateCombat == state) return false;

        _currentStateCombat = state;
        _stateClassesCombat[_currentStateCombat].Start();
        return true;
    }

    public bool SetMoveDirection(MoveDirection direction) {
        if (_moveDir == direction) return false;

        _moveDir = direction;
        return true;
    }


    //==================== State Classes
    //=================================== Base States
    private class IdleState : StateBase<Player> {
        public IdleState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHIdle, 0.1f);
        }

        public override void Update() {
            if (_owner._moveDirVec.magnitude > 0.0f) {
                _owner.SetStateBase(PlayerStateBase.Run);
                return;
            }

            if (_owner._currentStateCombat != PlayerStateCombat.Idle) {
                _owner._targetRotation = Quaternion.LookRotation(_owner._lookDir);

                _owner.transform.rotation = Quaternion.Slerp(
                        _owner.transform.rotation,
                        _owner._targetRotation,
                        _owner.rotationSpeed * Time.deltaTime);
            }
        }
    }

    private class RunState : StateBase<Player> {
        public RunState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AHRun, 0.1f);
        }

        public override void Update() {
            if (_owner._moveDirVec.magnitude <= 0.0f) {
                _owner.SetStateBase(PlayerStateBase.Idle);
                return;
            }

            _owner._velocity = _owner._moveDirVec * _owner.moveSpeed;

            // Ground Check
            if (_owner.IsOnGround()) {
                _owner._velocity.y = 0.0f;
            }
            else {
                RaycastHit hit;
                if (Physics.Raycast(_owner.transform.position, Vector3.down, out hit, 2.5f, LayerMask.GetMask("Ground"))) {
                    _owner._velocity.y = -5.0f;     // Hard coded, refactoring if it has to
                }
            }

            // Actual Move
            _owner._characterController.Move(_owner._velocity * Time.deltaTime);

            // None Combat -> just move forward
            if (_owner._currentStateCombat == PlayerStateCombat.Idle) {
                if(_owner.SetMoveDirection(MoveDirection.Forward))
                    _owner._animator.CrossFade(AHRun, 0.1f);

                _owner._targetRotation = Quaternion.LookRotation(_owner._moveDirVec);
            }
            else {
                CheckRotation();
            }

            _owner.transform.rotation =  Quaternion.Slerp(
                    _owner.transform.rotation, 
                    _owner._targetRotation, 
                    _owner.rotationSpeed * Time.deltaTime);
        }

        private void CheckRotation() {
            float theta = Vector3.Angle(_owner._lookDir, _owner._moveDirVec);

            MoveDirection moveDir = MoveDirection.Forward;
            if (theta > _owner._rotationThreshhold)
                moveDir = MoveDirection.Backward;

            if (_owner.SetMoveDirection(moveDir)) {
                switch (moveDir) {
                    case MoveDirection.Forward:
                        _owner._animator.CrossFade(AHRun, 0.1f);
                        break;
                    case MoveDirection.Backward:
                        _owner._animator.CrossFade(AHWalkBack, 0.1f);
                        break;
                }
            }

            switch (moveDir) {
                case MoveDirection.Forward:
                    _owner._upperRotationDiff = Quaternion.FromToRotation(_owner._moveDirVec, _owner._lookDir);
                    _owner._targetRotation = Quaternion.LookRotation(_owner._moveDirVec);
                    break;
                case MoveDirection.Backward:
                    _owner._upperRotationDiff = Quaternion.FromToRotation(-_owner._moveDirVec, _owner._lookDir);
                    _owner._targetRotation = Quaternion.LookRotation(-_owner._moveDirVec);
                    break;
            }
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
                Plane plane = new Plane(Vector3.up, _owner.transform.position);
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (plane.Raycast(ray, out float rayDistance)) {
                    _owner._lookDir = ray.GetPoint(rayDistance) - _owner.transform.position;
                }

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
                if (stateInfo.normalizedTime >= NomrmalizedTime1HAttackSliceHorizontal) {
                    _owner.SetStateCombat(PlayerStateCombat.Idle);
                    return;
                }
            }
        }
    }
}
