using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

namespace shootstar
{
    public class PlayerStateListener : CharacterAbility
    {
        //[Header("绑定玩家控制器")]
        //private CorgiController _controller;
        //private Health _health;

        protected const string _HurtParameterName = "Hurt";
        protected int _HurtParameter;   
        // 上一帧的状态缓存
        private bool _wasGrounded;
        private bool _wasJumping;
        private bool _wasFalling;

        // 防抖标志（防止重复触发）
        private bool hasLanded;
        private bool hasStartedJump;
        private bool hasStartedFall;

        // 受伤冷却（避免连续伤害时动画被刷爆）
        [Header("受伤触发间隔")]
        [SerializeField] private float hurtCooldown = 0.4f;
        private float lastHurtTime = -10f;

        [SerializeField] private UnityEvent OnJump;
        [SerializeField] private UnityEvent OnFall;
        [SerializeField] private UnityEvent OnLand;
        [SerializeField] private UnityEvent OnHurt;
        [SerializeField] private UnityEvent OnDie;
        [SerializeField] private AudioSource HurtAudio;
        [SerializeField] private AudioSource DeathAudio;

        protected override void Initialization()
        {
            base.Initialization();
            _controller = _character.GetComponent<CorgiController>();
        }
       protected override void Start()
        {
            base.Start();
            //_controller = GetComponent<CorgiController>();
            //if (_controller == null)
            //{
            //    Debug.LogError("❌ 没有找到 CorgiController，请挂在角色对象上！");
            //    enabled = false;
            //    return;
            //}
            _controller = GetComponent<CorgiController>();
            _health = GetComponent<Health>();

            if (_controller == null)
            {
                Debug.LogError("❌ 没有找到 CorgiController，请挂在角色对象上！");
                enabled = false;
                return;
            }

            if (_health == null)
            {
                Debug.LogError("❌ 没有找到 Health 组件，请确认玩家对象上挂有 Health！");
                enabled = false;
                return;
            }

            // ✅ 绑定受伤与死亡事件
            _health.OnHit += OnPlayerHurt;
            _health.OnDeath += OnPlayerDeath;
        }
        void OnDestroy()
        {
            // 取消事件绑定，防止报错
            if (_health != null)
            {
                _health.OnHit -= OnPlayerHurt;
                _health.OnDeath -= OnPlayerDeath;
            }
        }
        void Update()
        {
            var state = _controller.State;

            // === 起跳检测 ===
            if (!state.IsGrounded && state.IsJumping && !hasStartedJump)
            {
                hasStartedJump = true;
                hasStartedFall = false; // 重置其他标志
                hasLanded = false;

                Debug.Log("起跳事件触发");
                OnJump?.Invoke();
            }

            // === 下落检测 ===（离开地面且不是跳跃状态）
            if (!state.IsGrounded && !state.IsJumping && !hasStartedFall)
            {
                hasStartedFall = true;
                hasStartedJump = false;
                hasLanded = false;

                Debug.Log("下落事件触发");
                OnFall?.Invoke();
            }

            // === 落地检测 ===（上一帧在空中，这一帧接触地面）
            if (state.IsGrounded && !hasLanded)
            {
                hasLanded = true;
                hasStartedJump = false;
                hasStartedFall = false;

                Debug.Log("落地事件触发");
                OnLand?.Invoke();
            }

            // === 更新状态缓存（用于 Debug 可视化） ===
            _wasGrounded = state.IsGrounded;
            _wasJumping = state.IsJumping;
            _wasFalling = state.IsFalling;
        }
        /// <summary>
        /// 每次玩家受伤时触发（可用于播放受伤动画、闪红、震动等）
        /// </summary>
        private void OnPlayerHurt()
        {
            if (Time.time - lastHurtTime < hurtCooldown)
                return; // 防止连续触发太频繁

            lastHurtTime = Time.time;

            HurtAudio.Play();
            Debug.Log("💢 玩家受伤事件触发");
            OnHurt?.Invoke();
           _animator.SetTrigger("Hurt");
        }

        /// <summary>
        /// 玩家死亡时触发（血量归零）
        /// </summary>
        private void OnPlayerDeath()
        {
            DeathAudio.Play();
            Debug.Log("💀 玩家死亡事件触发");
            OnDie?.Invoke();
            eventManager.Instance.OnDie?.Invoke();
            _animator.SetTrigger("Death");
        }
        //protected override void InitializeAnimatorParameters()
        //{
        //    RegisterAnimatorParameter(_HurtParameterName, AnimatorControllerParameterType.Trigger, out _HurtParameter);
        //}

        //public override void UpdateAnimator()
        //{
        //    MMAnimatorExtensions.UpdateAnimatorTrigger(
        //        _animator,
        //        _HurtParameter,
        //        (_movement.CurrentState == CharacterStates.MovementStates.Hurt),
        //        _character._animatorParameters

        //        );
        //}
    }
}