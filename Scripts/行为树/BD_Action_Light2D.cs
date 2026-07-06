using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Unity.VisualScripting;

namespace shootstar
{

    [TaskCategory("Halabang")]
    public class BD_Action_Light2D : Action
    {
        public enum Light2DActionType
        {
            Intensity,
            Color,
            Range // 可选扩展
        }

        public Light2D targetLight;
        public Light2DActionType actionType = Light2DActionType.Intensity;
        public float targetIntensity;
        public Color targetColor;
        public float targetRange;
        public float duration;
        public Ease easetype;

        private bool isComplete=false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnStart()
        {
            TweenLight();

        }

        // Update is called once per frame
        public TaskStatus Update()
        {
            if (isComplete == false)
            {
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Success;

            }
        }
        public void TweenLight()
        {
            // 前置校验：避免空引用和非法参数
            if (targetLight == null)
            {
                //Debug.LogError("目标灯光组件未赋值！", this);
                return;
            }
            if (duration < 0)
            {
                //Debug.LogWarning("插值时长不能为负数，已修正为0.1f", this);
                duration = 0.1f;
            }

            if (actionType == Light2DActionType.Intensity)
            {
                // 核心修复：在value回调中给灯光亮度赋值
                DOTween.To(
                    () => targetLight.intensity, // 取值器：获取当前亮度
                    value => targetLight.intensity = value, // 赋值器：将插值结果赋给灯光（关键！）
                    targetIntensity, // 目标亮度
                    duration // 插值时长
                ).SetEase(easetype)
                 .SetUpdate(true) // 可选：不受Time.timeScale影响（比如暂停时仍生效）
                 .OnStart(() => Debug.Log($"开始插值灯光亮度：{targetLight.intensity} → {targetIntensity}"))
                 .OnComplete(() => isComplete=true);
            }
            // 可选扩展：补充其他灯光属性的插值（比如颜色、范围）
            else if (actionType == Light2DActionType.Color)
            {
                Color targetColor = Color.white; // 示例目标颜色，可改为可配置参数
                DOTween.To(
                    () => targetLight.color,
                    value => targetLight.color = value,
                    targetColor,
                    duration
                ).SetEase(easetype)
                .OnComplete(()=> isComplete=true);
            }
            else if (actionType == Light2DActionType.Range)
            {
                DOTween.To(
                () => targetLight.pointLightOuterRadius,
                value => targetLight.pointLightOuterRadius = value,
                targetRange,
                duration
            ).SetEase(easetype)
             //.OnStart(() => Debug.Log($"开始插值2D灯光范围：{targetLight.pointLightOuterRadius} → {targetRange}"))
             .OnComplete(() => isComplete=true);
            }
        }
    }
}