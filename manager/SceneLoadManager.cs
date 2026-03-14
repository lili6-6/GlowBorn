//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;
//using DG.Tweening;
//using System.Collections;
//using System.Diagnostics; // 引入Stopwatch所需命名空间

//namespace shootstar
//{
//    public class SceneLoadManager : MonoBehaviour
//    {
//        [Header("Fade Settings")]
//        [SerializeField] private float fadeDuration = 1.2f;
//        [SerializeField] private Ease fadeEase = Ease.InOutQuad;

//        [Header("UI")]
//        [SerializeField] private Image fadeImage;
//        [SerializeField] private Slider progressSlider;
//        [SerializeField] private TextMeshProUGUI progressText;

//        // 高精度计时器
//        private Stopwatch loadStopwatch;

//        private void Awake()
//        {
//            // 保证在多场景间不被销毁
//            DontDestroyOnLoad(gameObject);
//            InitUI();
//            // 初始化计时器
//            loadStopwatch = new Stopwatch();
//        }

//        /// <summary>
//        /// 加载下一个场景（BuildIndex + 1）
//        /// </summary>
//        public void LoadNextScene()
//        {
//            StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().buildIndex + 1));
//        }

//        /// <summary>
//        /// 加载指定场景
//        /// </summary>
//        public void LoadScene(int buildIndex)
//        {
//            StartCoroutine(LoadSceneCoroutine(buildIndex));
//        }

//        IEnumerator LoadSceneCoroutine(int buildIndex)
//        {
//            // 1. 重置并启动高精度计时器
//            loadStopwatch.Reset();
//            loadStopwatch.Start();

//            // 2. 初始化UI（增加空值保护）
//            InitUI();

//            // ---------- UI 激活（空值保护） ----------
//            if (fadeImage != null) fadeImage.gameObject.SetActive(true);
//            // 进度条先不激活，等淡出完成后再显示
//            if (progressSlider != null) progressSlider.gameObject.SetActive(false);
//            if (progressText != null) progressText.gameObject.SetActive(false);

//            // ---------- Fade Out（异步等待，避免阻塞） ----------
//            LogTime($"开始加载场景 {buildIndex}", loadStopwatch);
//            if (fadeImage != null)
//            {
//                fadeImage.color = Color.clear;
//                // 关键：加 yield return 异步等待动画完成，不阻塞主线程
//                fadeImage.DOColor(Color.black, fadeDuration)
//                    .SetEase(fadeEase)
//                    .SetUpdate(true); // 即使Time.timeScale=0也能运行
//                yield return new WaitForSeconds(fadeDuration);
//            }
//            LogTime("淡出完成", loadStopwatch);

//            // ---------- 激活进度条UI（淡出后再显示） ----------
//            if (progressSlider != null)
//            {
//                progressSlider.gameObject.SetActive(true);
//                progressSlider.value = 0f; // 重置进度条初始值
//            }
//            if (progressText != null)
//            {
//                progressText.gameObject.SetActive(true);
//                progressText.text = "0%"; // 重置进度文本
//            }

//            // ---------- 异步加载场景 ----------
//            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
//            operation.allowSceneActivation = false;

//            float displayedProgress = 0f;
//            while (operation.progress < 0.9f)
//            {
//                if (progressSlider == null || progressText == null) break;

//                // 计算目标进度（0~1）
//                float targetProgress = operation.progress / 0.9f;
//                // 单一平滑逻辑：用 MoveTowards 渐进更新，避免双重平滑
//                displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 2);
//                // 直接赋值进度条，不用DOTween（避免冗余）
//                progressSlider.value = displayedProgress;
//                progressText.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";

//                yield return null; // 每帧更新一次进度
//            }
//            LogTime("场景加载到90%", loadStopwatch);

//            // ---------- 补满进度条 ----------
//            if (progressSlider != null && progressText != null)
//            {
//                // 平滑补满进度（异步等待）
//                progressSlider.DOValue(1f, 0.2f)
//                      .SetEase(Ease.Linear)
//                      .OnComplete(() => { progressText.text = "100%"; });
//                yield return new WaitForSeconds(0.2f); // 等待补满动画完成
//            }
//            yield return new WaitForSeconds(0.1f); // 停留0.1秒，让玩家看到100%

//            // ---------- 激活场景 ----------
//            operation.allowSceneActivation = true;
//            LogTime($"场景 {buildIndex} 加载完成，切换前", loadStopwatch);
//            yield return new WaitUntil(() => operation.isDone);
//            LogTime($"场景 {buildIndex} 加载完成，切换完成", loadStopwatch);

//            // ---------- Fade In ----------
//            // 隐藏进度条
//            if (progressSlider != null) progressSlider.gameObject.SetActive(false);
//            if (progressText != null) progressText.gameObject.SetActive(false);

//            // 淡入动画（异步等待）
//            if (fadeImage != null)
//            {
//                fadeImage.DOColor(Color.clear, fadeDuration)
//                    .SetEase(fadeEase)
//                    .SetUpdate(true)
//                    .OnComplete(() => { fadeImage.gameObject.SetActive(false); });
//                yield return new WaitForSeconds(fadeDuration);
//            }

//            // ---------- 停止计时器并输出总耗时 ----------
//            loadStopwatch.Stop();
//            LogTime($"淡入完成，总耗时", loadStopwatch, isTotal: true);
//        }

