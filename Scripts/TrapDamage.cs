
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;


namespace shootstar
{
    public class TrapDamage : MonoBehaviour
    {
        [Header("Trap Settings")]
        public int DamageAmount = 10;          // 每次伤害量
        public float DamageInterval = 1f;      // 伤害间隔
        public float StopDelay = 1f;           // 离开后延时停止伤害

        [Header("Control")]
        public bool CanDamage = true;          // 是否启用陷阱

        [Header("Feedback")]
        public MMFeedbacks DamageFeedbacks;        // 受伤反馈
        public MMFeedbacks TrapActivateFeedbacks;  // 启动反馈

        private Coroutine _damageCoroutine;        // 持续伤害协程
        private Coroutine _delayedStopCoroutine;   // 延迟停止协程
        private bool _playerInside = false;
        private Health _playerHealth;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!CanDamage || !isActiveAndEnabled) return;

            if (other.CompareTag("Player"))
            {
                _playerInside = true;
                _playerHealth = other.GetComponent<Health>();

                // 播放陷阱启动反馈
                TrapActivateFeedbacks?.PlayFeedbacks();

                // 如果之前有延时停止协程，先取消
                if (_delayedStopCoroutine != null)
                {
                    StopCoroutine(_delayedStopCoroutine);
                    _delayedStopCoroutine = null;
                }

                // 开始持续伤害
                if (_damageCoroutine == null && _playerHealth != null)
                {
                    _damageCoroutine = StartCoroutine(DamageLoop());
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!CanDamage || !isActiveAndEnabled) return;

            if (other.CompareTag("Player"))
            {
                _playerInside = false;

                // 延迟停止伤害循环
                if (_delayedStopCoroutine != null)
                {
                    StopCoroutine(_delayedStopCoroutine);
                }
                _delayedStopCoroutine = StartCoroutine(DelayedStop());
            }
        }

        private IEnumerator DamageLoop()
        {
            while (_playerInside && CanDamage && _playerHealth != null && isActiveAndEnabled)
            {
                ApplyTrapDamage();
                yield return new WaitForSeconds(DamageInterval);
            }

            _damageCoroutine = null;
        }

        private IEnumerator DelayedStop()
        {
            yield return new WaitForSeconds(StopDelay);

            if (!_playerInside && _damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }

            _delayedStopCoroutine = null;
        }

        private void ApplyTrapDamage()
        {
            if (_playerHealth == null || !_playerHealth.CanTakeDamageThisFrame()) return;

            _playerHealth.Damage(
                DamageAmount,
                this.gameObject,
                DamageInterval, // invincibilityDuration
                DamageInterval,   // flickerDuration
                Vector3.zero
            );

            DamageFeedbacks?.PlayFeedbacks();
        }

        /// <summary>
        /// 外部控制陷阱开关
        /// </summary>
        public void SetTrapActive(bool active)
        {
            CanDamage = active;

            if (!active)
            {
                // 停止所有协程，防止物体被禁用时报错
                if (_damageCoroutine != null)
                {
                    StopCoroutine(_damageCoroutine);
                    _damageCoroutine = null;
                }

                if (_delayedStopCoroutine != null)
                {
                    StopCoroutine(_delayedStopCoroutine);
                    _delayedStopCoroutine = null;
                }

                _playerInside = false;
            }
        }

        private void OnDisable()
        {
            // 确保物体被禁用时安全停止所有协程
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }

            if (_delayedStopCoroutine != null)
            {
                StopCoroutine(_delayedStopCoroutine);
                _delayedStopCoroutine = null;
            }

            _playerInside = false;
        }
    }
}
