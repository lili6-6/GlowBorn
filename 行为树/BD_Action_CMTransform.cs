using Unity.Cinemachine;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("设置 Cinemachine 摄像机优先级（Priority）以控制镜头切换。")]
    public class BD_Action_CMTransform : Action
    {
        [BehaviorDesigner.Runtime.Tasks.TooltipAttribute("目标 Cinemachine 摄像机")]
        public CinemachineCamera targetCamera;

        [BehaviorDesigner.Runtime.Tasks.TooltipAttribute("要设置的摄像机优先级")]
        public int targetPriority = 10;

        public override void OnStart()
        {
            if (targetCamera == null)
            {
                Debug.LogWarning("[BD_Action_CMTransform] 未指定目标摄像机。");
                return;
            }

            // 设置摄像机优先级
            targetCamera.Priority = targetPriority;
        }

        public override TaskStatus OnUpdate()
        {
            // 执行一次后立即返回成功
            return TaskStatus.Success;
        }
    }
}
