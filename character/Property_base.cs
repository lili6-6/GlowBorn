
//using MoreMountains.CorgiEngine;
//using System;
//using System.Collections;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Events;

//namespace shootstar
//{
//    public class Property_base : MonoBehaviour
//    {
//        protected virtual void Start()
//        {

//        }
//        public Property_Animation property_Animation { get; private set; }

//        public enum PropertyState
//        {
//            Sleep,
//            Awake, // idle
//            Awake_red, // idle
//            Awake_blue, // idle
//            Awake_green, // idle
//            Exit,
//            Activated,
//            Hurt,
//            Die

//        }

//        public PropertyState CurrentState { get; set; }

//        [Header("碰撞触发设置")]
//        public UnityEvent OnCustomTriggerEnter;
//        public UnityEvent OnCustomTriggerExit;
//        public UnityEvent OnCustomCollisionEnter;
//        public UnityEvent OnCustomCollisionExit;

//        [SerializeField] private bool CanTrigger = true;
//        [SerializeField] private bool ColorAwake = false;
//        private Collider2D childcollider2D;

//        [SerializeField] private PropertyState defaultState;
//        private ColorType currentColor= ColorType.White;

//        // 碰撞计数
//        private int collisionCount = 0;

//        // 允许触发的标签数组
//        [SerializeField] private string[] allowedTags = { "Player", "Enemy", "NPC" };

//        void Awake()
//        {
//            property_Animation = GetComponent<Property_Animation>();
//            if (property_Animation == null)
//            {
//                //Debug.LogError("Property_Animation component is missing on " + gameObject.name);
//            }

//            if (CanTrigger)
//            {
//                childcollider2D = GetComponent<Collider2D>();
//                if (childcollider2D == null)
//                {
//                    //Debug.LogError("Collider2D component is missing on " + gameObject.name);
//                }
//                else
//                {
//                    childcollider2D.isTrigger = true;
//                }
//            }

//            CurrentState = defaultState;
//        }

//        protected virtual void start()
//        {
//            if (CanTrigger == false) GetComponent<Collider2D>().enabled = false;
//        }

//        protected virtual void Update() { }

//        // 触发事件 - 进入碰撞
//        protected virtual void OnTriggerEnter2D(Collider2D collision)
//        {
//            if (CanTrigger == false) { return; }

//            if (IsValidTag(collision.gameObject))
//            {
//                collisionCount++;

//                if (collisionCount == 1)
//                {
//                    // 触发状态
//                    //Debug.Log(collision.gameObject.name + " entered the plant area");
//                    if (collision.tag == "Player")
//                    {
//                        currentColor = collision.GetComponent<ChangeColorManager>().currentColor;
//                        Debug.Log("Current Color: " + currentColor);

//                        if (ColorAwake)
//                        {
//                            switch (currentColor)
//                            {
//                                case ColorType.White:
//                                    CurrentState = PropertyState.Awake;
//                                    break;
//                                case ColorType.Red:
//                                    CurrentState = PropertyState.Awake_red;
//                                    Debug.Log("Red State Activated");
//                                    break;
//                                case ColorType.Blue:
//                                    CurrentState = PropertyState.Awake_blue;
//                                    Debug.Log("Blue State Activated");
//                                    break;
//                                case ColorType.Green:
//                                    CurrentState = PropertyState.Awake_green;
//                                    Debug.Log("Green State Activated");
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            CurrentState = PropertyState.Awake;
//                        }

//                    }
//                    else
//                    {
//                        CurrentState = PropertyState.Awake;
//                    }

//                        Debug.Log(CurrentState);
//                    if (property_Animation != null)
//                        property_Animation.ChangeAnimation();
//                }

//                OnCustomTriggerEnter?.Invoke();
//            }
//        }

//        // 触发事件 - 离开碰撞
//        protected virtual void OnTriggerExit2D(Collider2D collision)
//        {
//            if (CanTrigger == false) { return; }

//            if (IsValidTag(collision.gameObject))
//            {
//                collisionCount--;

//                if (collisionCount == 0)
//                {
//                    // 恢复状态
//                    //Debug.Log(collision.gameObject.name + " exited the plant area");
//                    CurrentState = PropertyState.Exit;
//                    if (property_Animation != null)
//                        property_Animation.ChangeAnimation();
//                    StartCoroutine(resetSleep());
//                }

//                OnCustomTriggerExit?.Invoke();
//            }
//        }

//        // 另一个碰撞检测方法 - 如果需要支持其他类型的碰撞事件
//        protected virtual void OnCollisionEnter2D(Collision2D collision)
//        {
//            if (CanTrigger == false) { return; }

//            if (IsValidTag(collision.gameObject))
//            {
//                collisionCount++;

//                if (collisionCount == 1)
//                {
//                    // 触发状态
//                    //Debug.Log(collision.gameObject.name + " entered the plant area");
//                    CurrentState = PropertyState.Awake;
//                    if (property_Animation != null)
//                        property_Animation.ChangeAnimation();
//                }

//                OnCustomCollisionEnter?.Invoke();
//            }
//        }

