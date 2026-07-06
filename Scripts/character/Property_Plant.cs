using UnityEngine;
using UnityEngine.Events;


namespace shootstar
{

    public class Property_Plant : Property_base
    {


        protected override void Start()
        {
            base.Start();
            
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            // Additional logic for when the player enters the plant area can be added here
             if(collision.tag=="bullet")
            {
                CurrentState = PropertyState.Activated ;
                if (property_Animation != null)
                    property_Animation.ChangeAnimation();
            }
        }

        public override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);
            // Additional logic for when the player exits the plant area can be added here
        }


    }
}