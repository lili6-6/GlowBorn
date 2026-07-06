using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Halabang.Plugin {
  [TaskCategory("Halabang")]
  [TaskDescription("执行动画相关的各种行为")]
  public class BD_Action_Animation : Action {
    public enum ACTION_NAME {
      NULL,
      ACTIVE_BOOL,
      ACTIVE_TRIGGER
    }

    public ACTION_NAME triggerAction;
    public Animator TargetAnimator;
    [Tooltip("使用逗号分割每个参数")]
    public string AnimatorParams;
    [Tooltip("激活或取消Bool类型的参数")]
    public bool BoolEnable;

    private string[] animatorParamsArray;

    public override void OnAwake() {
      if (triggerAction == ACTION_NAME.NULL) Debug.LogError(FriendlyName + " 必须选择一个行为");
      if (TargetAnimator == null) Debug.LogError(FriendlyName + " 必须绑定一个动画控制器组件");
      if (string.IsNullOrWhiteSpace(AnimatorParams)) Debug.LogError(FriendlyName + " 必须提供至少一个动画参数");

      animatorParamsArray = AnimatorParams.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
    }
    public override void OnStart() {
      callAction();
    }

    private void callAction() {
      switch (triggerAction) {
        case ACTION_NAME.ACTIVE_BOOL:
          foreach (string param in animatorParamsArray) {
            TargetAnimator.SetBool(param, BoolEnable);
          }
          break;
        case ACTION_NAME.ACTIVE_TRIGGER:
          foreach (string param in animatorParamsArray) {
            TargetAnimator.SetTrigger(param);
          }
          break;
      }
    }
  }
}
