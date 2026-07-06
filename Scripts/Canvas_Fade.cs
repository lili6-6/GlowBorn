using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Canvas_FadeInOut : MonoBehaviour
{
    [Tooltip("淡入持续时间（秒）")]
    public float fadeInDuration = 1f;

    [Tooltip("淡入后停留时间（秒）")]
    public float stayDuration = 0.5f;

    [Tooltip("淡出持续时间（秒）")]
    public float fadeOutDuration = 1f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // 确保CanvasGroup存在
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始设为完全透明（淡入起点）
        canvasGroup.alpha = 0;

        // 启动“淡入→停留→淡出”流程
        StartCoroutine(FadeInThenOut());
    }

    /// <summary>先淡入，再停留，最后淡出</summary>
    private IEnumerator FadeInThenOut()
    {
        // 1. 淡入（从0到1）
        canvasGroup.DOFade(1, fadeInDuration);
        yield return new WaitForSeconds(fadeInDuration); // 等待淡入完成

        // 2. 停留指定时间
        yield return new WaitForSeconds(stayDuration);

        // 3. 淡出（从1到0）
        canvasGroup.DOFade(0, fadeOutDuration);
        yield return new WaitForSeconds(fadeOutDuration); // 等待淡出完成

        // 可选：淡出后执行额外操作（如隐藏物体）
        // gameObject.SetActive(false);
    }

    // 可选：外部调用终止动画
    public void StopFade()
    {
        canvasGroup.DOKill();
        StopAllCoroutines();
    }
}