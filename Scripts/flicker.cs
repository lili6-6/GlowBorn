using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace shootstar
{
    public class Flicker : MonoBehaviour
    {
        [SerializeField] private Light2D light2D;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private float flickerRange = 0.5f; // 闪烁的强度范围
        [SerializeField] private float floatRange = 0.5f;
        [SerializeField] private float scaleRange = 0.3f;
        [SerializeField] private float outerRange = 0.3f;

        private float originalIntensity; // 原始亮度
        private float originalPositionY; // 原始Y轴位置
        private float originalScale;
        private Tweener flickerTweener; // 动画控制器
        private Tweener floatTweener;

        void Start()
        {
            // 检查是否赋值了Light2D组件
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
                if (light2D == null)
                {
                    Debug.LogError("没有给Flicker脚本分配Light2D组件！");
                    return;
                }
            }

            // 保存初始值（使用Inspector中设置的实际值）
            originalIntensity = light2D.intensity;
            originalPositionY = transform.position.y;
            originalScale = transform.localScale.x;
            SetUniformRandomScale();

            //Debug.Log($"初始亮度: {originalIntensity}, 初始Y位置: {originalPositionY}");
            SetUniformRandomOuter();
            StartFlickering();
           
            
        }

        private void StartFlickering()
        {
            // 停止现有动画
            flickerTweener?.Kill();
            floatTweener?.Kill();

            // 计算亮度的上下限（基于原始值）
            float minIntensity = Mathf.Max(0, originalIntensity - flickerRange);
            float maxIntensity = originalIntensity + flickerRange;

            // 计算位置的上下限（基于原始值）
            float minPositionY = originalPositionY - floatRange;
            float maxPositionY = originalPositionY + floatRange;


            // 亮度闪烁动画（从当前值开始，在范围内来回）
            flickerTweener = DOTween.To(
                () => light2D.intensity,
                x => light2D.intensity = x,
                maxIntensity,
                duration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

            // 位置浮动动画（启用版本）
            floatTweener = DOTween.To(
                () => transform.position.y,
                y => transform.position = new Vector3(transform.position.x, y, transform.position.z),
                maxPositionY,
                duration * 5 // 位置浮动慢一点，更自然
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

          
        }
        private void SetUniformRandomScale()
        {
            // 仅生成一个随机浮点值，在最小和最大等比缩放之间
            float randomUniformValue = Random.Range(originalScale-scaleRange, originalScale+scaleRange);

            // 给XYZ轴设置相同的随机值，实现等比缩放
            transform.localScale = new Vector3(randomUniformValue, randomUniformValue, randomUniformValue);

            // 可选：控制台打印当前等比缩放值，方便调试查看
            Debug.Log($"启动时随机设置的等比缩放值：{randomUniformValue}（XYZ轴一致）");
        }
        private void SetUniformRandomOuter()
        {
            float randomUniformValue = Random.Range(light2D.pointLightOuterRadius - outerRange, light2D.pointLightOuterRadius + outerRange);
            light2D.pointLightOuterRadius = randomUniformValue;
            
        }

        // 禁用时停止动画并恢复初始值
        public void OnDisable()
        {
            flickerTweener?.Kill();
            floatTweener?.Kill();

            // 恢复初始状态
            if (light2D != null)
            {
                light2D.intensity = originalIntensity;
            }
            //transform.position = new Vector3(transform.position.x, originalPositionY, transform.position.z);
        }

        // 启用时重新开始
        private void OnEnable()
        {
            if (light2D != null)
            {
                StartFlickering();
            }
        }

        // 重置时恢复初始值
        private void OnReset()
        {
            light2D = GetComponent<Light2D>();
        }
    }
}