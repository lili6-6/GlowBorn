using UnityEngine;
using MoreMountains;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace shootstar
{
    /// <summary>
    /// 颜色能力脚本
    /// </summary>

    public class ColorAbility : CharacterAbility
    {
        [SerializeField] public GameObject redEffect; // 红色光源特效
        [SerializeField] public GameObject greenEffect; // 绿色光源特效
        [SerializeField] public GameObject blueEffect; // 蓝色光源特效
        [Header("")]
        [SerializeField] public GameObject whiteFade;
        [SerializeField] public GameObject redFade;
        [SerializeField] public GameObject greenFade;
        [SerializeField] public GameObject blueFade;

        [SerializeField] public GameObject waterBreath;

        [SerializeField] private float redduration = 0.8f; // 特效持续时间
        [SerializeField] private float greenduration = 0.8f; // 特效持续时间
        [SerializeField] private float blueduration = 0.8f; // 特效持续时间
        private float duration;
        [Header("蓄力时间")]
        [SerializeField] private float greenChargeTime = 1.0f; // 绿色光源蓄力时间
       
        [Header("绿色持续治疗")]
        [SerializeField] private float greenHealInterval = 0.5f; // 每0.5秒回血一次
        [SerializeField] private int greenHealAmount = 5;        // 每次回多少血
        [SerializeField] private int greenLightCost = 1;         // 每次消耗光源
        [SerializeField] private float greenLightConsumeInterval = 1.5f;
        // 每 1.5 秒才消耗 1 个光源
       
        private float currentGreenHold = 0f;

        private Coroutine greenHealCoroutine;
        [HideInInspector]public bool isGreenHolding;


        [HideInInspector] public ColorType currentColor = ColorType.White; // 当前光源颜色
        [HideInInspector] public BeHelped BeHelped;

        private GameObject Player;
        private colliderManager collManager;
        [HideInInspector] public GameObject currentMushroom; // 当前交互的蘑菇对象

        public static ColorAbility Instance;
        [Header("技能音效")]
        [SerializeField] public AudioSource greenAudio;
        [SerializeField] public AudioSource blueAudio;
        [SerializeField] public AudioSource redAudio;
        [SerializeField] public AudioSource ExplosionAudio;

        private string _AbilityAnimationParameterName = "Ability";

        private bool isHelped=false;


        [HideInInspector] public List<ColorAbility_ChargeExtension> colorAbility_ChargeExtensions;
        //protected int _AbilityAnimationParameter;
        // 0 ~ 1
        public float GreenChargePercent
        {
            get
            {
                return Mathf.Clamp01(currentGreenHold / greenChargeTime);
            }
        }

        void Awake()
        {
            Instance = this;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {
            Player = shootingstarGameManager.Instance.Player;
            if (isGreenHolding)
            {
                currentGreenHold += Time.deltaTime;
                currentGreenHold = Mathf.Min(currentGreenHold, greenChargeTime);
            }
        }

        public void meetInter(float pressTime)
        {
            collManager = Player.GetComponent<colliderManager>();
            Debug.Log(collManager.Inter);
            //Debug.Log(Player.GetComponent<colliderManager>().Inter+"1111");
            // playerAnimator.SetTrigger("Interact");
            if (currentColor == ColorType.Green)
            {
                if (collManager.Inter == "mushroom")
                {
                    //Debug.Log("触发蘑菇事件");
                    //meetMushRoom(pressTime);
                    //collManager.Inter = "";
                }
                else if (collManager.Inter == "helpFriend")
                {

                    //Debug.Log("触发帮助小伙伴事件");
                    //BeHelped.meetHelpFriend(pressTime,greenChargeTime);
                    //collManager.Inter = "";
                }
                else
                {
                    //StartCoroutine(PlayEffects(greenEffect));
                    //Debug.Log("触发治疗事件");
                    //treat(pressTime);

                }
                
            }
            else if (currentColor == ColorType.Red)
            {
                if (GetComponent<CharacterSwim>().InWater)
                {
                    return;
                }

                FireworkSkill.Instance.ShootStraight();
            }
            else if (currentColor == ColorType.Blue)
            {

                StealthSkill.Instance.TryActivateStealth();
            }
            //技能动画
           StartCoroutine( PlayAbilityAnimation());

      

        }
        private void meetMushRoom(float pressTime)
        {
            if(pressTime< greenChargeTime)
            {
                return;
            }
           
            if (shootingstarGameManager.Instance.uiManager.cLightNum < 1)
            {
                Debug.Log("光源不足，无法治愈蘑菇");
                return;
            }
            // 检查蘑菇生命值是否已满（使用Health组件）
            if (currentMushroom != null)
            {
                Health mushroomHealth = currentMushroom.GetComponent<Health>();
                if (mushroomHealth != null && mushroomHealth.CurrentHealth >= mushroomHealth.MaximumHealth)
                {
                    Debug.Log("蘑菇生命值已满，无需治疗");
                    return;
                }
            }
            else
            {
                Debug.Log("未找到蘑菇对象");
                return;
            }
            int temp = shootingstarGameManager.Instance.uiManager.cLightNum -= 1;
            collManager.OnCollectItem(-0.5f);
            shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
            eventManager.Instance.mushroom.Invoke();

            //播放蘑菇治愈音效
            greenAudio.Play();
            // 治疗蘑菇（使用Health组件进行治疗，MMHealthBar会自动更新）
            Health mushroomHealthComponent = currentMushroom.GetComponent<Health>();
            if (mushroomHealthComponent != null)
            {
                mushroomHealthComponent.GetHealth(5, gameObject); // 恢复10点生命值
            }
            Debug.Log("治愈成功，可以进行交互");
        }
        //private void meetMushRoom()
        //{
        //    //// 检查光源数量
        //    //if (cLightNum < 1)
        //    //{
        //    //    Debug.Log("光源不足，无法治愈蘑菇");
        //    //    return;
        //    //}

        //    // 检查蘑菇生命值是否已满（使用Health组件）
        //    if (currentMushroom != null)
        //    {
        //        Health mushroomHealth = currentMushroom.GetComponent<Health>();
        //        if (mushroomHealth != null && mushroomHealth.CurrentHealth >= mushroomHealth.MaximumHealth)
        //        {
        //            Debug.Log("蘑菇生命值已满，无需治疗");
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("未找到蘑菇对象");
        //        return;
        //    }

        //    // 消耗光源
        //    cLightNum -= 1;
        //    colliderManager.Instance.OnCollectItem(-0.5f);
        //    UpdateLightText(cLightNum);

        //    // 治疗蘑菇（使用Health组件进行治疗，MMHealthBar会自动更新）
        //    Health mushroomHealthComponent = currentMushroom.GetComponent<Health>();
        //    if (mushroomHealthComponent != null)
        //    {
        //        mushroomHealthComponent.GetHealth(10, gameObject); // 恢复10点生命值
        //    }

        //    mushroom.Invoke();
        //    Debug.Log("治愈成功，蘑菇恢复10点生命值");
        //}

        private void treat(float pressTime)
        {
            if (pressTime < greenChargeTime)
            {
                return;
            }
            if (shootingstarGameManager.Instance.uiManager.cLightNum < 1)
            {
                Debug.Log("光源不足，无法治疗");
                return;
            }
            //播放音效
            greenAudio.Play();
            int temp = shootingstarGameManager.Instance.uiManager.cLightNum -= 1;
            shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
            collManager.OnCollectItem(-0.5f);
            //shootingstarGameManager.Instance. healthBar.GetComponent<Slider>().value += 10;
            //Player.GetComponent<PlayerHealth>().currentHealth += 10;
            //Debug.Log("治疗成功，当前生命值：" + Player.GetComponent<PlayerHealth>().currentHealth);
            if (PlayerHealthExtension.Instance.CurrentHealth >= PlayerHealthExtension.Instance.PlayerHealth.MaximumHealth)
            {
                Debug.Log("生命值已满，无需治疗");
                return;
            }
            // 进行治疗
            int healAmount = 10;
            PlayerHealthExtension.Instance.Heal(healAmount);

            Debug.Log($"治疗成功，消耗1个光源，恢复{healAmount}点生命值");
            Debug.Log("治疗成功，当前生命值：" + PlayerHealthExtension.Instance.CurrentHealth);
        }
        //private void treat()///-------
        //{
        //    //// 检查条件
        //    //if (cLightNum < 1)
        //    //{
        //    //    Debug.Log("光源不足，无法治疗");
        //    //    return;
        //    //}

        //    //if (PlayerHealthExtension.Instance == null)
        //    //{
        //    //    Debug.LogError("PlayerHealthExtension实例未找到");
        //    //    return;
        //    //}
        //    // 检查生命值是否已满
        //    if (PlayerHealthExtension.Instance.CurrentHealth >= PlayerHealthExtension.Instance.PlayerHealth.MaximumHealth)
        //    {
        //        Debug.Log("生命值已满，无需治疗");
        //        return;
        //    }
        //    // 消耗光源
        //    cLightNum -= 1;
        //    UpdateLightText(cLightNum);
        //    colliderManager.Instance.OnCollectItem(-0.5f);

        //    // 进行治疗
        //    int healAmount = 10;
        //    PlayerHealthExtension.Instance.Heal(healAmount);

        //    Debug.Log($"治疗成功，消耗1个光源，恢复{healAmount}点生命值");
        //    Debug.Log("治疗成功，当前生命值：" + PlayerHealthExtension.Instance.CurrentHealth);
        //}

        public IEnumerator PlayEffects(GameObject effects)
        {
            switch (effects)
            {
                case var _ when effects == redEffect:
                    duration = redduration;
                    break;
                case var _ when effects == greenEffect:
                    duration = greenduration;
                    break;
                case var _ when effects == blueEffect:
                    duration = blueduration;
                    break;
                default:
                    duration = 0.8f; // 默认持续时间
                    break;
            }

            if (effects != null)
            {
                effects.SetActive(true);
                yield return new WaitForSeconds(duration); // 假设特效持续1秒
                effects.SetActive(false);
            }
        }
     
        public IEnumerator PlayAbilityAnimation()
        {
            Animator animator=this.GetComponent<Character>().CharacterAnimator.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("【空引用】animator 没有赋值！", this); // 重点看这个
            }
            if (string.IsNullOrEmpty(_AbilityAnimationParameterName))
            {
                Debug.LogError("【空引用】动画参数名 _AbilityAnimationParameterName 为空！", this);
            }

            // 原报错代码
            animator.SetBool(_AbilityAnimationParameterName, true);
            yield return new WaitForSeconds(1f); // 假设动画持续0.5秒
            animator.SetBool(_AbilityAnimationParameterName, false);
        }
        private IEnumerator GreenHealLoop(GameObject target)
        {
            // ---------- 蓄力 ----------
            float chargeTimer = 0f;
            while (chargeTimer < greenChargeTime)
            {
                if (!isGreenHolding)
                {
                    greenHealCoroutine = null;
                    yield break;
                }

                chargeTimer += Time.deltaTime;
                yield return null;
            }

            greenAudio.Play();
            greenEffect.SetActive(true);
            BeHelped beHelped = target.GetComponent<BeHelped>();
            if (beHelped != null)
            {
                beHelped.OnGreenHealStart();
            }

            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth == null)
            {
                Debug.LogError("目标没有 Health 组件");
                greenHealCoroutine = null;
                yield break;
            }
            // ---------- 持续阶段 ----------
            float healTimer = 0f;
            float lightConsumeTimer = 0f;

            while (isGreenHolding)
            {
                healTimer += Time.deltaTime;
                lightConsumeTimer += Time.deltaTime;

                // 扣光源（独立节奏）
                if (lightConsumeTimer >= greenLightConsumeInterval)
                {
                    if (shootingstarGameManager.Instance.uiManager.cLightNum < greenLightCost)
                    {
                        Debug.Log("光源不足，停止绿色治疗");
                        break;
                    }

                    shootingstarGameManager.Instance.uiManager.cLightNum -= greenLightCost;
                    shootingstarGameManager.Instance.uiManager.UpdateLightText(
                        shootingstarGameManager.Instance.uiManager.cLightNum);
                    collManager.OnCollectItem(-0.5f);

                    lightConsumeTimer = 0f;
                }

                // 回血
                if (healTimer >= greenHealInterval)
                {
                    if (targetHealth.CurrentHealth >= targetHealth.MaximumHealth)
                    {
                        Debug.Log("目标已满血，停止治疗");
                        isGreenHolding = false;
                        break;
                    }

                    targetHealth.GetHealth(greenHealAmount, gameObject);
                    healTimer = 0f;
                }

                greenEffect.SetActive(false);
                yield return null;
            }

            // ---------- 结束判定 ----------
            if (target == currentMushroom &&
                targetHealth.CurrentHealth >= targetHealth.MaximumHealth*0.8f)
            {
                eventManager.Instance.mushroom.Invoke();
                Debug.Log("蘑菇完全治愈");
            }

            // Help 完成判定
            beHelped = target.GetComponent<BeHelped>();
            if (beHelped != null)
            {
                bool full = targetHealth.CurrentHealth >= targetHealth.MaximumHealth;
                beHelped.OnGreenHealStop(full);
            }

            if (beHelped != null &&
                targetHealth.CurrentHealth >= targetHealth.MaximumHealth&& isHelped==false)
            {
                target.GetComponent<BeHelped>().helpFriends.Invoke();
                //eventManager.Instance.BeHelped.Invoke();
                shootingstarGameManager.Instance.uiManager.UpdateFriendsText(shootingstarGameManager.Instance.uiManager.cFriendsNum += 1);
                Debug.Log("小伙伴完全治愈");
                isHelped = true;
            }

            greenHealCoroutine = null;

        }

        public void OnInteractDown()
        {
            if (currentColor != ColorType.Green) return;

            collManager = Player.GetComponent<colliderManager>();

            isGreenHolding = true;
            currentGreenHold = 0f;
            GameObject target = null;

            if (collManager.Inter == "mushroom" && currentMushroom != null)
            {
                target = currentMushroom;
            }
            else if (collManager.Inter == "helpFriend" && BeHelped != null)
            {
                target = BeHelped.gameObject;
            }

            else
            {
                target = Player;
            }
            if (greenHealCoroutine == null)
            {
                greenHealCoroutine = StartCoroutine(GreenHealLoop(target));
            }
        }

        public void OnInteractUp()
        {
            isGreenHolding = false;
            currentGreenHold = 0f;  
            if (greenHealCoroutine != null)
            {
                StopCoroutine(greenHealCoroutine);
                greenHealCoroutine = null;
            }
        }
        public void ResetSlider()
        {
            if(colorAbility_ChargeExtensions != null)
            {
                foreach(var extension in colorAbility_ChargeExtensions)
                {
                    Debug.Log("resetSlider");
                    extension.canvasGroup.alpha = 0f;
                }
            }

        }



    }
}