using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using DG.Tweening;
using MoreMountains.CorgiEngine;
using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using shootstar;
using System.Collections;


namespace shootstar
{
    public class colliderManager : MonoBehaviour
    {
        //public GameObject Player;
        //public GameObject collectLight;
        public Light2D PlayerLight;

        [SerializeField] private float pausedTime;
        [Header("重力")]
        public float duration = 0.1f;
        public float targetGravity = -30f;
        public Ease type = Ease.InOutSine;

        [HideInInspector]public string Inter;

        public static colliderManager Instance { get; private set; }
        [HideInInspector]public CinemachinePositionComposer cinemachinePositionComposer;

        private float accumulatedRadius = 1f; // 初始光照半径

        //private CallJumpOnceNode CallJumpOnceNode;
        private int originJumpsNumber;
        private bool originInput;
        private ChangeColorManager ChangeColorManager;

        private float gravity;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            shootingstarGameManager.Instance.Player = this.gameObject;
        }
        void Start()
        {
            
            StealthSkill.Instance.Player = this.gameObject;

            ChangeColorManager=GetComponent<ChangeColorManager>();
        }

        // Update is called once per frame
        void Update()
        {
            shootingstarGameManager.Instance.Player = this.gameObject;
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            

            if (other.gameObject.tag == "ReverseZone")
            {
                Debug.Log("进入黑白界");
                this.GetComponent< FireworkSkill>().SetReverseState(true);
                this.GetComponent< StealthSkill>().isInReverseZone = true;
                //this.GetComponent<ColorAbility_ChargeExtension>().useReverseZone=true;

                DOTween.To(
                () => other.GetComponent<GravityZone>().ControllerParameters.Gravity,
                x => other.GetComponent<GravityZone>().ControllerParameters.Gravity = x,
                targetGravity,
                duration
               ).SetEase(type);

            }
            if(other.gameObject.tag=="TransOut")
            {
                Debug.Log("传送点");
               // eventManager.Instance.transOut.Invoke();
                Inter= other.gameObject.tag;
            }
           
            if (other.gameObject.tag == "mushroom")
            {
                if (ChangeColorManager.currentColor == ColorType.Green)
                {
                    if (other.gameObject.GetComponent<Health>() == null)
                    {
                        GetComponent<ColorAbility>().currentMushroom = other.gameObject.transform.parent.gameObject;
                        Debug.Log(other.gameObject.transform.parent.gameObject);
                    }
                    else
                    {
                        
                        GetComponent<ColorAbility>().currentMushroom = other.gameObject;
                    }
                   
                    Debug.Log("蘑菇");
                   // eventManager.Instance.meetMushroom.Invoke();
                    Inter = other.gameObject.tag;
                }
                else
                {
                    Debug.Log("光源不对，无法弹跳");
                }

            }
            if (other.gameObject.tag == "Obstacle")
            {
                Debug.Log("碰到墙壁，不能贴墙");
                this.gameObject.GetComponent<CharacterWallClinging>().enabled=false;
            }
            if (other.gameObject.tag == "Water")
            {
                ColorAbility.Instance.waterBreath.SetActive(true);
               AudioManager.Instance.waterAudio.Play();
                GetComponent<CharacterSwim>().InWater=true ;
               
                
            }
            if(other.gameObject.tag== "cliff"||other.gameObject.tag=="Obstacle")
            {
                switch(ChangeColorManager.currentColor)
                {
                    case ColorType.White:
                       ColorAbility.Instance.whiteFade.SetActive(true);
                        break;
                    case ColorType.Red:
                        ColorAbility.Instance.redFade.SetActive(true);
                        break;
                    case ColorType.Blue:
                    ColorAbility.Instance.blueFade.SetActive(true); 
                        break;
                    case ColorType.Green:
                        ColorAbility.Instance.greenFade.SetActive(true);
                        break;
                }
            }
           
        }
        void OnTriggerStay2D(Collider2D other)
        {
           
        }
        public void OnTriggerExit2D(Collider2D other)
        {

            if (other.gameObject.tag == "TransIn")
            {
                    shootingstarGameManager.Instance.uiManager.interBut.Interactable(false);
                
            }
            if (other.gameObject.tag == "TransOut")
            {
                shootingstarGameManager.Instance.uiManager.interBut.Interactable(false);
                Inter = "";
            }
            if (other.gameObject.tag == "mushroom")
            {
                Debug.Log("离开蘑菇");
                //eventManager.Instance. exitMushroom.Invoke();
                //other.gameObject.GetComponent<TrapDamage>().canDamage = true;

                Inter = "";

            }
            if (other.gameObject.tag == "Obstacle")
            {
                Debug.Log("离开墙壁，可以贴墙");
                this.gameObject.GetComponent<CharacterWallClinging>().enabled = true;
            }
            if (other.gameObject.tag == "ReverseZone")
            {
                Debug.Log("退出黑白界");
                //this.gameObject.GetComponent<CharacterJump>().NumberOfJumps = originJumpsNumber;

                StealthSkill.Instance.isInReverseZone = false;

            }
            if (other.gameObject.tag == "Water")
            {
                ColorAbility.Instance.waterBreath.SetActive(false);
                GetComponent<CharacterSwim>().InWater = false;
            }
            if (other.gameObject.tag == "cliff"||other.gameObject.tag== "Obstacle")
            {
                ColorAbility.Instance.whiteFade.SetActive(false);
                ColorAbility.Instance.redFade.SetActive(false);
                ColorAbility.Instance.blueFade.SetActive(false);
                ColorAbility.Instance.greenFade.SetActive(false);
            }


        }
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "cliff" || collision.gameObject.tag == "Obstacle")
            {
                switch (ChangeColorManager.currentColor)
                {
                    case ColorType.White:
                        ColorAbility.Instance.whiteFade.SetActive(true);
                        break;
                    case ColorType.Red:
                        ColorAbility.Instance.redFade.SetActive(true);
                        break;
                    case ColorType.Blue:
                        ColorAbility.Instance.blueFade.SetActive(true);
                        break;
                    case ColorType.Green:
                        ColorAbility.Instance.greenFade.SetActive(true);
                        break;
                }
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "cliff" || collision.gameObject.tag == "Obstacle")
            {
                ColorAbility.Instance.whiteFade.SetActive(false);
                ColorAbility.Instance.redFade.SetActive(false);
                ColorAbility.Instance.blueFade.SetActive(false);
                ColorAbility.Instance.greenFade.SetActive(false);
            }
        }


        /// <summary>
        /// 光源收集，光照范围
        /// </summary>
        /// <param name="num"></param>
        public void OnCollectItem(float num)
        {
            Light2D li = PlayerLight.GetComponent<Light2D>();

            // 每次收集加 2
            accumulatedRadius += num;

            DOTween.To(
                () => li.pointLightOuterRadius,
                x => li.pointLightOuterRadius = x,
                accumulatedRadius,  // 累计的目标值
                0.5f                // 动画时长
            ).SetEase(Ease.InOutSine);
            if (li.pointLightOuterRadius < 1) li.pointLightOuterRadius = 1;
        }


        public IEnumerator Pause()
        {
            GetComponent<CharacterPause>().PauseCharacter();
            yield return new WaitForSeconds(pausedTime);
            GetComponent<CharacterPause>().UnPauseCharacter();
        }








        }

}
