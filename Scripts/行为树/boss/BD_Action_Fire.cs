using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace shootstar
{
    public enum targetType
    {
        Character,
        Spider
    }
    [TaskCategory("Halabang/Boss")]
    public class BD_Action_Fire : Action
    {
        public GameObject target;
        public Transform firePoint;
        public GameObject bulletPrefab;
        public GameObject bloomEffector;
        public float bulletForce = 20f;
        public LayerMask hitLayer;
        public float duration = 5f;

        public float maxExistTime = 5f;   // ★★★ 新增：子弹最大存在时间
        private float timer = 0f;         // ★★★ 新增：计时器

        private bool hit = false;
        private bool hitPlayer = false;
        private bool playerInWeb = false;
        private Vector3 hitPos;
        private bool already = false;

        private GameObject bullet;        // ★★★ 新增：保存实例化的子弹
        public targetType TargetType= targetType.Character;

        public override void OnStart()
        {
            timer = 0f;

            hit = false;
            hitPlayer = false;
            playerInWeb = false;

            // 朝向玩家（你之前的规则：1=左， -1=右）
            Transform player = shootingstarGameManager.Instance.Player.transform;
            float dir = Mathf.Sign(player.position.x - target.transform.position.x);
            Vector3 scale = target.transform.localScale;

            // dir > 0 = 玩家在右边，那么角色要朝右，也就是 scale.x 要为负
            if (dir > 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }

            target.transform.localScale = scale;


            Vector3 playerPos = player.transform.position;
            Vector2 fireDir = (playerPos - firePoint.position).normalized;

            bullet = GameObject.Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            bullet.GetComponent<Bullet_RB>().Init(
                fireDir,
                bulletForce,
                hitLayer,
                OnBulletHit,
                OnBulletHitPlayer
            );
            if(TargetType== targetType.Character)
            {
                target.GetComponent<Character_base>().CurrentState = _CharacterStates.Ability;
                target.GetComponent<Character_Animation>().ChangeAnimation(2f);
            }
            else if(TargetType== targetType.Spider)
            {
                target.GetComponent<SpiderController>().PlayAni("Fire");
            }

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;

            // ★★★ 新增：超过时间未命中 → 自动销毁并成功 ★★★
            if (timer >= maxExistTime && !hit && !hitPlayer)
            {
                if (bullet != null)
                    GameObject.Destroy(bullet);

                Debug.Log("子弹在限定时间内未命中 -> 自动销毁");

                return TaskStatus.Success;
            }
            // ★★★ 新增结束 ★★★


            if (hit && !already)
            {
                PlayEffector(hitPos);
                already = false;
                return TaskStatus.Success;
            }

            if (hitPlayer)
            {
                Debug.Log("玩家被蛛网子弹直接击中！");
                PlayEffector(hitPos);
                already = false;
                return TaskStatus.Success;
            }

            if (playerInWeb)
            {
                Debug.Log("玩家进入蛛网范围！");
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        private void OnBulletHit(Vector3 pos)
        {
            hit = true;
            hitPos = pos;
        }

        private void OnBulletHitPlayer(Vector3 pos)
        {
            hitPlayer = true;
            hitPos = pos;
        }

        private void OnPlayerEnterWeb()
        {
            playerInWeb = true;
        }

        private void PlayEffector(Vector3 pos)
        {
            if (bloomEffector)
            {
                var eff = GameObject.Instantiate(bloomEffector, pos, Quaternion.identity);
                GameObject.Destroy(eff, duration);
                already = true;
            }
        }
    }
}
