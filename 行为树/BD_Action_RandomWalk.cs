using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System.ComponentModel;
using DG.Tweening;

namespace shootstar
{
    public enum TargetType
    {
        Character,
        Spider
    }
    [TaskCategory("Halabang")]
   
    public class BD_Action_RandomWalk : Action
    {
        [SerializeField] private GameObject target;
        [SerializeField]private float walkRadius = 5f;

        private bool walkOver=false;
        private Tweener walkTween;

        [SerializeField]public TargetType targetType= TargetType.Spider;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnStart()
        {
            if(target==null)
            {
                target=this.gameObject;
            }
            walkOver = false;
            walkTween.Kill();

            int randomDirection=Random.Range(0,2);//0:left 1:right
            float targetDir=randomDirection==0?-1:1;

            float randomDistance=Random.Range(1f,walkRadius);

            float targetPosition=target.transform.position.x+targetDir*randomDistance;
             walkTween=target.transform.DOMoveX(targetPosition, 2f)
                .OnComplete(()=> 
                {
                    walkOver= true;
                });
            if(targetType== TargetType.Spider)
            {
                target.GetComponent<SpiderController>().PlayAni("Walk");
            }
            else if(targetType== TargetType.Character)
            {
                target.GetComponent<Character_base>().CurrentState = _CharacterStates.Walk;
                target.GetComponent<Character_Animation>().ChangeAnimation(1f);
            }
            

        }

        // Update is called once per frame
        public override TaskStatus OnUpdate()
        {
            if(walkOver)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }

        }
    }
}