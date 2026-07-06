//using UnityEngine;
//using DG.Tweening;
//using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
//using MoreMountains.CorgiEngine;
//using System.Collections;
//using UnityEngine.Events;

//namespace shootstar
//{
//    public class SpiderController : MonoBehaviour
//    {
//        [HideInInspector]public GameObject targetLight;
//        [HideInInspector] public GameObject targetWeb;
//        private GameObject Player;
//         private float jumpDuration;
//        private Tweener jumpTween;
//        private Vector3 targetPosition;
//       // [SerializeField]private Vector2 force;
//        [SerializeField] private Animator animator;
//        [SerializeField] private UnityEvent SpiderEvent;

//        private bool isTrans=false;


//        private void Start()
//        {
//            this.GetComponent<Transform>().rotation= Quaternion.Euler(0,0,180);
//            Player = shootingstarGameManager.Instance.Player;
//            if (isTrans)
//            {
//                //StartSpiderWalk(this.ToString()+"new");
//            }
//            PlayAni("Walk");


//        }

//        private void Update()
//        {
//            Player=shootingstarGameManager.Instance.Player;
//            if (targetWeb == null)
//            {
//                Debug.Log("web为空");
//                // 在SpiderController脚本中的对应位置
//                if (targetLight != null)
//                {
//                    Debug.Log("targetLight不为空，重新启用CorgiController");
//                    WebShooter webShooter = targetLight.GetComponent<WebShooter>();
//                    if (webShooter != null && webShooter.spiderWalk != null && webShooter.spiderWalk.IsActive())
//                    {
//                        Debug.Log(webShooter+".spiderWalk.Kill()");
//                        webShooter.spiderWalk.Kill();

//                    }
//                }
//                this.gameObject.GetComponent<CorgiController>().enabled = true;
//            }
//        }

//        private void OnTriggerEnter2D(Collider2D others)
//        {

//            if (others.gameObject.tag == "cLight")
//            {
//                Debug.Log("碰到光源");
//                //Debug.Log(targetLight.GetInstanceID());
//                //Debug.Log(others.gameObject.GetInstanceID());
//                if (others.gameObject.GetInstanceID() != targetLight.gameObject.GetInstanceID())
//                {
//                    return;
//                }
//                //Debug.Log("开始转化");
//               StartCoroutine (Trans());

//                //StartSpiderWalk(this.gameObject.ToString());
//            }
//            if (others.gameObject.tag == "Player")
//            {
//                StartCoroutine(hurtPlayer());
//            }
//            if(others.gameObject.tag=="bullet")
//            {
//               StartCoroutine(Destory());
//            }
//            if(others.gameObject.tag == "Ground")
//            {
//                SpiderEvent.Invoke();
//            }

//        }
//        public IEnumerator Trans()
//        {
//            PlayAni("Bite");
//            Debug.Log("chi");
//            targetLight.gameObject.GetComponent<WebShooter>().animator.SetTrigger("Trans");
//            yield return new WaitForSeconds(2f);
//            targetLight.gameObject.SetActive(false);
//            //Destroy(others);
//            GameObject newSpider = Instantiate(targetLight.gameObject.GetComponent<WebShooter>().newBurnSpider, targetLight.gameObject.GetComponent<WebShooter>().temp, Quaternion.identity);
//            newSpider.GetComponent<SpiderController>().isTrans = true;
//            this.GetComponent<CorgiController>().enabled = true;

//            //yield return new WaitForSeconds(3f);
//            //newSpider.GetComponent<SpiderController>().SpiderEvent.Invoke();
//            //SpiderEvent.Invoke();
//        }
//        private void OnTriggerExit2D(Collider2D collision)
//        {
//            if (collision.CompareTag("cLight"))
//            {
//                //collision.GetComponent<BeCollect>().enabled=true;
//                Debug.Log("离开光源");
//            }
//        }

//        public void PlayAni(string targetPara)
//        {
//            animator.SetBool("Walk", false);
//            animator.SetBool("Bite", false);
//            animator.SetBool("Die", false);
//            animator.SetBool("Fire", false);
//            animator.SetBool(targetPara,true);

//        }



