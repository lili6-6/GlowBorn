//using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
//using UnityEngine;
//using static UnityEngine.Input;

//namespace shootstar
//{
//    public class InputController : MonoBehaviour
//    {
//        // Start is called once before the first execution of Update after the MonoBehaviour is created

//        [SerializeField] private KeyCode changeColor=KeyCode.M;
//        [SerializeField] private KeyCode interact=KeyCode.N;
//        [SerializeField] private KeyCode pose=KeyCode.P;

//        [HideInInspector]private float interactHoldTimer=0f;
//        [HideInInspector]private float ineractPressTime=1f;
//        void Start()
//        {

//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (shootingstarGameManager.Instance.uiManager.ChangeBut.isInteractable&& Input.GetKeyDown(changeColor)||Input.GetAxis("Mouse ScrollWheel") !=0)
//            {
//                Debug.Log("111");
//               shootingstarGameManager.Instance.ChangeColor();
//            }
//            else if (shootingstarGameManager.Instance.uiManager.interBut.isInteractable && Input.GetKeyDown(interact)||Input.GetKeyDown(KeyCode.Mouse0))
//            {
//                float pressTime = InteractDownTimer();
//                Debug.Log("222");
//               // shootingstarGameManager.Instance. meetInter(pressTime);
//               shootingstarGameManager.Instance.Player.GetComponent<ColorAbility>().meetInter(pressTime);
//            }
//            else if (Input.GetKeyDown(pose))
//            {
//                shootingstarGameManager.Instance.uiManager.SetBut.onClick.Invoke();
//            }
//        }

//        public float InteractDownTimer()
//        {
//            interactHoldTimer+= Time.deltaTime;
//            Debug.Log("按下时间："+ interactHoldTimer);
//            return interactHoldTimer;
//        }
//    }
//}
using UnityEngine;
// 移除无用的命名空间（避免冲突）
// using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
// using static UnityEngine.Input;
using Michsky;
using UnityEngine.UI;
namespace shootstar
{
    public class InputController : MonoBehaviour
    {
        [Header("交互按键配置")]
        [SerializeField] private KeyCode changeColor = KeyCode.M;    // 切换颜色按键
        [SerializeField] private KeyCode interact = KeyCode.N;      // 交互按键
        [SerializeField] private KeyCode pause = KeyCode.P;          // 按键

        [Header("交互计时配置")]
        [SerializeField] private float interactPressThreshold = 1f; // 长按判定阈值（秒）
        private float interactHoldTimer = 0f;                       // 交互按键按压时长
        private bool isInteractHolding = false;                     // 是否正在按住交互键

        [Header("触发间隔配置")]
        [SerializeField] private float scrollCooldown = 0.2f;       // 滚轮触发间隔（防连触）
        private float lastScrollTriggerTime = 0f;                   // 滚轮上次触发时间
       
        


        void Start()
        {
            
        } 
        void Update()
        {
            // ========== 1. 切换颜色：M键按下 或 滚轮滚动（带冷却+UI可交互） ==========
            CheckChangeColorInput();

            // ========== 2. 交互逻辑：N键/左键按住计时，松开时传递按压时长 ==========
            CheckInteractInput();

            // ========== 3. 姿势触发：P键按下（原逻辑保留） ==========
            CheckPauseInput();
        }

        /// <summary>
        /// 检测切换颜色输入（M键/滚轮）
        /// </summary>
        private void CheckChangeColorInput()
        {
            // 仅当UI按钮可交互时触发
            if (!shootingstarGameManager.Instance.uiManager.ChangeBut.isInteractable) return;

            bool isMKeyPressed = Input.GetKeyDown(changeColor); // M键瞬时按下
            float scrollValue = Input.GetAxis("Mouse ScrollWheel"); // 滚轮值
            bool isScrollTrigger = scrollValue != 0 && Time.time - lastScrollTriggerTime > scrollCooldown; // 滚轮滚动且超过冷却

            // M键按下 或 滚轮滚动 → 触发颜色切换
            if (isMKeyPressed || isScrollTrigger)
            {
                Debug.Log("触发颜色切换 | 滚轮值：" + scrollValue + " | M键按下：" + isMKeyPressed);
                shootingstarGameManager.Instance.ChangeColor();

                // 更新滚轮冷却时间（仅滚轮触发时更新）
                if (isScrollTrigger)
                {
                    lastScrollTriggerTime = Time.time;
                }
            }
        }

        /// <summary>
        /// 检测交互输入（N键/左键按住计时）
        /// </summary>
        private void CheckInteractInput()
        {
            // 仅当UI按钮可交互时触发
            if (!shootingstarGameManager.Instance.uiManager.interBut.isInteractable)
            {
                // 非交互状态：重置计时
                ResetInteractTimer();
                return;
            }

            // --- 交互键（N键）按住/松开检测 ---
            if (Input.GetKeyDown(interact) || Input.GetMouseButtonDown(0))
            {
                isInteractHolding = true;
                interactHoldTimer = 0f;

                shootingstarGameManager.Instance.Player
                    .GetComponent<ColorAbility>()
                    .OnInteractDown();

             
                Debug.Log("交互键按下，开始计时");
            }
            else if (Input.GetKeyUp(interact) || Input.GetMouseButtonUp(0))
            {
                if (isInteractHolding)
                {
                    shootingstarGameManager.Instance.Player
                        .GetComponent<ColorAbility>()
                        .OnInteractUp();

                    shootingstarGameManager.Instance.Player
                        .GetComponent<ColorAbility>()
                        .meetInter(interactHoldTimer);

                   
                    ResetInteractTimer();
                }
            }


            // --- 按住期间累加时长 ---
            if (isInteractHolding)
            {
                interactHoldTimer += Time.deltaTime;
               
                // 可选：长按阈值提醒（比如超过1秒打印日志）
                if (interactHoldTimer >= interactPressThreshold && Mathf.Approximately(interactHoldTimer % interactPressThreshold, 0))
                {
                    Debug.Log("交互键长按超过" + interactPressThreshold + "秒");
                }
            }
        }

        /// <summary>
        /// 检测姿势触发输入（P键）
        /// </summary>
        private void CheckPauseInput()
        {
            if (Input.GetKeyDown(pause))
            {
                Debug.Log("触发姿势");
                shootingstarGameManager.Instance.uiManager.SetBut.onClick.Invoke();
            }
        }

        /// <summary>
        /// 重置交互计时状态
        /// </summary>
        private void ResetInteractTimer()
        {
            isInteractHolding = false;
            interactHoldTimer = 0f;
           
        }

        // 可选：外部调用重置滚轮冷却（比如切换场景）
        public void ResetScrollCooldown()
        {
            lastScrollTriggerTime = 0f;
        }

    }
}