//        /// <summary>
//        /// 格式化输出时间日志
//        /// </summary>
//        /// <param name="message">日志描述</param>
//        /// <param name="stopwatch">计时器实例</param>
//        /// <param name="isTotal">是否是总耗时</param>
//        private void LogTime(string message, Stopwatch stopwatch, bool isTotal = false)
//        {
//            // 获取耗时（秒，保留2位小数）
//            float elapsedSeconds = (float)stopwatch.Elapsed.TotalSeconds;
//            if (isTotal)
//            {
//                UnityEngine.Debug.Log($"【场景加载】{message}：{elapsedSeconds:F2}s");
//            }
//            else
//            {
//                UnityEngine.Debug.Log($"【场景加载】{message}，耗时：{elapsedSeconds:F2}s");
//            }
//        }

//        // 补充：确保 InitUI 方法有完整的空值保护（避免UI初始化报错）
//        void InitUI()
//        {
//            if (fadeImage != null)
//            {
//                fadeImage.color = Color.clear;
//                fadeImage.gameObject.SetActive(false);
//            }

//            if (progressSlider != null)
//            {
//                progressSlider.minValue = 0f;
//                progressSlider.maxValue = 1f;
//                progressSlider.value = 0f;
//                progressSlider.gameObject.SetActive(false);
//            }

//            if (progressText != null)
//            {
//                progressText.text = "0%";
//                progressText.gameObject.SetActive(false);
//            }
//        }
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Diagnostics;

namespace shootstar
{
    public class SceneLoadManager : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1.2f;
        [SerializeField] private Ease fadeEase = Ease.InOutQuad;

        [Header("Loading Settings")]
        [SerializeField] private float fakeProgressSpeed = 0.35f;
        [SerializeField] private float minLoadingTime = 1.2f;

        [Header("UI")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Progress Follow")]
        [SerializeField] private RectTransform followTarget; // 👈 跟随物体

        private RectTransform sliderRect;
        private float sliderWidth;

        private Stopwatch loadStopwatch;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitUI();
            loadStopwatch = new Stopwatch();

            if (progressSlider != null)
            {
                sliderRect = progressSlider.fillRect.parent.GetComponent<RectTransform>();
                sliderWidth = sliderRect.rect.width;
            }
        }

        public void LoadNextScene()
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void LoadScene(int buildIndex)
        {
            StartCoroutine(LoadSceneCoroutine(buildIndex));
        }

        private IEnumerator LoadSceneCoroutine(int buildIndex)
        {
            loadStopwatch.Reset();
            loadStopwatch.Start();

            InitUI();

            // ============ Fade Out ============
            if (fadeImage != null)
            {
                fadeImage.gameObject.SetActive(true);
                fadeImage.color = Color.clear;

                fadeImage.DOColor(Color.black, fadeDuration)
                    .SetEase(fadeEase)
                    .SetUpdate(true);

                yield return new WaitForSeconds(fadeDuration);
            }

            // ============ 显示进度 ============
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(true);
                progressSlider.value = 0f;
            }

            if (progressText != null)
            {
                progressText.gameObject.SetActive(true);
                progressText.text = "0%";
            }

            if (followTarget != null)
                followTarget.gameObject.SetActive(true);

            // ============ 异步加载 ============
            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
            operation.allowSceneActivation = false;

            float displayedProgress = 0f;
            float timer = 0f;

            while (operation.progress < 0.9f || timer < minLoadingTime)
            {
                timer += Time.deltaTime;

                displayedProgress += Time.deltaTime * fakeProgressSpeed;
                displayedProgress = Mathf.Min(displayedProgress, 0.9f);

                UpdateProgressUI(displayedProgress);

                yield return null;
            }

            // ============ 补满 ============
            if (progressSlider != null)
            {
                progressSlider.DOValue(1f, 0.25f).SetEase(Ease.Linear);
            }

            UpdateProgressUI(1f);
            yield return new WaitForSeconds(0.25f);

            operation.allowSceneActivation = true;
            yield return new WaitUntil(() => operation.isDone);

            // ============ Fade In ============
            if (progressSlider != null) progressSlider.gameObject.SetActive(false);
            if (progressText != null) progressText.gameObject.SetActive(false);
            if (followTarget != null) followTarget.gameObject.SetActive(false);

            if (fadeImage != null)
            {
                fadeImage.DOColor(Color.clear, fadeDuration)
                    .SetEase(fadeEase)
                    .SetUpdate(true)
                    .OnComplete(() => fadeImage.gameObject.SetActive(false));

                yield return new WaitForSeconds(fadeDuration);
            }

            loadStopwatch.Stop();
            UnityEngine.Debug.Log($"【场景加载】完成，总耗时 {loadStopwatch.Elapsed.TotalSeconds:F2}s");
        }

        // ================= 关键：统一更新进度 =================
        private void UpdateProgressUI(float progress)
        {
            if (progressSlider != null)
                progressSlider.value = progress;

            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            if (followTarget != null && sliderRect != null)
            {
                float x = Mathf.Lerp(
                    -sliderWidth / 2f,
                    sliderWidth / 2f,
                    progress
                );

                Vector2 pos = followTarget.anchoredPosition;
                pos.x = x;
                followTarget.anchoredPosition = pos;
            }
        }

        private void InitUI()
        {
            if (fadeImage != null)
            {
                fadeImage.color = Color.clear;
                fadeImage.gameObject.SetActive(false);
            }

            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
                progressSlider.value = 0f;
                progressSlider.gameObject.SetActive(false);
            }

            if (progressText != null)
            {
                progressText.text = "0%";
                progressText.gameObject.SetActive(false);
            }

            if (followTarget != null)
            {
                followTarget.gameObject.SetActive(false);
            }
        }
    }
}