//        void StartSpiderWalk(string user )
//        {
//            Debug.Log(user);
//            if (!Player) 
//            {
//                Debug.Log("没有Player"+ user);
//                return;
//            }
//            //this.GetComponent<c>
//            targetPosition=Player.transform.position;
//            Debug.Log("跳向玩家"+ user);
//            jumpTween= this.transform.DOMove(targetPosition, jumpDuration);
//            //this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down * force.y);
//            //this.gameObject.GetComponent<Rigidbody2D>().AddForce((targetPosition - this.transform.position).normalized * force.x);
//        }
//        public IEnumerator hurtPlayer()
//        {
//            jumpTween.Kill();
//            Debug.Log("伤害玩家");
//            yield return new WaitForSeconds(0.5f);
//            StartCoroutine(Destory());

//        }
//        public IEnumerator Destory()
//        {
//            PlayAni("Die");
//            yield return new WaitForSeconds(4f);
//            Destroy(gameObject);
//        }
//    }
//}
using UnityEngine;
using DG.Tweening;
using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine.Events;

namespace shootstar
{
    public class SpiderController : MonoBehaviour
    {
        [HideInInspector] public GameObject targetLight;
        [HideInInspector] public GameObject targetWeb;

        private GameObject Player;

        [Header("Move")]
        private Tweener jumpTween;
        private Vector3 targetPosition;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private UnityEvent SpiderEvent;

        private bool isTrans = false;

        private void Start()
        {
            // 初始朝下
            transform.rotation = Quaternion.Euler(0, 0, 180);

            Player = shootingstarGameManager.Instance.Player;

            PlayAni("Walk");
        }

        private void Update()
        {
            Player = shootingstarGameManager.Instance.Player;

            // ===== 蛛丝断裂 / 消失处理 =====
            if (targetWeb == null || !targetWeb.activeInHierarchy)
            {
                Debug.Log("蛛丝消失，蜘蛛恢复自由行动");

                if (targetLight != null)
                {
                    WebShooter webShooter = targetLight.GetComponent<WebShooter>();
                    //if (webShooter != null && webShooter.spiderWalk != null && webShooter.spiderWalk.IsActive())
                    //{
                    //    webShooter.spiderWalk.Kill();
                    //}
                }

                // 恢复 CorgiController
                var cc = GetComponent<CorgiController>();
                if (cc != null && !cc.enabled)
                    cc.enabled = true;

                targetWeb = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D others)
        {
            if (others.CompareTag("cLight"))
            {
                // 只响应自己的目标光源
                if (targetLight == null ||
                    others.gameObject.GetInstanceID() != targetLight.GetInstanceID())
                    return;

                StartCoroutine(Trans());
            }

            if (others.CompareTag("Player"))
            {
                StartCoroutine(HurtPlayer());
            }

            if (others.CompareTag("bullet"))
            {
                StartCoroutine(DestroySelf());
            }

            if (others.CompareTag("Ground"))
            {
                SpiderEvent?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("cLight"))
            {
                Debug.Log("蜘蛛离开光源");
            }
        }

        // ===== 转化逻辑 =====
        public IEnumerator Trans()
        {
            PlayAni("Bite");
            Debug.Log("蜘蛛开始转化");

            var shooter = targetLight.GetComponent<WebShooter>();
            if (shooter != null && shooter.animator != null)
                shooter.animator.SetTrigger("Trans");

            yield return new WaitForSeconds(2f);

            if (targetLight != null)
                targetLight.SetActive(false);

            if (shooter != null && shooter.newBurnSpider != null)
            {
                GameObject newSpider = Instantiate(
                    shooter.newBurnSpider,
                    shooter.temp,
                    Quaternion.identity
                );

                var sc = newSpider.GetComponent<SpiderController>();
                if (sc != null)
                    sc.isTrans = true;
            }

            var cc = GetComponent<CorgiController>();
            if (cc != null)
                cc.enabled = true;
        }

        // ===== 伤害玩家 =====
        public IEnumerator HurtPlayer()
        {
            if (jumpTween != null && jumpTween.IsActive())
                jumpTween.Kill();

            Debug.Log("蜘蛛伤害玩家");

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(DestroySelf());
        }

        // ===== 死亡 =====
        public IEnumerator DestroySelf()
        {
            PlayAni("Die");
            yield return new WaitForSeconds(4f);
            Destroy(gameObject);
        }

        // ===== 动画控制 =====
        public void PlayAni(string targetPara)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Bite", false);
            animator.SetBool("Die", false);
            animator.SetBool("Fire", false);

            animator.SetBool(targetPara, true);
        }
    }
}