//        // 另一个碰撞检测方法 - 离开时
//        protected virtual void OnCollisionExit2D(Collision2D collision)
//        {
//            if (CanTrigger == false) { return; }

//            if (IsValidTag(collision.gameObject))
//            {
//                collisionCount--;

//                if (collisionCount == 0)
//                {
//                    // 恢复状态
//                    //Debug.Log(collision.gameObject.name + " exited the plant area");
//                    CurrentState = PropertyState.Exit;
//                    if (property_Animation != null)
//                        property_Animation.ChangeAnimation();
//                    StartCoroutine(resetSleep());
//                }

//                OnCustomCollisionExit?.Invoke();
//            }
//        }

//        // 用于验证标签是否在允许的标签数组中
//        //private bool IsValidTag(GameObject obj)
//        //{
//        //    foreach (string tag in allowedTags)
//        //    {
//        //        if (obj.CompareTag(tag))
//        //        {
//        //            return true;
//        //        }
//        //    }
//        //    return false;
//        //}
//        private bool IsValidTag(GameObject obj)
//        {
//            return Array.Exists(allowedTags, tag => tag == obj.tag);  // 使用 Array.Exists 查找标签
//        }
//        public IEnumerator resetSleep()
//        {

//            yield return new WaitForSeconds(2f);
//            if (CurrentState == PropertyState.Exit)
//            {
//                CurrentState = PropertyState.Sleep;
//                if (property_Animation != null)
//                    property_Animation.ChangeAnimation();
//            }
//        }
//    }
//}

using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace shootstar
{
    public class Property_base : MonoBehaviour, ILightReactive
    {
        public Property_Animation property_Animation { get; private set; }

        public enum PropertyState
        {
            Sleep,
            Awake,
            Awake_red,
            Awake_blue,
            Awake_green,
            Exit,
            Activated,
            Hurt,
            Die
        }

        public PropertyState CurrentState { get; set; }

        [Header("碰撞触发设置")]
        public UnityEvent OnCustomTriggerEnter;
        public UnityEvent OnCustomTriggerExit;

        [SerializeField] private bool CanTrigger = true;
        [SerializeField] private bool ColorAwake = false;
        [SerializeField] private PropertyState defaultState;

        private Collider2D _collider2D;
        private int _collisionCount = 0;

        [SerializeField] private string[] allowedTags = { "Player", "Enemy", "NPC" };

        private void Awake()
        {
            property_Animation = GetComponent<Property_Animation>();

            if (CanTrigger)
            {
                _collider2D = GetComponent<Collider2D>();
                if (_collider2D != null)
                {
                    _collider2D.isTrigger = true;
                }
            }

            CurrentState = defaultState;
        }

        #region Trigger

        protected virtual void Start() { }
        protected virtual void Update() { }

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!CanTrigger) return;
            if (!IsValidTag(collision.gameObject)) return;

            _collisionCount++;
            if (_collisionCount > 1) return;

            if (!ColorAwake)
            {
                // 不吃颜色：只 Awake
                CurrentState = PropertyState.Awake;
            }
            else
            {
                // 吃颜色：根据当前光 Awake
                var colorMgr = collision.GetComponent<ChangeColorManager>();
                var color = colorMgr != null ? colorMgr.CurrentColor : ColorType.White;

                ApplyLight(color);
                return; // ⚠️ 已经在 ApplyLight 里播动画了
            }

            property_Animation?.ChangeAnimation();
            OnCustomTriggerEnter?.Invoke();
        }


        public virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (!CanTrigger) return;
            if (!IsValidTag(collision.gameObject)) return;

            _collisionCount--;

            if (_collisionCount <= 0)
            {
                _collisionCount = 0;
                CurrentState = PropertyState.Exit;
                property_Animation?.ChangeAnimation();
                StartCoroutine(ResetSleep());
            }

            OnCustomTriggerExit?.Invoke();
        }

        #endregion

        #region Light Reaction（唯一颜色入口）

        public void ApplyLight(ColorType color)
        {
            if (!ColorAwake) return;

            switch (color)
            {
                case ColorType.White:
                    CurrentState = PropertyState.Awake;
                    break;
                case ColorType.Red:
                    CurrentState = PropertyState.Awake_red;
                    break;
                case ColorType.Blue:
                    CurrentState = PropertyState.Awake_blue;
                    break;
                case ColorType.Green:
                    CurrentState = PropertyState.Awake_green;
                    break;
            }

            Debug.Log($"[Property] ApplyLight {color} on {name}");
            property_Animation?.ChangeAnimation();
        }


        #endregion

        #region Utils

        private bool IsValidTag(GameObject obj)
        {
            return Array.Exists(allowedTags, tag => tag == obj.tag);
        }

        private IEnumerator ResetSleep()
        {
            yield return new WaitForSeconds(2f);

            if (CurrentState == PropertyState.Exit)
            {
                CurrentState = PropertyState.Sleep;
                property_Animation?.ChangeAnimation();
            }
        }

        #endregion
    }
}
