using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

[TaskCategory("Halabang")]
[TaskDescription("UI RectTransform 变换行为 (独立版，不依赖TweenSetting)")]
public class BD_Action_UITransform : Action
{
    public enum ACTION_TYPE
    {
        None,
        Move,
        Scale,
        Rotation
    }

    [Header("基本设置")]
    public ACTION_TYPE action;
    public RectTransform target;

    [Header("动画参数")]
    public float duration = 0.5f;
    public float delay = 0f;
    public int loopCycle = 0; // 0 表示不循环
    public LoopType loopType = LoopType.Restart;
    public Ease easeType = Ease.Linear;

    [Header("目标数值")]
    public Vector2 targetAnchoredPos;
    public Vector3 targetScale = Vector3.one;
    public Vector3 targetRotation;

    public override void OnStart()
    {
        if (target == null) return;

        switch (action)
        {
            case ACTION_TYPE.Move:
                target.DOAnchorPos(targetAnchoredPos, duration)
                    .SetDelay(delay)
                    .SetLoops(loopCycle, loopType)
                    .SetEase(easeType);
                break;

            case ACTION_TYPE.Scale:
                target.DOScale(targetScale, duration)
                    .SetDelay(delay)
                    .SetLoops(loopCycle, loopType)
                    .SetEase(easeType);
                break;

            case ACTION_TYPE.Rotation:
                target.DOLocalRotate(targetRotation, duration)
                    .SetDelay(delay)
                    .SetLoops(loopCycle, loopType)
                    .SetEase(easeType);
                break;
        }
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
