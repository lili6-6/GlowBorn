using System.Collections;
using UnityEngine;

namespace shootstar
{


    public class Property_Web : Property_base
    {
        [SerializeField] public LineRenderer Web;
        private int UpTransDamage;
        private bool isUpTrans = false;
        [HideInInspector]public bool isWebActive = false;
        [SerializeField]public bool pauseWeb = false;
        //[SerializeField] public float burnedDuration=2f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
            UpTransDamage = this.GetComponent<TrapDamage>().DamageAmount * 2;
            shootingstarGameManager.Instance.webManager.webs.Add(this);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        public override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (collision.CompareTag("cLight"))
            {
                collision.GetComponent<BeCollect>().enabled = false;
                if (collision.GetComponent<Flicker>().isActiveAndEnabled)
                {
                    collision.GetComponent<Flicker>().OnDisable();
                }
                
                collision.GetComponent<WebShooter>().animator.SetTrigger("BeFixed");
            }
            if (collision.tag == "Player")
            {
                shootingstarGameManager.Instance.webManager.isInWeb = true;
                isWebActive= true;
                if (isUpTrans)
                {
                    int temp = shootingstarGameManager.Instance.uiManager.cLightNum -= 1;
                    shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
                }
                   
            }
        }
        public override void OnTriggerExit2D(Collider2D collision)
        {
         base.OnTriggerExit2D(collision);
            if(collision.tag=="cLight")
            {
                //StartCoroutine(UpTrans());
                UpTrans();
                if (collision != null&&collision.GetComponent<WebShooter>()!=null)
                {
                    collision.GetComponent<WebShooter>().animator.SetTrigger("BreakFree");
                }
            }
            if (collision.tag == "Player")
            {
                shootingstarGameManager.Instance.webManager.isInWeb = false;
                isWebActive= false;
            }

        }
        //Éý¼¶
        public void UpTrans()
        {
            CurrentState = PropertyState.Activated;
            if(property_Animation!= null)
                property_Animation.ChangeAnimation();
            //yield return new WaitForSeconds(burnedDuration);
            //this.gameObject.SetActive(false);
            this.GetComponent<TrapDamage>().DamageAmount = UpTransDamage;
            isUpTrans = true;
        }
    }
}