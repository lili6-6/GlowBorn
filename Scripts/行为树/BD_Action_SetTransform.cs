using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("直接设置Transform位置")]
    public class BD_Action_SetTransform : BehaviorDesigner.Runtime.Tasks.Action
    {
        [Tooltip("目标Transform组件")]
        public Transform target;

        public enum SetType
        {
            Position,
            Rotation,
            Scale,
            Everything
        }
        public SetType action;

        [Tooltip("是否包含子对象")]
        public bool includeChildren;

        [Tooltip("是否使用世界坐标")]
        public bool isWorldPosition = true;

        [Tooltip("目标位置")]
        public Vector3 targetPosition;

        [Tooltip("目标旋转")]
        public Vector3 targetRotation;

        [Tooltip("目标缩放")]
        public Vector3 targetScale = Vector3.one;

        private List<Transform> targetTransforms = new List<Transform>();

        public override void OnStart()
        {
            if (target == null)
            {
                Debug.LogError("目标Transform为null，无法设置Transform");
                return;
            }

            // 获取目标及其子对象的Transform组件
            Transform[] transforms = target.GetComponentsInChildren<Transform>(true);
            if (transforms != null && transforms.Length > 0)
            {
                if (includeChildren)
                {
                    targetTransforms.AddRange(transforms);
                }
                else
                {
                    // 只添加目标自身
                    targetTransforms.Add(transforms[0]);
                }
            }

            // 根据 action 执行不同的设置
            switch (action)
            {
                case SetType.Position:
                    SetPosition();
                    break;
                case SetType.Rotation:
                    SetRotation();
                    break;
                case SetType.Scale:
                    SetScale();
                    break;
                case SetType.Everything:
                    SetPosition();
                    SetRotation();
                    SetScale();
                    break;
            }
        }

        public override TaskStatus OnUpdate()
        {
            // 位置设置是瞬时完成的，直接返回成功
            return TaskStatus.Success;
        }

        private void SetPosition()
        {
            foreach (Transform t in targetTransforms)
            {
                if (isWorldPosition)
                {
                    t.position = targetPosition;  // 设置世界坐标
                }
                else
                {
                    t.localPosition = targetPosition;  // 设置本地坐标
                }
            }
        }

        private void SetRotation()
        {
            foreach (Transform t in targetTransforms)
            {
                if (isWorldPosition)
                {
                    t.eulerAngles = targetRotation;  // 设置世界旋转
                }
                else
                {
                    t.localEulerAngles = targetRotation;  // 设置本地旋转
                }
            }
        }

        private void SetScale()
        {
            foreach (Transform t in targetTransforms)
            {
                if (isWorldPosition)
                {
                    Debug.LogWarning("缩放只能设置为本地坐标，忽略世界坐标设置");
                    t.localScale = targetScale;  // 设置本地缩放
                }
                else
                {
                    t.localScale = targetScale;  // 设置本地缩放
                }
            }
        }
    }
}