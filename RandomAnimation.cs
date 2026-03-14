//using UnityEngine;
//using System.Collections;
//using UnityEngine.UIElements;

//namespace shootstar
//{
//    public class RandomAnimation : MonoBehaviour
//    {
//        [Header("Animals Settings")]
//        [SerializeField] private GameObject[] animalsPrefab;
//        [SerializeField] private float minDistanceFromPlayer = 6f;
//        [SerializeField] private float maxDistanceFromPlayer = 10f;
//        [SerializeField] private float spawnYOffset = 2f; // 相对玩家的Y偏移
//        [SerializeField] private float minInterval = 5f;
//        [SerializeField] private float maxInterval = 12f;
//        [SerializeField] private float animalLifetime = 5f;

//        private GameObject player;
//        private Coroutine spawnRoutine;

//        private void OnEnable()
//        {
//            // 确保始终有协程在跑
//            spawnRoutine = StartCoroutine(RandomAnimalRoutine());
//        }

//        private void OnDisable()
//        {
//            if (spawnRoutine != null)
//                StopCoroutine(spawnRoutine);
//        }

//        private IEnumerator RandomAnimalRoutine()
//        {
//            while (true)
//            {
//                // 确保实时获取玩家引用（防止玩家被重生等情况）
//                if (player == null)
//                {
//                    if (shootingstarGameManager.Instance != null)
//                        player = shootingstarGameManager.Instance.Player;
//                }

//                // 如果仍然找不到就等一帧
//                if (player == null)
//                {
//                    yield return null;
//                    continue;
//                }

//                // 随机等待时间
//                float waitTime = Random.Range(minInterval, maxInterval);
//                yield return new WaitForSeconds(waitTime);

//                // 玩家移动时才触发
//                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
//                //if (rb != null && Mathf.Abs(rb.linearVelocity.x) < 0.2f)
//                //    continue;

//                SpawnAnimalNearPlayer();
//            }
//        }

//        private void SpawnAnimalNearPlayer()
//        {
//            if (animalsPrefab.Length == 0 || player == null) return;

//            int randomIndex = Random.Range(0, animalsPrefab.Length);
//            GameObject prefab = animalsPrefab[randomIndex];

//            // 随机左右方向
//            float offsetX = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
//            if (Random.value < 0.5f) // 50%概率取负值
//                offsetX = -offsetX;

//            // 实时取玩家当前世界坐标
//            Vector3 playerPos = player.transform.position;
//            Vector3 spawnPos = new Vector3(playerPos.x + offsetX, playerPos.y + spawnYOffset, 0f);

//            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);
//            //Debug.Log(instance.name + " 出现了！"+spawnPos);
//           // 朝向
//            if (instance.TryGetComponent<SpriteRenderer>(out var sr))
//                sr.flipX = offsetX < 0;

//            // 播放动画
//            if (instance.TryGetComponent<Animator>(out var anim))
//                anim.SetTrigger("Idle");

//            Destroy(instance, animalLifetime);
//        }
//    }
//}
using UnityEngine;
using System.Collections;

namespace shootstar
{
    public class RandomAnimation : MonoBehaviour
    {
        [Header("Animals Settings")]
        [SerializeField] private GameObject[] animalsPrefab;
        [SerializeField] private float minDistanceFromPlayer = 2f;
        [SerializeField] private float maxDistanceFromPlayer = 6f;
        [SerializeField] private float minInterval = 5f;
        [SerializeField] private float maxInterval = 12f;
        [SerializeField] private float animalLifetime = 5f;

        [Header("Camera Settings")]
        [HideInInspector] private Camera mainCamera; // 主相机引用
        [SerializeField] private float cameraEdgeBuffer = 1f; // 边缘缓冲，避免贴边生成

        private GameObject player;
        private Coroutine spawnRoutine;
        private Vector2 cameraBoundsMin;
        private Vector2 cameraBoundsMax;

        private void OnEnable()
        {
            mainCamera = shootingstarGameManager.Instance.cameraManager.mainCamera;
            // 获取主相机
            if (mainCamera == null)
                mainCamera = Camera.main;

            // 确保始终有协程在跑
            spawnRoutine = StartCoroutine(RandomAnimalRoutine());
        }

        private void OnDisable()
        {
            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);
        }

        private void UpdateCameraBounds()
        {
            // 计算相机可视范围的边界
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            // 添加边缘缓冲
            cameraBoundsMin = new Vector2(bottomLeft.x + cameraEdgeBuffer, bottomLeft.y + cameraEdgeBuffer);
            cameraBoundsMax = new Vector2(topRight.x - cameraEdgeBuffer, topRight.y - cameraEdgeBuffer);
        }

        private IEnumerator RandomAnimalRoutine()
        {
            while (true)
            {
                // 确保实时获取玩家引用（防止玩家被重生等情况）
                if (player == null)
                {
                    if (shootingstarGameManager.Instance != null)
                        player = shootingstarGameManager.Instance.Player;
                }

                // 如果仍然找不到就等一帧
                if (player == null)
                {
                    yield return null;
                    continue;
                }

                // 随机等待时间
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                // 玩家移动时才触发
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                //if (rb != null && Mathf.Abs(rb.linearVelocity.x) < 0.2f)
                //    continue;

                SpawnAnimalNearPlayer();
            }
        }

        private void SpawnAnimalNearPlayer()
        {
            if (animalsPrefab.Length == 0 || player == null || mainCamera == null) return;

            // 更新相机边界
            UpdateCameraBounds();

            int randomIndex = Random.Range(0, animalsPrefab.Length);
            GameObject prefab = animalsPrefab[randomIndex];

            // 生成围绕玩家的随机位置，但保持在相机范围内
            Vector3 spawnPos = GetRandomPositionAroundPlayerInCameraView();

            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);

            // 计算朝向（面向玩家）
            Vector3 directionToPlayer = player.transform.position - spawnPos;
            if (instance.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.flipX = directionToPlayer.x < 0; // 根据玩家位置翻转
            }

            // 播放动画
            if (instance.TryGetComponent<Animator>(out var anim))
                anim.SetTrigger("Idle");

            Destroy(instance, animalLifetime);
        }

        private Vector3 GetRandomPositionAroundPlayerInCameraView()
        {
            Vector3 playerPos = player.transform.position;
            Vector3 spawnPos = Vector3.zero;

            // 尝试生成有效的位置（最多尝试10次）
            for (int i = 0; i < 10; i++)
            {
                // 生成围绕玩家的随机方向和距离
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

                spawnPos = new Vector3(
                    playerPos.x + randomDirection.x * randomDistance,
                    playerPos.y + randomDirection.y * randomDistance,
                    0f
                );

                // 检查位置是否在相机范围内
                if (IsPositionInCameraBounds(spawnPos))
                {
                    break;
                }
            }

            // 确保位置在相机范围内（保底）
            spawnPos.x = Mathf.Clamp(spawnPos.x, cameraBoundsMin.x, cameraBoundsMax.x);
            spawnPos.y = Mathf.Clamp(spawnPos.y, cameraBoundsMin.y, cameraBoundsMax.y);

            return spawnPos;
        }

        private bool IsPositionInCameraBounds(Vector3 position)
        {
            return position.x >= cameraBoundsMin.x &&
                   position.x <= cameraBoundsMax.x &&
                   position.y >= cameraBoundsMin.y &&
                   position.y <= cameraBoundsMax.y;
        }
    }
}