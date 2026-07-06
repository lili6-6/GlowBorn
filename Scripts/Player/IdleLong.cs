using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace shootstar
{
    public class IdleLong : CharacterAbility
    {
        [Header("Idle Long Settings")]
        public float IdleThreshold = 3f; // 静止多久触发待机
        public float SpeedThreshold = 0.05f; // 速度阈值
        public string[] IdleLongAnimationNames = { "IdleLong_1", "IdleLong_2", "IdleLong_3" };

        private float _idleTimer = 0f;
        private bool _isPlayingIdleLong = false;

        private string _currentIdleLongName;
        private int _currentIdleLongParameter;
        private Dictionary<string, int> _idleLongParameters = new Dictionary<string, int>();

        protected override void Initialization()
        {
            base.Initialization();
            _controller = _character.GetComponent<CorgiController>();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            float horizontalSpeed = Mathf.Abs(_controller.Speed.x);
            float verticalSpeed = Mathf.Abs(_controller.Speed.y);
            bool isMoving = (horizontalSpeed > SpeedThreshold || verticalSpeed > SpeedThreshold);

            // 一旦角色移动或跳起，立即中断IdleLong
            if (isMoving || !_controller.State.IsGrounded)
            {
                _idleTimer = 0f;
                if (_isPlayingIdleLong)
                {
                    StopIdleLong();
                }
                return;
            }

            // 累积静止时间
            _idleTimer += Time.deltaTime;

            // 触发待机动画
            if (!_isPlayingIdleLong && _idleTimer >= IdleThreshold)
            {
                PlayRandomIdleLong();
            }

            // 如果正在播放，检测是否播放完毕
            if (_isPlayingIdleLong && CheckIdleLongFinished())
            {
                StopIdleLong();
                _idleTimer = 0f;
            }
        }

        private void PlayRandomIdleLong()
        {
            if (IdleLongAnimationNames.Length == 0 || _animator == null)
                return;

            _currentIdleLongName = IdleLongAnimationNames[Random.Range(0, IdleLongAnimationNames.Length)];

            if (!_idleLongParameters.TryGetValue(_currentIdleLongName, out _currentIdleLongParameter))
            {
                Debug.LogWarning($"Animator未注册参数 {_currentIdleLongName}");
                return;
            }

            _animator.SetBool(_currentIdleLongParameter, true);
            _isPlayingIdleLong = true;
        }

        private bool CheckIdleLongFinished()
        {
            if (!_isPlayingIdleLong || _animator == null)
                return false;

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            // 这里不强制匹配Hash名，只要动画播放完就停止
            return stateInfo.normalizedTime >= 1f && stateInfo.loop == false;
        }

        private void StopIdleLong()
        {
            if (_isPlayingIdleLong && _idleLongParameters.ContainsKey(_currentIdleLongName))
            {
                _animator.SetBool(_idleLongParameters[_currentIdleLongName], false);
            }
            _isPlayingIdleLong = false;
        }

        protected override void InitializeAnimatorParameters()
        {
            foreach (string animName in IdleLongAnimationNames)
            {
                int paramHash;
                RegisterAnimatorParameter(animName, AnimatorControllerParameterType.Bool, out paramHash);
                _idleLongParameters[animName] = paramHash;
            }
        }

        public override void UpdateAnimator() { }
    }
}
