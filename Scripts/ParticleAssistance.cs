//using UnityEngine;

//namespace shootstar
//{
//    public class ParticleAssistance : MonoBehaviour
//    {
//        [SerializeField] private GameObject Player; // 玩家对象

//        [Header("Scale Based Settings")]
//        [SerializeField] private Vector3 flipRotation = new Vector3(0, 180, 0); // 玩家X轴为负时的旋转
//        [SerializeField] private Vector3 flipScale = new Vector3(1, 1, 1);     // 玩家X轴为负时的缩放

//        [Header("Z-Rotation Based Settings")]
//        [SerializeField]private Vector3 zOriginFlipRotation = new Vector3(0, 0, 0); // 玩家Z轴0度时的旋转
//        [SerializeField] private Vector3 zFlipRotation = new Vector3(0, 0, 180); // 玩家Z轴-180度时的旋转
//        [SerializeField] private Vector3 zFlipScale = new Vector3(1, 1, 1);      // 玩家Z轴-180度时的缩放

//        private Vector3 originalRotation; // 当前对象的原始旋转
//        private Vector3 originalScale;    // 当前对象的原始缩放

//        [SerializeField] private float scaleThreshold = 0.1f;      // 缩放检测阈值
//        [SerializeField] private float rotationThreshold = 90f;    // Z轴旋转检测阈值

//        void Start()
//        {
//            // 自动获取玩家引用
//            if (Player == null && shootingstarGameManager.Instance != null)
//            {
//                Player = shootingstarGameManager.Instance.Player;
//            }

//            // 记录当前对象的初始旋转和缩放
//            originalRotation = transform.rotation.eulerAngles;
//            originalScale = transform.localScale;
//        }

//        void Update()
//        {
//            if (Player == null) return;

//            // 优先检测Z轴旋转是否为-180度
//            if (IsPlayerZRotated())
//            {
//                // 玩家Z轴-180度，应用Z轴翻转设置
//                transform.rotation = Quaternion.Euler(zFlipRotation);
//                originalRotation = zOriginFlipRotation; // 更新原始旋转为Z轴0度时的旋转
//                transform.localScale = zFlipScale;
//            }
//            // 检测玩家scale的X轴正负
//            else if (Player.transform.localScale.x < -scaleThreshold)
//            {
//                // 玩家X轴为负，设置翻转旋转和缩放
//                transform.rotation = Quaternion.Euler(flipRotation);
//                transform.localScale = flipScale;
//            }
//            else if (Player.transform.localScale.x > scaleThreshold)
//            {
//                // 玩家X轴为正，恢复原始旋转和缩放
//                transform.rotation = Quaternion.Euler(originalRotation);
//                transform.localScale = originalScale;
//            }
//        }

//        /// <summary>
//        /// 判断玩家Z轴是否旋转-180度
//        /// </summary>
//        private bool IsPlayerZRotated()
//        {
//            float zRotation = Player.transform.rotation.eulerAngles.z;
//            // 检测Z轴是否接近180度或-180度
//            return Mathf.Abs(zRotation - 180f) < rotationThreshold ||
//                   Mathf.Abs(zRotation + 180f) < rotationThreshold;
//        }

//        // 手动刷新旋转状态
//        public void RefreshRotation()
//        {
//            if (Player == null) return;

//            if (IsPlayerZRotated())
//            {
//                transform.rotation = Quaternion.Euler(zFlipRotation);
//                transform.localScale = zFlipScale;
//            }
//            else if (Player.transform.localScale.x < -scaleThreshold)
//            {
//                transform.rotation = Quaternion.Euler(flipRotation);
//                transform.localScale = flipScale;
//            }
//            else
//            {
//                transform.rotation = Quaternion.Euler(originalRotation);
//                transform.localScale = originalScale;
//            }
//        }
//    }
//}
using UnityEngine;

namespace shootstar
{
    public class ParticleAssistance : MonoBehaviour
    {
        [SerializeField] private GameObject Player; // 玩家对象

