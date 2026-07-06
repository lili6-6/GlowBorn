using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.CorgiEngine;

namespace shootstar
{
    [TaskCategory("Halabang/Boss")]
    public class BD_Action_BossJump : Action
    {
        public GameObject boss;

        public float jumpDistance = 3f;
        public float jumpHeight = 2f;
        public float jumpTime = 0.8f;

        private Vector3 startPos;
        private Vector3 endPos;
        private float timer;
        private Transform player;

        // ★ 新增：检测 Boss 踩到玩家的范围
        public Vector2 hitCheckSize = new Vector2(1f, 1f);

        public override void OnStart()
        {
            timer = 0f;

            // 玩家
            player = shootingstarGameManager.Instance.Player.transform;

            if (boss == null)
                boss = gameObject;

            startPos = boss.transform.position;

            // 判断方向
            float dir = (player.position.x > boss.transform.position.x) ? 1f : -1f;

            // --- ★★★★★ 跳前自动朝向玩家 ★★★★★ ---
            // --- ★★★★★ 跳前自动朝向玩家（基于原scale翻转） ★★★★★ ---

            Vector3 scale = boss.transform.localScale;

            // 目标方向：若玩家在右边 → X 为负（或正，根据你项目），这里按你原逻辑不改
            float targetSign = (player.position.x > boss.transform.position.x) ? -1f : 1f;

            // 当前缩放符号（保持大小不变）
            float currentSize = Mathf.Abs(scale.x);

            // 应用翻转
            scale.x = currentSize * targetSign;

            boss.transform.localScale = scale;

            // ----------------------------------------------------


            // 计算落点
            endPos = startPos + new Vector3(jumpDistance * dir, 0f, 0f);

            // 播放动画
            var anim = boss.GetComponent<Character_base>();
            if (anim)
            {
                //anim.CurrentState = "Jump";
                //anim.character_Animation.ChangeAnimation();
            }
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float t = timer / jumpTime;
            if (t > 1f) t = 1f;

            // --- 抛物线 Y ---
            float heightOffset = 4f * jumpHeight * t * (1f - t);

            // --- 水平移动 ---
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += heightOffset;

            boss.transform.position = pos;

            shootingstarGameManager.Instance.cameraManager.ShakeCurrentCamera();
            // ★ 检查是否踩到玩家
            if (CheckHitPlayer())
            {
                ApplyDamageToPlayer();   // 这个方法留空给你填
                return TaskStatus.Success;
            }

            if (t >= 1f)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        private bool CheckHitPlayer()
        {
            if (player == null) return false;

            Vector2 bossPos = boss.transform.position;
            Vector2 playerPos = player.position;

            return Mathf.Abs(bossPos.x - playerPos.x) < hitCheckSize.x * 0.5f &&
                   Mathf.Abs(bossPos.y - playerPos.y) < hitCheckSize.y * 0.5f;
        }

        private void ApplyDamageToPlayer()
        {
            // TODO: 扣血逻辑塞这里
            player.GetComponent<Health>().Damage(20, this.gameObject, 0.1f, 0f, Vector3.zero);
            
        }
    }
}
