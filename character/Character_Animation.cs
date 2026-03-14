//using System.Collections;
//using System.Security.Cryptography;
//using Unity.Collections;
//using UnityEngine;
//using UnityEngine.InputSystem.XR.Haptics;

//namespace shootstar
//{
//    public class Character_Animation : MonoBehaviour
//    {
//        [SerializeField] private Animator targetAnimator;
//        private AnimatorControllerParameter[] Parameters;
//        private Character_base Character;
//        private Rigidbody2D rb;
//        Vector2 lastPos;
//        // Start is called once before the first execution of Update after the MonoBehaviour is created

//        void Awake()
//        {
//            Character = GetComponent<Character_base>();
//            rb = GetComponent<Rigidbody2D>();
//        }
//        void Start()
//        {
//            Parameters = targetAnimator.parameters;
//            Character.CurrentState =_CharacterStates.Idle;
//            ChangeAnimation();

//        }

//        // Update is called once per frame
//        void Update()
//        {
//            //float speedY = rb.linearVelocityY;
//            //targetAnimater.SetFloat("Jump", speedY);
//            //float speedX = rb.linearVelocityX;
//            //targetAnimater.SetFloat("Walk",speedX);
//            // 先计算速度
//            Vector2 currentPos = transform.position;
//            Vector2 velocity = (currentPos - lastPos) / Time.deltaTime;

//            // 更新动画
//            targetAnimator.SetFloat("Walk", Mathf.Abs(velocity.x));
//            targetAnimator.SetFloat("Jump", velocity.y);

//            // 记录
//            lastPos = currentPos;

//        }
//        public void ChangeAnimation(float? duration=null)
//        {
//            ResetAniamtion();

//            StartCoroutine( SetAnimation(duration));

//        }
//        public void ResetAniamtion()
//        {
//            foreach (var item in Parameters)
//            {
//                // 横向/纵向速度不准清理
//                if (item.name == "Walk" || item.name == "Jump")
//                {
//                    continue;
//                }


//                switch (item.type)
//                {
//                    case AnimatorControllerParameterType.Float:
//                        targetAnimator.SetFloat(item.name, 0);
//                        break;
//                    case AnimatorControllerParameterType.Int:
//                        targetAnimator.SetInteger(item.name, 0);
//                        break;
//                    case AnimatorControllerParameterType.Bool:
//                        targetAnimator.SetBool(item.name, false);
//                        break;
//                    case AnimatorControllerParameterType.Trigger:
//                        targetAnimator.ResetTrigger(item.name);
//                        break;
//                }
//            }
//        }



//        public IEnumerator SetAnimation(float? duration = null)
//        {
//            targetAnimator.SetBool(Character.CurrentState.ToString(), true);
//            targetAnimator.SetTrigger(Character.CurrentState.ToString());

//            // 如果 duration 有值并且大于 0，则等待
//            if (duration.HasValue && duration.Value > 0f)
//            {
//                yield return new WaitForSeconds(duration.Value);
//                ResetAniamtion();
//                targetAnimator.SetBool("Idle", true);
//            }
//        }

//    }
//}
using System.Collections;
using UnityEngine;

namespace shootstar
{
    // 确保枚举在命名空间内可访问（如果Character_base中已定义，可省略）
    

    public class Character_Animation : MonoBehaviour
    {
        [SerializeField] public Animator targetAnimator; // 修正拼写后的Animator引用
        private AnimatorControllerParameter[] Parameters; // Animator参数数组
        private Character_base Character; // 角色核心逻辑引用
        private Rigidbody2D rb; // 刚体引用
        [HideInInspector]public Vector2 lastPos; // 上一帧位置（计算速度用）

        // 防抖动阈值（避免微小位移触发走路动画）
        [Header("动画配置")]
        [SerializeField] public float speedThreshold = 0.01f;

        void Awake()
        {
            // 1. 安全获取组件（加空值校验）
            Character = GetComponent<Character_base>();
            if (Character == null)
            {
                Debug.LogError($"[{gameObject.name}] 未找到Character_base组件！", this);
            }

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError($"[{gameObject.name}] 未找到Rigidbody2D组件！", this);
            }

            // 2. 初始化lastPos（避免第一帧速度异常）
            lastPos = transform.position;
        }

