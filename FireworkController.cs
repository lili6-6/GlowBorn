using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains;
using MoreMountains.CorgiEngine;

namespace shootstar
{
    /// <summary>
    /// 控制子弹行为：直线飞行，碰撞即销毁
    /// </summary>
    public class FireworkController : MonoBehaviour
    {
        [Header("Bullet Settings")]
        public float damage = 30f;                   // 子弹伤害
        [Header("攻击对象")]
        public string[] targetTags = { "Monster" };         // 目标的Tag
        public GameObject FireEffectPrefab;          // 发射特效    
        public GameObject hitEffectPrefab;           // 碰撞特效
        public float surviveTime = 5f;      // 子弹存活时间
        public BulletExplosion BulletExplosion; // 震动脚本
        [Header("音效")]
       private AudioSource explosionSound; // 爆炸音效
        public static FireworkController Instance;

        void Awake()
        {
            Instance = this;
          
        }

        void Start()
        {
            StartCoroutine(survive());
            explosionSound = ColorAbility.Instance.ExplosionAudio;
        }
  
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 如果碰到不想碰撞的标签，直接返回
            if (collision.CompareTag("nearScene"))
            {
                // 什么都不做，子弹继续飞行
                return;
            }
            // 检查是否击中目标
            // if (collision.collider.CompareTag(targetTag))
            // 判断碰撞对象是否在允许的目标标签列表中
            if (IsTargetTag(collision.tag, targetTags))
            {
                Debug.Log("💥 子弹击中目标：" + collision.name);

                // 对敌人造成伤害
                //Health targetHealth = collision.GetComponent<Health>();
                //if (targetHealth != null)
                //{
                //    targetHealth.TakeDamage(damage);
                //}
                // 对敌人造成伤害
                Health targetHealth = collision.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.Damage(damage, this.gameObject, 0.1f, 0f, Vector3.zero);
                }
                else if (collision.GetComponent<Health_extension>() != null) 
                {
                 collision.GetComponent<Health_extension>().health.Damage(damage,this.gameObject,0.1f, 0f, Vector3.zero);

                }
            }
            else
            {
                Debug.Log("子弹碰到非目标对象：" + collision.name);
            }
            // 播放爆炸音效
            if (explosionSound != null)
            {
                explosionSound.Play();
                Debug.Log("播放子弹爆炸音效");
            }
            // 播放击中特效
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
               // Debug.Log("播放子弹击中特效");
                BulletExplosion.Explode();
                Destroy(effect, 2f); // 2秒后自动清理
            }
            // 销毁子弹
            Destroy(gameObject);
        }
        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    //// 如果碰到不想碰撞的标签，直接返回
        //    //if (collision.CompareTag("nearScene"))
        //    //{
        //    //    // 什么都不做，子弹继续飞行
        //    //    return;
        //    //}
        //    // 检查是否击中目标
        //    // if (collision.collider.CompareTag(targetTag))
        //    // 判断碰撞对象是否在允许的目标标签列表中
        //    if (IsTargetTag(collision.tag, targetTags))
        //    {
        //        Debug.Log("💥 子弹击中目标：" + collision.name);
        //        // 对敌人造成伤害
        //        Health targetHealth = collision.GetComponent<Health>();
        //        if (targetHealth != null)
        //        {
        //            targetHealth.Damage(damage, this.gameObject, 0.1f, 0f, Vector3.zero);
        //        }
        //        else
        //        {
        //            Debug.Log("子弹碰到非目标对象：" + collision.name);
        //        }
        //        // 播放击中特效
        //        if (hitEffectPrefab != null)
        //        {
        //            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        //            Debug.Log("播放子弹击中特效");
        //            BulletExplosion.Explode();
        //            Destroy(effect, 2f); // 2秒后自动清理
        //        }
        //        // 销毁子弹
        //        Destroy(gameObject);
        //    }
        //}

        private IEnumerator survive()
        {
            yield return new WaitForSeconds(surviveTime);
            //Debug.Log("子弹存活时间到，自动销毁");
            Destroy(gameObject);
        }

        public void PlayFireEffect()
        {
            // 播放发射特效
            if (FireEffectPrefab != null)
            {
                GameObject effect = Instantiate(FireEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 3f); // 2秒后自动清理
            }
        }
        /// <summary>
        /// 判断是否是合法目标标签
        /// </summary>
        //public bool IsTargetTag(string tag)
        //{
        //    foreach (var t in targetTags)
        //    {
        //        if (tag == t) return true;
        //    }
        //    return false;
        //}
        public static bool IsTargetTag(string tag, string[] targetTags)
        {
            foreach (var t in targetTags)
            {
                if (tag == t) return true;
            }
            return false;
        }

    }
}
