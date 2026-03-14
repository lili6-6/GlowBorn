
//using DG.Tweening;
//using UnityEngine;

//namespace shootstar
//{
//    public class legMove : MonoBehaviour
//    {
//        public Transform limbSolverTarget;
//        public float moveDistance;
//        public LayerMask groundLayer;

//        // 新增：用于指定重力方向的向量
//        public Vector2 gravityDirection = Vector2.down;

//        // 新增：碰撞检测相关变量
//        private bool isGrounded = false;
//        private Vector3 groundContactPoint;

//        // Start is called once before the first execution of Update after the MonoBehaviour is created
//        void Start()
//        {
//            // 可选：自动从Rigidbody2D获取重力方向
//            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
//            if (rb != null)
//            {
//                // 根据重力缩放判断重力方向
//                //gravityDirection = rb.gravityScale <= 0 ? Vector2.down : Vector2.up;
//            }

//            // 添加碰撞器和刚体（如果没有的话）
//            SetupCollider();
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            // 使用碰撞检测的结果更新脚部位置
//            if (isGrounded)
//            {
//                Vector3 point = groundContactPoint;
//                point += (Vector3)gravityDirection * -0.1f;
//                transform.position = point;
//            }

//            if (Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance)
//            {
//                move();
//            }
//        }

//        public void move()
//        {
//            Vector3 temp = new Vector3((transform.position.x + limbSolverTarget.position.x) / 2, limbSolverTarget.position.y - 1.5f, 0f);
//            limbSolverTarget.DOMove(temp, 0.3f)
//                .onComplete = () =>
//                {
//                    limbSolverTarget.DOMove(transform.position, 0.3f);
//                };
//        }

//        // 设置脚部的碰撞器
//        private void SetupCollider()
//        {
//            // 添加CircleCollider2D作为触发器
//            CircleCollider2D collider = GetComponent<CircleCollider2D>();
//            if (collider == null)
//            {
//                collider = gameObject.AddComponent<CircleCollider2D>();
//            }

//            // 设置为触发器
//            //collider.isTrigger = true;
//            collider.radius = 0.1f; // 脚部碰撞器大小

//            // 添加刚体（如果没有）
//            Rigidbody2D rb = GetComponent<Rigidbody2D>();
//            if (rb == null)
//            {
//                rb = gameObject.AddComponent<Rigidbody2D>();
//                //rb.isKinematic = true; // 设置为运动学刚体，不受物理影响
//                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//            }
//        }

//        // 触发器进入
//        private void OnTriggerEnter2D(Collider2D other)
//        {
//            if (((1 << other.gameObject.layer) & groundLayer) != 0)
//            {
//                isGrounded = true;
//                groundContactPoint = other.ClosestPoint(transform.position);
//            }
//        }

//        // 触发器停留
//        private void OnTriggerStay2D(Collider2D other)
//        {
//            if (((1 << other.gameObject.layer) & groundLayer) != 0)
//            {
//                isGrounded = true;
//                groundContactPoint = other.ClosestPoint(transform.position);
//            }
//        }

//        // 触发器退出
//        private void OnTriggerExit2D(Collider2D other)
//        {
//            if (((1 << other.gameObject.layer) & groundLayer) != 0)
//            {
//                isGrounded = false;
//            }
//        }
//    }
//}


//using DG.Tweening;
//using UnityEngine;

//namespace shootstar
//{
//    public class legMove : MonoBehaviour
//    {
//        public Transform limbSolverTarget;
//        public float moveDistance = 0.6f;
//        public LayerMask groundLayer;

//        private Vector2 gravityDir;
//        public float rayOffset = 0.4f;
//        public float footOffset = 0.1f;

//        void Start()
//        {
//            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
//            gravityDir = rb && rb.gravityScale < 0 ? Vector2.up : Vector2.down;
//        }

//        void Update()
//        {
//            UpdateGroundPoint();

//            if (Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance)
//            {
//                StepMove();
//            }
//        }

//        /// <summary>
//        /// 保证正、反重力都贴在地面正确一侧
//        /// </summary>
//        void UpdateGroundPoint()
//        {
//            // 射线起点在脚的反重力方向以外，避免穿平台
//            Vector3 origin = transform.position - (Vector3)gravityDir * rayOffset;

//            RaycastHit2D hit = Physics2D.Raycast(origin, gravityDir, 2f, groundLayer);

//            if (!hit.collider)
//                return;

//            Vector3 point = hit.point;

//            // 关键：根据 gravityDir 自动决定偏移方向
//            // 判断 normal 是否和重力方向相反
//            float dot = Vector2.Dot(hit.normal, -gravityDir);

//            Vector3 offsetDir;

//            if (dot > 0.5f)
//            {
//                // normal 与 -gravityDir 基本一致 → 地面在正确的一侧
//                offsetDir = hit.normal;
//            }
//            else
//            {
//                // normal 与 -gravityDir 不一致 → 标准化为地面外侧
//                offsetDir = -gravityDir;
//            }

//            point += offsetDir * footOffset;

//            transform.position = point;
//        }

//        void StepMove()
//        {
//            Vector3 mid = (transform.position + limbSolverTarget.position) * 0.5f
//                          - (Vector3)gravityDir * 0.6f;

//            limbSolverTarget.DOKill();

//            limbSolverTarget.DOMove(mid, 0.2f).OnComplete(() =>
//            {
//                limbSolverTarget.DOMove(transform.position, 0.2f);
//            });
//        }
//    }
//}
using DG.Tweening;
using UnityEngine;

namespace shootstar
{
    public class legMove : MonoBehaviour
    {
        public Transform limbSolverTarget;
        public float moveDistance = 0.6f;
        public LayerMask groundLayer;

        private Vector2 gravityDir;
        private Vector3 groundPoint;   // 推荐落脚点
        public float rayOffset = 0.4f;
        public float footOffset = 0.1f;

        void Start()
        {
            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
            gravityDir = rb && rb.gravityScale < 0 ? Vector2.up : Vector2.down;
        }

        void Update()
        {
            UpdateGroundPoint();

            // 检测 target 与 groundPoint 的距离
            float dist = Vector2.Distance(limbSolverTarget.position, groundPoint);
            if (dist > moveDistance)
            {
                StepMove();
            }
        }

        /// <summary>
        /// 计算正确落脚点，不直接改 transform.position
        /// </summary>
        void UpdateGroundPoint()
        {
            // 射线起点沿反重力方向抬高，避免穿平台
            Vector3 origin = transform.position - (Vector3)gravityDir * rayOffset;

            RaycastHit2D hit = Physics2D.Raycast(origin, gravityDir, 2f, groundLayer);
            if (!hit.collider) return;

            Vector3 point = hit.point;

            // 确保 target 落在“重力方向下的地面一侧”
            float dot = Vector2.Dot(hit.normal, -gravityDir);

            if (dot < 0)
            {
                // normal 与-重力方向相反 → 调整 target 到正确一侧
                point -= (Vector3)gravityDir * footOffset;
            }
            else
            {
                // normal 与-重力方向一致 → target 落在正常地面
                point += (Vector3)gravityDir * footOffset;
            }

            groundPoint = point;
        }

        void StepMove()
        {
            limbSolverTarget.DOKill(true);

            // 中点沿反重力方向抬起脚
            Vector3 mid = (limbSolverTarget.position + groundPoint) * 0.5f
                          - (Vector3)gravityDir * 0.6f;

            // 抬脚
            limbSolverTarget.DOMove(mid, 0.2f).OnComplete(() =>
            {
                // 落脚
                limbSolverTarget.DOMove(groundPoint, 0.2f);
            });
        }
    }
}