        void Start()
        {
            // 3. 核心修复：targetAnimator空值校验（解决Parameters赋值空引用）
            if (targetAnimator == null)
            {
                Debug.LogError($"[{gameObject.name}] targetAnimator未赋值！", this);
                Parameters = new AnimatorControllerParameter[0]; // 初始化空数组，避免遍历报错
                return;
            }

            // 4. 赋值Animator参数数组
            Parameters = targetAnimator.parameters;

            // 5. 初始化状态（加空值校验）
            if (Character != null)
            {
                targetAnimator.SetBool("Alive", true); // 确保Alive状态开启
                Character.CurrentState = _CharacterStates.Idle;
                ChangeAnimation();
            }
        }

        void Update()
        {
            // 空值校验：Animator/刚体为空直接返回
            if (targetAnimator == null || rb == null) return;

            // 6. 优化速度计算（避免Time.deltaTime为0的异常）
            Vector2 currentPos = transform.position;
            Vector2 velocity = Time.deltaTime > 0 ? (currentPos - lastPos) / Time.deltaTime : Vector2.zero;

            // 7. 速度防抖动（仅当速度超过阈值时更新）
            float walkSpeed = Mathf.Abs(velocity.x) > speedThreshold ? Mathf.Abs(velocity.x) : 0f;
            float jumpSpeed = Mathf.Abs(velocity.y) > speedThreshold ? velocity.y : 0f;

            // 8. 更新动画参数（空值保护）
            targetAnimator.SetFloat("Walk", walkSpeed);
            targetAnimator.SetFloat("Jump", jumpSpeed);
           
            // 9. 记录当前位置（放在最后，避免第一帧计算错误）
            lastPos = currentPos;
        }

        // 动画切换主方法
        public void ChangeAnimation(float? duration = null)
        {
            // 空值校验：Animator/Character为空直接返回
            if (targetAnimator == null || Character == null) return;

            ResetAnimation(); // 修正拼写：ResetAniamtion → ResetAnimation
            StartCoroutine(SetAnimation(duration));
        }

        // 修正拼写错误：ResetAniamtion → ResetAnimation（核心！）
        public void ResetAnimation()
        {
            // 空值校验：Animator/参数数组为空直接返回
            if (targetAnimator == null || Parameters == null || Parameters.Length == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] 无Animator参数可重置！", this);
                return;
            }

            // 遍历重置参数（排除Walk/Jump，保留你的逻辑）
            foreach (var item in Parameters)
            {
                // 跳过空参数项
                if (item == null || string.IsNullOrEmpty(item.name)) continue;

                // 保留Walk/Jump参数不重置（你的核心逻辑）
                if (item.name == "Alive" ) continue;

                // 按类型重置参数
                switch (item.type)
                {
                    case AnimatorControllerParameterType.Float:
                        targetAnimator.SetFloat(item.name, 0f);
                        break;
                    case AnimatorControllerParameterType.Int:
                        targetAnimator.SetInteger(item.name, 0);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        targetAnimator.SetBool(item.name, false);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        targetAnimator.ResetTrigger(item.name);
                        break;
                }
            }
        }

        // 设置动画协程
        public IEnumerator SetAnimation(float? duration = null)
        {
            // 空值校验
            if (targetAnimator == null || Character == null) yield break;

            string stateName = Character.CurrentState.ToString();

            // 10. 优化：先检查参数是否存在，避免Animator报错
            if (!HasAnimatorParameter(stateName, AnimatorControllerParameterType.Bool))
            {
                Debug.LogWarning($"[{gameObject.name}] Animator无Bool参数：{stateName}", this);
            }
            else
            {
                targetAnimator.SetBool(stateName, true);
            }

            if (!HasAnimatorParameter(stateName, AnimatorControllerParameterType.Trigger))
            {
                Debug.LogWarning($"[{gameObject.name}] Animator无Trigger参数：{stateName}", this);
            }
            else
            {
                targetAnimator.SetTrigger(stateName);
            }

            // 11. 等待指定时长（加范围校验）
            if (duration.HasValue && duration.Value > 0f && duration.Value < 10f) // 防超长等待
            {
                yield return new WaitForSeconds(duration.Value);

                // 重置动画后切回Idle
                ResetAnimation();
                if (HasAnimatorParameter("Idle", AnimatorControllerParameterType.Bool))
                {
                    targetAnimator.SetBool("Idle", true);
                }
            }
        }

        // 辅助方法：检查Animator是否包含指定参数
        private bool HasAnimatorParameter(string paramName, AnimatorControllerParameterType paramType)
        {
            if (targetAnimator == null) return false;

            foreach (var param in targetAnimator.parameters)
            {
                if (param.name == paramName && param.type == paramType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}