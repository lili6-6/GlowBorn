using UnityEngine;

namespace shootstar
{
    public class Bullet_RB : MonoBehaviour
    {
        private System.Action<Vector3> onHit;
        private System.Action<Vector3> onHitPlayer;

        private LayerMask hitLayer;

        public void Init(Vector2 dir, float force, LayerMask layer,
                         System.Action<Vector3> hitCallback,
                         System.Action<Vector3> hitPlayerCallback)
        {
            hitLayer = layer;
            onHit = hitCallback;
            onHitPlayer = hitPlayerCallback;

            GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;

            // ★ ① 玩家检测（不依靠 LayerMask）
            if (other.CompareTag("Player"))
            {
                onHitPlayer?.Invoke(transform.position);
                Destroy(gameObject);
                return;
            }

            // ★ ② 非玩家 → 检查是否是“命中目标层”
            if (((1 << otherLayer) & hitLayer) != 0)
            {
                Debug.Log(other.name);
                onHit?.Invoke(transform.position);
                Destroy(gameObject);
                return;
            }

            // ★ ③ 不在 hitLayer → 完全无事发生（不会销毁）
            // Debug.Log("子弹碰到非目标物体：" + other.name);
        }
    }


}