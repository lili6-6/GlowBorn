using MoreMountains.CorgiEngine;
using UnityEngine;
using BehaviorDesigner.Runtime;

namespace shootstar
{


    public class Character_boss : Character_base
    {
        [SerializeField] public BehaviorTree bossBrain;
        private Rigidbody2D rb; // 刚体引用

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError($"[{gameObject.name}] 未找到Rigidbody2D组件！", this);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (CurrentState == _CharacterStates.Die)
            {
                Debug.Log("Boss 死亡，停止行为树");
                bossBrain.RestartWhenComplete = false;
            }
           

        }
    }
}