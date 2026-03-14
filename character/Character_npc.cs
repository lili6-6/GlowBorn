using UnityEngine;
using UnityEngine.Events;


namespace shootstar
{

    public class Character_npc  : Character_base
    {
        [Header("³ý¶¯»­")]
        public UnityEvent Enter;
        public UnityEvent Exit;

        protected override void Start()
        {
            
        }

        // Update is called once per frame
        protected override void Update()
        {

        }
        //public void OnColliderEnter2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Player"))
        //    {
        //        Debug.Log("Player entered the plant area");
        //        CurrentState = "trigger";
        //        property_Animation.ChangeAnimation();
        //    }
        //    Enter?.Invoke();
        //}
        //public void OnColliderExit2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Player"))
        //    {
        //        Debug.Log("Player exited the plant area");
        //        CurrentState = "Idle";
        //        property_Animation.ChangeAnimation();
        //    }
        //    Exit?.Invoke();
        //}
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Player entered the plant area");
                //CurrentState = _CharacterStates.
                //character_Animation.ChangeAnimation();
            }
            Enter?.Invoke();
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Player exited the plant area");
                CurrentState = _CharacterStates.Idle;
                character_Animation.ChangeAnimation();
            }
            Exit?.Invoke();
        }
    }
}