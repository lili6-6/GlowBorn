using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace shootstar
{
    public class FireworkSkill : MonoBehaviour
    {
        [Header("Bullet Settings")]
        public GameObject bulletPrefab;    // 子弹预制体，需带 Rigidbody2D
        public Transform shootPoint;       // 发射位置
        public float shootSpeed = 15f;     // 子弹速度

        [Header("Reference")]
        public Transform model;            // 模型（决定发射方向，左右翻转）

        public static FireworkSkill Instance;   

        private Camera mainCamera;

        private bool isInReverseZone= false;



        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    // ① 如果点击到 UI，则直接返回
            //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            //    {
            //        Debug.Log("点击在 UI 上，不发射子弹");
            //        return;
            //    }

            //    // ② 如果当前颜色不是红色，也返回
            //    if (shootingstarGameManager.Instance.currentColor != "red")
            //    {
            //        Debug.Log("当前不是红色，不能发射子弹");
            //        return;
            //    }

            //    // ③ 两个条件都通过，才执行发射
            //    ShootStraight();
            //}
            

        }

        /// <summary>
        /// 直线发射子弹，自动判断左右方向
        /// </summary>
        public void ShootStraight()
        {
            if (bulletPrefab == null || shootPoint == null || model == null)
            {
                Debug.LogError("❌ 缺少必要引用，请在Inspector中拖入 BulletPrefab、ShootPoint 和 Model！");
                return;
            }
            if (shootingstarGameManager.Instance.uiManager.cLightNum <= 0)
            {
                Debug.Log("⚠️ 光源不足，无法发射子弹！");
                return;
            }
           
            // 生成子弹
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            int temp =shootingstarGameManager.Instance.uiManager. cLightNum -= 1;
            colliderManager.Instance.OnCollectItem(-0.5f);
            shootingstarGameManager.Instance.uiManager. UpdateLightText(temp);

            // 判断模型朝向：localScale.x > 0 说明面向右，< 0 面向左
            Vector2 shootDir = (model.localScale.x > 0) ? Vector2.right : Vector2.left;

            // 如果在反向区域，反转方向
            if (isInReverseZone)
            {
                Debug.Log("翻转");
                shootDir = -shootDir;
            }
            // 给子弹施加速度
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = shootDir * shootSpeed;
            }
            else
            {
                Debug.LogError("❌ 子弹Prefab缺少Rigidbody2D组件！");
            }
            //播放发射特效
            FireworkController.Instance.PlayFireEffect();
            //播放发射音效
            ColorAbility.Instance.redAudio.Play();
            // 让子弹朝向运动方向
            bullet.transform.right = shootDir;
            StartCoroutine(ColorAbility.Instance.PlayEffects(ColorAbility.Instance.redEffect));
        }
        public void SetReverseState(bool state)
        {
            isInReverseZone = state;
        }
    }
}