        [Header("Scale Based Settings")]
        [SerializeField] private Vector3 flipRotation = new Vector3(0, 180, 0); // 玩家X轴为负时的旋转
        [SerializeField] private Vector3 flipScale = new Vector3(1, 1, 1);     // 玩家X轴为负时的缩放

        [Header("Z-Rotation Based Settings")]
        [SerializeField] private Vector3 zOriginFlipRotation = new Vector3(0, 0, 0); // 玩家Z轴0度/scaleX正的旋转
        [SerializeField] private Vector3 zFlipRotation = new Vector3(0, 0, 180);     // 玩家Z轴-180度/scaleX负的旋转
        [SerializeField] private Vector3 zFlipScale = new Vector3(1, 1, 1);          // 玩家Z轴旋转时的缩放

        private Vector3 originalRotation; // 当前对象的原始旋转
        private Vector3 originalScale;    // 当前对象的原始缩放

        [SerializeField] private float scaleThreshold = 0.1f;      // 缩放检测阈值
        [SerializeField] private float rotationThreshold = 90f;    // Z轴旋转检测阈值

        void Start()
        {
            // 自动获取玩家引用
            if (Player == null && shootingstarGameManager.Instance != null)
            {
                Player = shootingstarGameManager.Instance.Player;
            }

            // 记录当前对象的初始旋转和缩放
            originalRotation = transform.rotation.eulerAngles;
            originalScale = transform.localScale;
        }

        void Update()
        {
            if (Player == null) return;

            // 检测玩家是否处于Z轴旋转状态
            if (IsPlayerZRotated())
            {
                // 在Z轴旋转状态下，根据scaleX的正负切换对应的Z轴旋转参数
                if (Player.transform.localScale.x < -scaleThreshold)
                {
                    // Z轴旋转且scaleX为负：使用zFlipRotation
                    transform.rotation = Quaternion.Euler(zFlipRotation);
                }
                else
                {
                    // Z轴旋转且scaleX为正：使用zOriginFlipRotation
                    transform.rotation = Quaternion.Euler(zOriginFlipRotation);
                }
                transform.localScale = zFlipScale; // 应用Z轴旋转时的缩放
            }
            // 非Z轴旋转状态下，使用原有的scaleX判断逻辑
            else if (Player.transform.localScale.x < -scaleThreshold)
            {
                // 玩家X轴为负，设置翻转旋转和缩放
                transform.rotation = Quaternion.Euler(flipRotation);
                transform.localScale = flipScale;
            }
            else if (Player.transform.localScale.x > scaleThreshold)
            {
                // 玩家X轴为正，恢复原始旋转和缩放
                transform.rotation = Quaternion.Euler(originalRotation);
                transform.localScale = originalScale;
            }
        }

        /// <summary>
        /// 判断玩家Z轴是否旋转-180度
        /// </summary>
        private bool IsPlayerZRotated()
        {
            float zRotation = Player.transform.rotation.eulerAngles.z;
            // 检测Z轴是否接近180度或-180度
            return Mathf.Abs(zRotation - 180f) < rotationThreshold ||
                   Mathf.Abs(zRotation + 180f) < rotationThreshold;
        }

        // 手动刷新旋转状态
        public void RefreshRotation()
        {
            if (Player == null) return;

            if (IsPlayerZRotated())
            {
                if (Player.transform.localScale.x < -scaleThreshold)
                {
                    transform.rotation = Quaternion.Euler(zFlipRotation);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(zOriginFlipRotation);
                }
                transform.localScale = zFlipScale;
            }
            else if (Player.transform.localScale.x < -scaleThreshold)
            {
                transform.rotation = Quaternion.Euler(flipRotation);
                transform.localScale = flipScale;
            }
            else
            {
                transform.rotation = Quaternion.Euler(originalRotation);
                transform.localScale = originalScale;
            }
        }
    }
}