using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : SkeletalEntity {
    public enum PlayerState {
        Idle,
        Run
    }

    // Private Field
    private static readonly int AnimHashIdle = Animator.StringToHash("Idle");
    private static readonly int AnimHashRun = Animator.StringToHash("Running");
    private static readonly string WeaponslotRightPath = "Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/weaponslot.r";

    private CharacterController _characterController;
    private Transform _weaponslotRight;
    [SerializeField] private WeaponBase _currentWeapon;

    private Vector2 _inputVec;

    private PlayerState _currentState;
    private Dictionary<PlayerState, StateBase<Player>> _stateClasses = new();

    private float rayLength = 0.1f;

    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponslotRight = transform.Find(WeaponslotRightPath);
        SetWeapone((Instantiate(Resources.Load("Prefabs/Weapons/sword_1handed")) as GameObject).GetComponent<WeaponBase>());

        _currentState = PlayerState.Idle;
        _stateClasses.Add(PlayerState.Idle, new IdleState(this));
        _stateClasses.Add(PlayerState.Run, new RunState(this));
    }

    void Update() {
        if (_moveDir.magnitude > 0.0f) {
            _velocity = _moveDir * moveSpeed;

            // Ground Check
            if (IsOnGround()) {
                _velocity.y = 0.0f;
            }
            else {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f, LayerMask.GetMask("Ground"))) {
                    _velocity.y = -5.0f;
                }
            }

            _characterController.Move(_velocity * Time.deltaTime);

            if (_currentState != PlayerState.Run) {
                SetState(PlayerState.Run);
            }

            _targetRotation = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }
        else {
            if (_currentState != PlayerState.Idle) {
                SetState(PlayerState.Idle);
            }
        }

        _stateClasses[_currentState].Update();
    }

    void OnMove(InputValue value) {
        _inputVec = value.Get<Vector2>().normalized * moveSpeed;
        Vector3 forward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z).normalized;
        Vector3 right = new Vector3(Camera.main.transform.right.x, 0.0f, Camera.main.transform.right.z).normalized;

        _moveDir = (right * _inputVec.x + forward * _inputVec.y).normalized;
    }

    void OnInteract() {
        Manager.Interact.Interact();
    }

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

    private class IdleState : StateBase<Player> {
        public IdleState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AnimHashIdle, 0.1f);
        }

        public override void Update() {

        }
    }

    private class RunState : StateBase<Player> {
        public RunState(Player owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AnimHashRun, 0.1f);
        }

        public override void Update() {

        }
    }
}
