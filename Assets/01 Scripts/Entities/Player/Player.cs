using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PublicDefines;

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
    private static readonly float AllowInputTime1HAttackSliceHorizontal = 0.3f;
    private static readonly float AnimLen1HAttackSliceHorizontal = 0.5f;

    private CharacterController _characterController;
    private Transform _weaponslotRight;
    private Transform _spineTransform;
    [SerializeField] private WeaponBase _currentWeapon;

    private Vector2 _inputVec;
    private Vector3 _lookDir;
    [SerializeField] private Vector3 _attackPointOffset = new Vector3(0.0f, 1.0f, 1.0f);
    [SerializeField] private Vector3 _attackRange = new Vector3(1.0f, 1.0f, 1.5f);

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
        _weaponslotRight = transform.Find(TPWeaponslotRight);
        _spineTransform = transform.Find(TPSpineTransform);

        // TEMP: 임시로 무기 장착중, 인벤토리 시스템 구현 후 완전 교체 예정
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

    // 디버그 렌더링 필요하면 구현, 아직 리팩토링 필요한 기능임
    void AttackOneHand() {
        Debug.Log("Attack");
        Collider[] enemies = Physics.OverlapBox(
            transform.position + (_lookDir + transform.rotation * _attackPointOffset),
            _attackRange / 2.0f, 
            Quaternion.identity,
            LayerMask.GetMask("Enemies"));

        foreach (Collider c in enemies) {
            Debug.Log(c.name);
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

    private void PickingGroundPoint() {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float rayDistance)) {
            _lookDir = (ray.GetPoint(rayDistance) - transform.position).normalized;
        }
    }

    // SetState~ Methods returns true when it changes state
    public bool SetStateBase(PlayerStateBase state) {
        if (_currentStateBase == state) return false;

        _currentStateBase = state;
        _stateClassesBase[_currentStateBase].Start();
        return true;
    }

    public bool SetStateCombat(PlayerStateCombat state, bool forced = false) {
        if (_currentStateCombat == state && !forced) return false;

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
                _owner.PickingGroundPoint();

                _owner.SetStateCombat(PlayerStateCombat.Attack);
                return;
            }
        }
    }

    private class AttackState : StateBase<Player> {
        public AttackState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AH1HAttackSliceHorizontal, 0.1f, 1, 0.0f);
        }

        public override void Update() {
            AnimatorStateInfo stateInfo = _owner._animator.GetCurrentAnimatorStateInfo(1);

            if (Mouse.current.leftButton.wasPressedThisFrame) {
                if (stateInfo.normalizedTime >= AllowInputTime1HAttackSliceHorizontal) {
                    _owner.PickingGroundPoint();

                    _owner.SetStateCombat(PlayerStateCombat.Attack, true);
                    return;
                }
            }

            if (!_owner._animator.IsInTransition(1)) {
                if (stateInfo.normalizedTime >= AnimLen1HAttackSliceHorizontal) {
                    _owner.SetStateCombat(PlayerStateCombat.Idle);
                    return;
                }
            }
        }
    }
}
