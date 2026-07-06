using BehaviorDesigner.Runtime.Tasks;
using shootstar;
using UnityEngine;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("转换场景")]
    public class BD_Action_TransScene : Action
    {
         private string targetScene; // 目标场景名称
         private bool isAsync = true; // 是否异步加载场景

        // 在行为树开始时调用
        public override void OnStart()
        {
            // 立即调用场景切换
            TransScene();
        }

        // 行为树的每帧更新
        public override TaskStatus OnUpdate()
        {
            // 一旦场景切换请求发出，就返回 Success
            return TaskStatus.Success;
        }

        // 切换场景的函数
        private void TransScene()
        {
            // 调用 GameSceneManager 来加载目标场景
            //GlobalGameManager.Instance.sceneLoadManager.LoadScene(targetScene, isAsync);
            GlobalGameManager.Instance.sceneLoadManager.LoadNextScene();
        }
    }
}
