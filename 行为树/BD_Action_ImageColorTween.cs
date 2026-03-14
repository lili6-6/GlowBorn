using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// 行为树节点：使用DoTween实现Image颜色渐变动画
/// </summary>
[TaskCategory("Halabang")] // 节点分类（可自定义）
[TaskDescription("通过DoTween将Image组件的颜色从起始色过渡到目标色，完成后返回成功")]
public class BD_Action_ImageColorTween : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("目标Image组件（若为空则自动获取当前GameObject上的Image）")]
    public SharedGameObject targetImageObject; // 共享变量：目标Image所在的GameObject

    [BehaviorDesigner.Runtime.Tasks.Tooltip("动画开始时的颜色（默认使用当前Image的颜色）")]
    public Color startColor = Color.white;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("动画结束时的目标颜色")]
    public Color targetColor = Color.clear;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("动画持续时间（秒）")]
    public float duration = 1f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("动画缓动曲线")]
    public Ease easeType = Ease.Linear;

    private Image targetImage; // 目标Image组件
    private Tween colorTween; // DoTween动画实例
    private bool isTweenCompleted; // 动画是否完成


    public override void OnStart()
    {
        // 初始化目标Image组件
        if (targetImageObject.Value != null)
        {
            targetImage = targetImageObject.Value.GetComponent<Image>();
        }
        else
        {
            // 若未指定目标，则获取当前节点所在GameObject的Image组件
            targetImage = GetComponent<Image>();
        }

        // 校验Image组件是否存在
        if (targetImage == null)
        {
            Debug.LogError("ImageColorTween：未找到Image组件！");
            return;
        }

        // 若未指定起始色，则使用当前Image的颜色作为起始色
        if (startColor == Color.white && targetImage.color != startColor)
        {
            startColor = targetImage.color;
        }

        // 重置状态
        isTweenCompleted = false;

        // 启动DoTween颜色渐变动画
        colorTween = targetImage.DOColor(targetColor, duration)
            .From(startColor) // 从起始色开始
            .SetEase(easeType) // 设置缓动曲线
            .OnComplete(() => isTweenCompleted = true); // 动画完成时标记状态
    }


    public override TaskStatus OnUpdate()
    {
        // 若Image组件不存在，直接返回失败
        if (targetImage == null)
        {
            return TaskStatus.Failure;
        }

        // 动画未完成时，持续返回Running
        if (!isTweenCompleted)
        {
            return TaskStatus.Running;
        }

        // 动画完成，返回成功
        return TaskStatus.Success;
    }


    public override void OnEnd()
    {
        // 节点结束时终止动画（防止残留）
        if (colorTween != null && colorTween.IsActive())
        {
            colorTween.Kill();
        }
    }


   
}