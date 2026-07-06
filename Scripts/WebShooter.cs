
using DG.Tweening;
using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace shootstar
{
    public class WebShooter : MonoBehaviour
    {
        [SerializeField]public Animator animator;
        [SerializeField] public GameObject webPrefab; // 蛛丝的预制体
        private GameObject targetLight; // 目标光源对象
        [SerializeField] private float triggerDistance; // 触发蛛丝发射的距离
        private float distance; // 玩家与目标光源的距离
        [HideInInspector] public bool isFixed = false; // 是否已经固定（蛛丝是否已发射）
        [SerializeField] public GameObject spiderPrefab; // 蜘蛛的预制体
        [SerializeField] public GameObject newBurnSpider; // 燃烧状态蜘蛛的预制体
        [SerializeField] private Ease easeType; // DOTween 的缓动类型
        [SerializeField] private float duration; // 蛛丝延伸的持续时间
        [SerializeField] private float Jackduration = 2f; // 蜘蛛爬行的持续时间
        [SerializeField] private float shootDelay = 0f; // 发射延迟时间
        [SerializeField] private UnityEvent InRange;
        [Header("是否定点-光源")]
        [SerializeField] private bool isFixedPoint = false;
        [SerializeField] private Vector3 fixedPoint;
        [HideInInspector] public bool isCompleted = false; // 蛛丝是否完全延伸
        [HideInInspector] public GameObject spider; // 当前生成的蜘蛛对象
        private GameObject Player; // 玩家对象
        private Vector3 startPoint; // 蛛丝的起点
        [HideInInspector] public Tweener spiderWalk; // 蜘蛛爬行的 Tweener
        [HideInInspector] public Vector3 temp; // 蜘蛛目标位置
        public List<GameObject> targetWebs = new List<GameObject>(); // 当前生成的蛛丝对象列表

        [Header("蛛丝数量设置")]
        [SerializeField] private int webCount = 1; // 蛛丝数量
        [SerializeField] private float webSpacing = 0.5f; // 蛛丝之间的间距（用于定点模式）
        [SerializeField] private float angleRange = 15f; // 非定点模式下蛛丝的角度范围

        [HideInInspector] public bool bossUse=false;
        [HideInInspector]public bool spawnSpiders = true;

        void Start()
        {
            targetLight = this.gameObject; // 将当前对象设置为目标光源
        }

        void Update()
        {
            if (shootingstarGameManager.Instance.Player == null) return;

            //if(targetWebs == null)
            //{
            //   this.GetComponent<BeCollect>().enabled =true;
            //}

            if (isFixed || targetWebs.Count > 0) return;

            Player = shootingstarGameManager.Instance.Player;
            distance = Vector2.Distance(Player.transform.position, targetLight.transform.position);

            if (bossUse) return;
            if (distance < triggerDistance)
            {
                isFixed = true;            // 🔥 阻止重复触发
                StartCoroutine(shootWebs());
                InRange.Invoke();
            }
        }


        // 自动检测 fixedPoint 是屏幕坐标还是世界坐标：
        // - 如果 fixedPoint 在 [0, cam.pixelWidth] x [0, cam.pixelHeight] 范围内，视为屏幕坐标
        // - 否则视为世界坐标
        private Vector3 ResolveFixedPointWorld(Camera cam)
        {
            if (cam == null) return fixedPoint;

            float camZ = Mathf.Abs(cam.transform.position.z);

            bool looksLikeScreenCoord =
                fixedPoint.x >= 0 && fixedPoint.x <= cam.pixelWidth &&
                fixedPoint.y >= 0 && fixedPoint.y <= cam.pixelHeight;

            if (looksLikeScreenCoord)
            {
                Vector3 sp = new Vector3(fixedPoint.x, fixedPoint.y, camZ);
                Vector3 world = cam.ScreenToWorldPoint(sp);
                world.z = 0f;
                return world;
            }
            else
            {
                Vector3 w = fixedPoint;
                w.z = 0f;
                return w;
            }
        }

        // 发射多个蛛丝的协程
        public IEnumerator shootWebs()
        {
            Camera cam = Camera.main;
            if (cam == null || Player == null || targetLight == null || webPrefab == null) yield return null;

            float pixelMargin = 200f; // 屏幕外偏移量
            float camZ = Mathf.Abs(cam.transform.position.z);

            // 存储所有蛛丝的起点和终点
            List<Vector3> startPoints = new List<Vector3>();
            List<Vector3> endPoints = new List<Vector3>();

            // 计算每个蛛丝的起点和终点
            for (int i = 0; i < webCount; i++)
            {
                Vector3 startWorld;
                if (!isFixedPoint)
                {
                    // 右侧屏外随机起点（根据蛛丝数量调整位置）
                    float yOffset = (i - (webCount - 1) / 2f) * webSpacing;
                    Vector3 startScreen = new Vector3(
                        cam.pixelWidth + pixelMargin,
                        Random.Range(cam.pixelHeight * 0.2f, cam.pixelHeight * 0.8f) + yOffset,
                        camZ
                    );
                    startWorld = cam.ScreenToWorldPoint(startScreen);
                }
                else
                {
                    // 处理定点：自动判断 fixedPoint 是屏幕坐标还是世界坐标，并根据数量偏移
                    startWorld = ResolveFixedPointWorld(cam);
                    float yOffset = (i - (webCount - 1) / 2f) * webSpacing;
                    startWorld.y += yOffset;
                }

                startWorld.z = 0f;

                // 光源位置（世界坐标）
                Vector3 lightWorld = targetLight.transform.position;
                lightWorld.z = 0f;

                // 方向向量（从起点指向光源）
                Vector3 dir = (lightWorld - startWorld).normalized;

                // 角度范围限制（仅在 非定点 模式 下生效）
                float minAngle = 40f;
                float maxAngle = 85f;
                if (!isFixedPoint)
                {
                    float angleDeg = Mathf.Atan2(Mathf.Abs(dir.y), Mathf.Abs(dir.x)) * Mathf.Rad2Deg;

                    // 如果角度太小，随机分配一个范围内的角度（并重新计算起点指向）
                    if (angleDeg < minAngle)
                    {
                        float randomAngle = Random.Range(minAngle, maxAngle);
                        // 根据蛛丝索引调整角度
                        if (webCount > 1)
                        {
                            float angleOffset = (i - (webCount - 1) / 2f) * angleRange / (webCount - 1);
                            randomAngle += angleOffset;
                        }
                        float tanAngle = Mathf.Tan(randomAngle * Mathf.Deg2Rad);
                        float newY = Mathf.Sign(dir.y) * tanAngle * Mathf.Abs(dir.x);
                        dir.y = newY;
                        dir = dir.normalized;
                    }
                }
                else
                {
                    // 定点模式：确保 dir 是从 fixedPoint 指向光源（避免使用之前的 dir 导致偏移）
                    dir = (lightWorld - startWorld).normalized;
                }

                // 计算蛛丝延伸起点与终点（用摄像机视口大小作为长度估算）
                float worldHalfHeight = cam.orthographicSize;
                float worldHalfWidth = worldHalfHeight * cam.aspect;
                float totalLength = worldHalfWidth * 1f;

                Vector3 dirFromStartToLight = (lightWorld - startWorld).normalized;
                Vector3 leftPoint = startWorld;
                Vector3 rightPoint;

                if (isFixedPoint)
                {
                    // 从固定点穿过光源向另一侧延伸一段距离
                    rightPoint = lightWorld + dirFromStartToLight * totalLength;
                }
                else
                {
                    // 非定点：以 dir 为方向，从光源两侧延伸
                    rightPoint = lightWorld + dir * totalLength;
                    leftPoint = lightWorld - dir * totalLength;
                }

                startPoints.Add(leftPoint);
                endPoints.Add(rightPoint);
            }

            // 发射延迟
            yield return new WaitForSeconds(shootDelay);

            // 记录已完成的蛛丝数量
            int completedWebs = 0;

            // 生成所有蛛丝
            for (int i = 0; i < webCount; i++)
            {
                int webIndex = i; // 用于闭包
                Vector3 leftPoint = startPoints[i];
                Vector3 rightPoint = endPoints[i];

                // 实例化蛛丝
                GameObject webGO = Instantiate(webPrefab);
                Property_Web prop = webGO.GetComponent<Property_Web>();
                if (prop == null)
                {
                    Debug.LogWarning("webPrefab 缺少 Property_Web 组件");
                    Destroy(webGO);
                    continue;
                }

                LineRenderer lr = prop.Web;
                EdgeCollider2D ec = webGO.GetComponent<EdgeCollider2D>();
                if (lr == null) { Destroy(webGO); continue; }
                if (ec == null) { ec = webGO.AddComponent<EdgeCollider2D>(); }

                lr.useWorldSpace = true;
                lr.positionCount = 2;
                lr.SetPosition(0, leftPoint);
                lr.SetPosition(1, leftPoint);

                targetWebs.Add(webGO);

               


                // 设置初始碰撞体（四边形近似）
                UpdateEdgeColliderFromLine(lr, ec, 0.2f);

                // DOTween 延伸蛛丝（注意 OnComplete 的写法）
                DOTween.To(
                    () => lr.GetPosition(1),
                    p =>
                    {
                        lr.SetPosition(1, p);
                        UpdateEdgeColliderFromLine(lr, ec, 0.2f);
                    },
                    rightPoint,
                    duration
                ).SetEase(easeType).OnComplete(() =>
                {
                    completedWebs++;
                    // 最后一个蛛丝完成时才触发后续事件
                    if (completedWebs == webCount)
                    {
                        isCompleted = true;
                        shootingstarGameManager.Instance.cameraManager.ShakeCurrentCamera();
                        if (spawnSpiders)
                        {
                            SpawnSpider();
                        }
                           
                    }
                });

                // 如果需要蛛丝之间有间隔，可以添加延迟
                if (i < webCount - 1)
                {
                    yield return new WaitForSeconds(shootDelay / webCount);
                }
            }
        }

        // 从 LineRenderer 更新 EdgeCollider2D（将线宽转换成四点）
        private void UpdateEdgeColliderFromLine(LineRenderer lr, EdgeCollider2D ec, float lineWidth)
        {
            if (lr == null || ec == null) return;
            if (lr.positionCount < 2) return;

            Vector3 p0 = lr.GetPosition(0);
            Vector3 p1 = lr.GetPosition(1);
            Vector3 dir = (p1 - p0).normalized;
            Vector3 perpendicular = Vector3.Cross(dir, Vector3.forward) * lineWidth;

            Vector2[] quad = new Vector2[]
            {
                new Vector2(p0.x + perpendicular.x, p0.y + perpendicular.y),
                new Vector2(p1.x + perpendicular.x, p1.y + perpendicular.y),
                new Vector2(p1.x - perpendicular.x, p1.y - perpendicular.y),
                new Vector2(p0.x - perpendicular.x, p0.y - perpendicular.y)
            };

            ec.points = quad;
        }

        // 生成蜘蛛的方法
        private void SpawnSpider()
        {
            if (spiderPrefab == null) return;

            // 蜘蛛生成位置：定点模式直接用 fixedPoint 的世界坐标，否则用第一个蛛丝的起点并做一点垂直偏移
            Vector3 spawnPos;
            if (isFixedPoint)
            {
                Camera cam = Camera.main;
                spawnPos = ResolveFixedPointWorld(cam);
            }
            else if (targetWebs.Count > 0)
            {
                Property_Web firstWebProp = targetWebs[0].GetComponent<Property_Web>();
                if (firstWebProp != null && firstWebProp.Web != null)
                {
                    spawnPos = firstWebProp.Web.GetPosition(0);
                }
                else
                {
                    spawnPos = startPoint;
                }
                spawnPos.y -= 0.5f;
            }
            else
            {
                spawnPos = startPoint;
                spawnPos.y -= 0.5f;
            }

            spawnPos.z = 0f;

            spider = Instantiate(spiderPrefab, spawnPos, Quaternion.identity);
            var sc = spider.GetComponent<SpiderController>();
            if (sc != null)
            {
                sc.targetLight = this.gameObject;
                Debug.Log(sc+"目标光源已设置");
                // 给蜘蛛分配第一个蛛丝作为目标
                if (targetWebs.Count > 0)
                {
                    sc.targetWeb = targetWebs[0];
                }
            }

            temp = new Vector3(targetLight.transform.position.x, targetLight.transform.position.y - 0.5f, targetLight.transform.position.z);
            if (spiderWalk != null && spiderWalk.IsActive()) spiderWalk.Kill();
            spiderWalk = spider.transform.DOMove(temp, Jackduration).SetEase(Ease.Linear);
        }

        public void OnTriggerEnter2D(Collider2D others)
        {
            // 保留备用
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Web")
            {
                Debug.Log("蛛丝碰撞退出");
                targetWebs.Remove(collision.gameObject);
                if(targetWebs.Count == 0)
                {
                    Debug.Log("蛛丝全部消失，启用BeCollect");
                    if (this.gameObject != null)
                    {
                        this.GetComponent<BeCollect>().enabled = true;
                    }
                }
            }
        }
    }
}
