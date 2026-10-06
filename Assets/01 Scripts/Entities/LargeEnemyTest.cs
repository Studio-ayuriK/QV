using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemyTest : SkeletalEntity {
    public enum LargeEnemyState {
        Idle,
        Tracking
    }

    // Private Field
    private static readonly int AnimHashIdle = Animator.StringToHash("Idle");
    private static readonly int AnimHashWalk = Animator.StringToHash("Walking");

    private CharacterController _characterController;

    private LargeEnemyState _currentState;
    private Dictionary<LargeEnemyState, StateBase<LargeEnemyTest>> _stateClasses = new();

    private Transform _playerTransform;

    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _currentState = LargeEnemyState.Idle;
        _stateClasses.Add(LargeEnemyState.Idle, new IdleState(this));
        _stateClasses.Add(LargeEnemyState.Tracking, new TrackState(this));

        _playerTransform = FindAnyObjectByType<Player>().transform;
    }

    void Update() {
        if ((_playerTransform.position - transform.position).magnitude > 2.0f) {
            // _velocity = _moveDir * moveSpeed;

            // Ground Check
            //if (IsOnGround()) {
            //    _velocity.y = 0.0f;
            //}
            //else {
            //    RaycastHit hit;
            //    if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f, LayerMask.GetMask("Ground"))) {
            //        _velocity.y = -5.0f;
            //    }
            //}

            if (_currentState != LargeEnemyState.Tracking) {
                SetState(LargeEnemyState.Tracking);
            }

            _targetRotation = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }
        else {
            if (_currentState != LargeEnemyState.Idle) {
                SetState(LargeEnemyState.Idle);
            }
        }

        _stateClasses[_currentState].Update();
    }

    public void SetState(LargeEnemyState state) {
        if (_currentState == state) return;

        _currentState = state;
        _stateClasses[_currentState].Start();
    }

    private class IdleState : StateBase<LargeEnemyTest> {
        public IdleState(LargeEnemyTest owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AnimHashIdle, 0.1f);
        }
    }

    private class TrackState : StateBase<LargeEnemyTest> {
        public TrackState(LargeEnemyTest owner) : base(owner) { }

        public override void Start() {
            _owner._animator.CrossFade(AnimHashWalk, 0.1f);
        }

        public override void Update() {
            Transform ownerTransform = _owner.transform;
            _owner._moveDir = (_owner._playerTransform.position - ownerTransform.position).normalized;
            _owner._velocity = _owner._moveDir * _owner.moveSpeed;

            RaycastHit hit;
            if (Physics.Raycast(ownerTransform.position, Vector3.down, out hit, 2.5f, LayerMask.GetMask("Ground"))) {
                _owner._velocity.y = -5.0f;
            }

            _owner._characterController.Move(_owner._velocity * Time.deltaTime);
        }
    }
}
