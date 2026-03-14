using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("Transform变换行为")]
    public class BD_Action_Transform : BehaviorDesigner.Runtime.Tasks.Action
    {
        [Flags]
        public enum ACTION_TYPE
        {
            Null = 1,
            Scale = 2,
            Position = 4,
            Rotation = 8
        }

        public ACTION_TYPE action;
        public Transform target;
        public bool includeChildren;
        public bool isWorldPosition;
        public Vector3 targetPosition;
        public bool isWorldRotation;
        public Vector3 targetRotation;
        public Vector3 targetScale = Vector3.one;

        [Header("Tween参数设置")]
        public float duration = 1f;                // 时长
        public float delay = 0f;                   // 延迟
        public int loopCycle = 0;                  // 循环次数，-1=无限
        public LoopType loopType = LoopType.Restart;
        public Ease easeType = Ease.Linear;        // 缓动方式

        private List<Transform> targetTransforms = new List<Transform>();

        public override void OnStart()
        {
            callAction();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        private void callAction()
        {
            targetTransforms.Clear();

            Transform[] transforms = target.GetComponentsInChildren<Transform>(true);
            if (transforms != null)
            {
                if (includeChildren)
                {
                    targetTransforms = transforms.ToList();
                }
                else
                {
                    targetTransforms.Add(transforms[0]);
                }
            }

            foreach (Transform t in targetTransforms)
            {
                if (action.HasFlag(ACTION_TYPE.Position))
                {
                    Tweener positionTweener;
                    if (isWorldPosition)
                    {
                        positionTweener = t.DOMove(targetPosition, duration);
                    }
                    else
                    {
                        positionTweener = t.DOLocalMove(targetPosition, duration);
                    }
                    positionTweener
                        .SetDelay(delay)
                        .SetLoops(loopCycle, loopType)
                        .SetEase(easeType);
                }

                if (action.HasFlag(ACTION_TYPE.Rotation))
                {
                    Tweener rotationTweener;
                    if (isWorldRotation)
                    {
                        rotationTweener = t.DORotate(targetRotation, duration);
                    }
                    else
                    {
                        rotationTweener = t.DOLocalRotate(targetRotation, duration);
                    }
                    rotationTweener
                        .SetDelay(delay)
                        .SetLoops(loopCycle, loopType)
                        .SetEase(easeType);
                }

                if (action.HasFlag(ACTION_TYPE.Scale))
                {
                    t.DOScale(targetScale, duration)
                        .SetDelay(delay)
                        .SetLoops(loopCycle, loopType)
                        .SetEase(easeType);
                }
            }
        }
    }
}
