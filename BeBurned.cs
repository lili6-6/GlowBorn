using DG.Tweening;
using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace shootstar
{


    public class BeBurned : MonoBehaviour
    {
        [HideInInspector]private Animator animator;
        [SerializeField]private float duration=0.5f;
        [HideInInspector]public GameObject Light;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //animator=this.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
               if(collision.GetComponent<ChangeColorManager>().currentColor== ColorType.Red)
               {
                    StartCoroutine( Burned());
                }
            }
            if(collision.CompareTag("cLight"))
            {
                collision.GetComponent<BeCollect>().enabled=false;
            }
            if (collision.CompareTag("Monster"))
            {
                //collision.GetComponent<SpiderController>().targetLight = Light;
                // collision.GetComponent<CorgiController>().enabled=false;
            }
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.CompareTag("cLight"))
            {
                collision.GetComponent<BeCollect>().enabled=true;
            }
            if (collision.CompareTag("Monster"))
            {
                //Light.GetComponent<WebShooter>().spiderWalk.Kill();
               // collision.GetComponent<CorgiController>().enabled=true;
            }
        }
        public IEnumerator Burned()
        {
            animator.SetTrigger("BeBurned");
            yield return new WaitForSeconds(duration);

            //Light.GetComponent<BeCollect>().enabled = true;
            Destroy(gameObject);

            
        }
    }
}
