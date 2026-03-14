//using UnityEngine;

//namespace shootstar
//{
//    public class MultiSegmentVineUltimate : MonoBehaviour
//    {
//        [Header("藤蔓段")]
//        public Rigidbody2D[] vineSegments; // 多段链条
//        public float swingForce = 5f;      // 左右键荡藤蔓施加力
//        public float grabRange = 1.5f;     // 玩家抓取范围

//        [Header("挂点设置")]
//        public float playerFollowSpeed = 15f; // 玩家跟随挂点速度

//        private Rigidbody2D grabbedSegment;   // 玩家抓取的链条段
//        private GameObject grabPoint;         // 挂点
//        private Transform playerTransform;    // 玩家 Transform
//        private bool isGrabbing = false;

//        void Update()
//        {
//            if (isGrabbing && grabbedSegment != null && grabPoint != null && playerTransform != null)
//            {
//                // 左右键给抓取段施加力荡藤蔓
//                float h = Input.GetAxis("Horizontal");
//                grabbedSegment.AddForce(new Vector2(h * swingForce, 0f), ForceMode2D.Force);

//                // 玩家跟随挂点插值移动（脱离物理）
//                playerTransform.position = Vector3.Lerp(playerTransform.position, grabPoint.transform.position, playerFollowSpeed * Time.deltaTime);

//                // 松手
//                if (Input.GetKeyDown(KeyCode.Space))
//                {
//                    ReleaseVine();
//                }
//            }
//        }

//        /// <summary>
//        /// 玩家尝试抓取藤蔓
//        /// </summary>
//        /// <param name="player">玩家 Transform</param>
//        public void TryGrab(Transform player)
//        {
//            if (isGrabbing) return;

//            grabbedSegment = GetClosestSegment(player.position);
//            if (grabbedSegment == null) return;

//            float dist = Vector2.Distance(player.position, grabbedSegment.position);
//            if (dist > grabRange) return;

//            // 创建挂点（Kinematic，不依赖链条物理）
//            grabPoint = new GameObject("VineGrabPoint");
//            grabPoint.transform.position = grabbedSegment.position;
//            Rigidbody2D grabRb = grabPoint.AddComponent<Rigidbody2D>();
//            grabRb.isKinematic = true;

//            playerTransform = player;
//            isGrabbing = true;
//        }

//        /// <summary>
//        /// 松手藤蔓
//        /// </summary>
//        public void ReleaseVine()
//        {
//            if (grabPoint != null)
//            {
//                Destroy(grabPoint);
//                grabPoint = null;
//            }
//            grabbedSegment = null;
//            playerTransform = null;
//            isGrabbing = false;
//        }

//        /// <summary>
//        /// 获取离玩家最近的藤蔓段
//        /// </summary>
//        private Rigidbody2D GetClosestSegment(Vector2 playerPos)
//        {
//            if (vineSegments == null || vineSegments.Length == 0) return null;

//            Rigidbody2D closest = vineSegments[0];
//            float minDist = Vector2.Distance(playerPos, closest.position);

//            foreach (var seg in vineSegments)
//            {
//                float dist = Vector2.Distance(playerPos, seg.position);
//                if (dist < minDist)
//                {
//                    closest = seg;
//                    minDist = dist;
//                }
//            }
//            return closest;
//        }
//    }
//}
