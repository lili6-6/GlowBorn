//using System.Collections;
//using Unity.Collections;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.InputSystem.XR.Haptics;

//namespace shootstar
//{
//    [System.Serializable]
//    public class DestorySetting
//    {
//       public string targetParameter;
//        public float Delay;
//        public UnityEvent UnityEvent;
//    }
//    public class Property_Animation : MonoBehaviour
//    {
//        [SerializeField] private Animator targetAnimater;
//        //[SerializeField] private Animator aiAniamator;
//        private AnimatorControllerParameter[] Parmeters;
//        //private AnimatorControllerParameter[] aiParmeters;
//        private Property_base Character;

//       [SerializeField] public DestorySetting[] destorySettings;
//        // Start is called once before the first execution of Update after the MonoBehaviour is created

//        void Awake()
//        {
//            Character = GetComponent<Property_base>();
//        }
//        void Start()
//        {
//            Parmeters = targetAnimater.parameters;
//            //if (aiAniamator != null)
//            //    aiParmeters = aiAniamator.parameters;
//            ChangeAnimation();
//        }

//        // Update is called once per frame
//        void Update()
//        {

//        }
//        public void ChangeAnimation()
//        {
//            ResetAniamtion();

//            StartCoroutine( SetAnimation());

//        }
//        public void ResetAniamtion()
//        {

//            foreach (var item in Parmeters)
//            {
//                switch (item.type)
//                {
//                    case AnimatorControllerParameterType.Float:
//                        targetAnimater.SetFloat(item.name, 0);
//                        break;
//                    case AnimatorControllerParameterType.Int:
//                        targetAnimater.SetInteger(item.name, 0);
//                        break;
//                    case AnimatorControllerParameterType.Bool:
//                        targetAnimater.SetBool(item.name, false);
//                        break;
//                    case AnimatorControllerParameterType.Trigger:
//                        targetAnimater.ResetTrigger(item.name);
//                        break;
//                    default:
//                        break;
//                }
//            }


//        }


//        public IEnumerator SetAnimation()
//        {
//            string stateName = Character.CurrentState.ToString();
//            Debug.Log($"Setting animation state to: {stateName}");
//            targetAnimater.SetBool(stateName, true);
//            if (stateName == "Awake_green") eventManager.Instance.Floweropen.Invoke();
//            if (destorySettings != null)
//            {
//                foreach (var destorySetting in destorySettings)
//                {
//                    if (destorySetting.targetParameter == stateName)
//                    {
//                        destorySetting.UnityEvent?.Invoke();
//                        if (destorySetting.Delay > 0)
//                        {
//                            yield return new WaitForSeconds(destorySetting.Delay);
//                            Destroy(this.gameObject);
//                        }

//                    }
//                }
//            }


//        }


//    }
//}
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR.Haptics;

namespace shootstar
{
    [System.Serializable]
    public class DestorySetting
    {
        public string targetParameter;
        public float Delay;
        public UnityEvent UnityEvent;
    }

    public class Property_Animation : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimater;
        private AnimatorControllerParameter[] Parmeters;
        private Property_base Character;
        [SerializeField] public DestorySetting[] destorySettings;

        // 添加标志防止重复执行
        private bool isCoroutineRunning = false;

        void Awake()
        {
            Character = GetComponent<Property_base>();
        }

        void Start()
        {
            Parmeters = targetAnimater.parameters;
            ChangeAnimation();
        }

        public void ChangeAnimation()
        {
            // 检查对象是否激活，且协程未在运行
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"无法在非激活对象上启动协程: {gameObject.name}");
                // 直接同步执行动画设置，不使用协程
                SetAnimationImmediately();
                return;
            }

            if (isCoroutineRunning) return;

            ResetAniamtion();
            StartCoroutine(SetAnimationCoroutine());
        }

        public void ResetAniamtion()
        {
            foreach (var item in Parmeters)
            {
                switch (item.type)
                {
                    case AnimatorControllerParameterType.Float:
                        targetAnimater.SetFloat(item.name, 0);
                        break;
                    case AnimatorControllerParameterType.Int:
                        targetAnimater.SetInteger(item.name, 0);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        targetAnimater.SetBool(item.name, false);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        targetAnimater.ResetTrigger(item.name);
                        break;
                    default:
                        break;
                }
            }
        }

        // 协程版本（对象激活时使用）
        private IEnumerator SetAnimationCoroutine()
        {
            isCoroutineRunning = true;
            yield return SetAnimationInternal();
            isCoroutineRunning = false;
        }

        // 同步版本（对象非激活时使用）
        private void SetAnimationImmediately()
        {
            // 使用协程的包装器立即执行
            StartCoroutine(SetAnimationInternal(true));
        }

        // 核心动画设置逻辑
        private IEnumerator SetAnimationInternal(bool isImmediate = false)
        {
            string stateName = Character.CurrentState.ToString();
            //Debug.Log($"Setting animation state to: {stateName}");

            // 确保Animator组件可用
            if (targetAnimater != null && targetAnimater.isActiveAndEnabled)
            {
                targetAnimater.SetBool(stateName, true);
            }

            if (stateName == "Awake_green")
                eventManager.Instance.Floweropen.Invoke();

            if (destorySettings != null)
            {
                foreach (var destorySetting in destorySettings)
                {
                    if (destorySetting.targetParameter == stateName)
                    {
                        destorySetting.UnityEvent?.Invoke();

                        if (destorySetting.Delay > 0)
                        {
                            // 如果是立即执行模式，跳过等待
                            if (!isImmediate)
                            {
                                yield return new WaitForSeconds(destorySetting.Delay);
                            }

                            // 安全销毁对象
                            if (gameObject != null)
                            {
                                Destroy(gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}