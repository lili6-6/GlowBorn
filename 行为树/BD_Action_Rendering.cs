using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine.Rendering;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("对指定 Volume 的权重进行平滑调整")]
    public class BD_Action_Rendering : Action
    {
        public enum ActionType
        {
            None,
            FadeIn,
            FadeOut,
            SetValue
        }

        [Header("Action Settings")]
        public ActionType actionType = ActionType.None;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("要操作的 Volume 对象")]
        public Volume targetVolume;

        [Header("Transition Settings")]
        [BehaviorDesigner.Runtime.Tasks.Tooltip("目标权重（仅在 SetValue 时使用）")]
        [Range(0f, 1f)] public float targetWeight = 1f;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("过渡时长（秒）")]
        public float duration = 1f;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("缓动类型")]
        public Ease easeType = Ease.InOutSine;

        private Tween tween;

        public override void OnStart()
        {
            if (targetVolume == null)
            {
                Debug.LogWarning("[BD_Action_Rendering] 未设置 targetVolume。");
                return;
            }

            float endValue = 1f;
            switch (actionType)
            {
                case ActionType.FadeIn:
                    endValue = 1f;
                    break;
                case ActionType.FadeOut:
                    endValue = 0f;
                    break;
                case ActionType.SetValue:
                    endValue = targetWeight;
                    break;
                default:
                    return;
            }

            // 直接 tween 到目标值
            tween?.Kill();
            tween = DOTween.To(() => targetVolume.weight, v => targetVolume.weight = v, endValue, duration)
                .SetEase(easeType);
        }

        public override TaskStatus OnUpdate()
        {
            if (tween == null || !tween.IsActive())
                return TaskStatus.Success;

            return tween.IsComplete() ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}
