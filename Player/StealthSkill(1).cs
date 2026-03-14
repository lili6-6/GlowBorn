using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace shootstar
{
    public class StealthSkill : MonoBehaviour
    {
        [Header("Stealth Settings")]
        public float stealthDuration = 3f;//潜行时间
        //public float cooldownTime = 5f;//冷却时间
        public float stealthSpeedMultiplier = 1.5f;//移动倍率
        public Color stealthColor = new Color(1, 1, 1, 0.5f);

       // [Header("UI References")]
        [HideInInspector]public Slider durationSlider;          // 技能持续时间Slider
        [HideInInspector]public TMP_Text cooldownText;          // 冷却倒计时文本

        [Header("玩家")]
        public GameObject Player;
        [Header("Ghost Trail Settings")]
        public GameObject ghostPrefab;       // 残影预制体
        public float ghostInterval = 0.05f;  // 生成间隔
        public float ghostLifetime = 0.4f;   // 持续时间
        public float ghostAlpha = 0.5f;      // 初始透明度
        public bool enableGhostDuringStealth = true; // 只在潜行时启用残影
        [Header("Ghost Visual Tuning")]
        public Vector3 ghostOffset = Vector3.zero;   // 残影位置偏移（锚点）
        //[Header("残影锚点（可选）")]
        //public Transform ghostAnchor;


        private float ghostTimer = 0f;
        private SpriteRenderer[] playerSprites; // 保存所有sprite


        [HideInInspector] public ButtonManager stealthBut;

        [HideInInspector]public bool isStealthActive = false;
        [HideInInspector]public bool isOnCooldown = false;
        private Color originalColor;
        private float originalMoveSpeed;

        private float currentCooldownTime;
        private float currentDurationTime;

        public GameObject model;

        public static StealthSkill Instance; // 单例实例
        private CorgiController CorgiController;
        [HideInInspector]public TerrainProperty terrainProperty;
        private float terrainSpeedMultiplier;

        // [Header("Canvas 引用")]
        [HideInInspector] public Canvas uiCanvas; // 需要手动拖入你的UI Canvas

        [HideInInspector]public bool isInReverseZone = false;

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
        }

        void Start()
        {
            durationSlider=shootingstarGameManager.Instance.uiManager.durationSlider;
            //cooldownText=shootingstarGameManager.Instance.cooldownText;
            //stealthBut=shootingstarGameManager.Instance.stealthBut;
            uiCanvas=shootingstarGameManager.Instance.uiManager.uiCanvas;

            if (model != null)
                {
                    //originalColor = model.color;
                }
                else
                {
                    Debug.LogError("子对象上没有找到SpriteRenderer组件: " );
                }
            // 获取玩家的 Character 脚本
            CorgiController = LevelManager.Instance.Players[0].GetComponent<CorgiController>();
            originalMoveSpeed = CorgiController.DefaultParameters.SpeedFactor;

           

            // 初始化UI
            InitializeUI();

            // 获取所有SpriteRenderer（包括子物体）
            if (Player != null)
            {
                playerSprites = Player.GetComponentsInChildren<SpriteRenderer>();
            }

        }

        void InitializeUI()
        {
            if (durationSlider != null)
            {
                durationSlider.gameObject.SetActive(false);
                durationSlider.maxValue = stealthDuration;
                durationSlider.value = 0;
            }

            if (cooldownText != null)
            {
                cooldownText.text = "";
                cooldownText.gameObject.SetActive(false);
            }
        }

        void Update()
        {
          

            // UI刷新
            UpdateUI();

            if (enableGhostDuringStealth && isStealthActive)
            {
                ghostTimer += Time.deltaTime;
                if (ghostTimer >= ghostInterval)
                {
                    CreateGhost();
                    ghostTimer = 0f;
                }
            }


        }

        /// <summary>
        /// 技能触发逻辑（按钮和按键都调用这个方法）
        /// </summary>
        public void TryActivateStealth()
        {
            if (!isStealthActive && !isOnCooldown)
            {
                StartCoroutine(ActivateStealth());
            }
            
        }

        

        public IEnumerator ActivateStealth()
        {
            //stealthBut.Interactable(false);

            // 激活潜行状态
            isStealthActive = true;
            currentDurationTime = stealthDuration;
           

            //消耗光源
            if (shootingstarGameManager.Instance.uiManager.cLightNum > 2)
            {
                int temp = shootingstarGameManager.Instance.uiManager.cLightNum -= 2;
                colliderManager.Instance.OnCollectItem(-1f);
                shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
            }
            else
            {
                // 没有光源，无法进入潜行状态
                isStealthActive = false;
              //  stealthBut.Interactable(true);
                yield break;
            }
          
            // 显示持续时间 UI
            if (durationSlider != null)
            {
                durationSlider.gameObject.SetActive(true);
                durationSlider.maxValue = stealthDuration;
                durationSlider.value = stealthDuration;
            }

            // 改变子对象颜色
            if (model != null)
            {
               // model.color = stealthColor;
            }
            //播放特效
            StartCoroutine(ColorAbility.Instance.PlayEffects(ColorAbility.Instance.blueEffect));
            //播放音效
            ColorAbility.Instance.blueAudio.Play(); 
            // 修改移动速度
            if (isInReverseZone)
            {
                shootingstarGameManager.Instance.levelSceneManager. ReverseZone.GetComponent<GravityZone>().ControllerParameters.SpeedFactor *= stealthSpeedMultiplier;
            }
            else
            {
                terrainProperty= Player.GetComponent<ShadowController>().CurrentPlatForm.GetComponent<TerrainProperty>();
                terrainSpeedMultiplier = terrainProperty != null ? terrainProperty.moveMultiplier : 1f;
                
                CorgiController.DefaultParameters.SpeedFactor = originalMoveSpeed *  stealthSpeedMultiplier*terrainSpeedMultiplier ;
           Debug.Log("  "+ originalMoveSpeed + "  "+ terrainSpeedMultiplier);
            }

            // 计时循环
            while (currentDurationTime > 0)
            {
                currentDurationTime -= Time.deltaTime;

                if (durationSlider != null)
                {
                    durationSlider.value = currentDurationTime;

                    // === 1. 根据是否处于反转区域决定UI相对位置 ===
                    float yOffset = 1.5f; // 上下距离
                    if (isInReverseZone)
                    {
                        // 如果在反转区域，UI在玩家下方
                        yOffset = -1.5f;
                    }

                    // 世界坐标 + 偏移
                    Vector3 worldPos = Player.transform.position + new Vector3(0, yOffset, 0);

                    // 世界 -> 屏幕坐标
                    //Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                    // 屏幕 -> UI Canvas 本地坐标
                    //Vector2 uiPos;
                    //RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    //    uiCanvas.GetComponent<RectTransform>(),
                    //    screenPos,
                    //    null, // Overlay 模式
                    //    out uiPos
                    //);
                    // 获取 Canvas 的 RectTransform
                    RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();

                    // 获取 UI Camera （Canvas 上设置的 Render Camera）
                    Camera uiCam = uiCanvas.worldCamera;

                    // 世界坐标 -> 屏幕坐标（基于主相机）
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                    // 屏幕坐标 -> Canvas 本地坐标（必须传入 UI 相机）
                    Vector2 uiPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect,
                        screenPos,
                        uiCam,  // 这里很关键！！！
                        out uiPos
                    );

                    // 应用位置
                    durationSlider.GetComponent<RectTransform>().localPosition = uiPos;

                    // 4. 更新UI位置
                    durationSlider.GetComponent<RectTransform>().localPosition = uiPos;
                }

                yield return null;

            }
            ResetSteal();


           // StartCoroutine(StartCooldown());
        }
        public void ResetSteal()
        {
            // 恢复状态
            if (model != null)
            {
               // model.color = originalColor;
            }
            terrainProperty = Player.GetComponent<ShadowController>().CurrentPlatForm.GetComponent<TerrainProperty>();
            terrainSpeedMultiplier = terrainProperty != null ? terrainProperty.moveMultiplier : 1f;

            CorgiController.DefaultParameters.SpeedFactor = originalMoveSpeed*terrainSpeedMultiplier ;
           Debug.Log("Resetsteal:" + originalMoveSpeed+" "+ terrainSpeedMultiplier);
            shootingstarGameManager.Instance.levelSceneManager.ReverseZone.GetComponent<GravityZone>().ControllerParameters.SpeedFactor = originalMoveSpeed; 

            if (durationSlider != null)
            {
                durationSlider.gameObject.SetActive(false);
            }

            isStealthActive = false;
        }


        //IEnumerator StartCooldown()
        //{
        //    isOnCooldown = true;
        //    currentCooldownTime = cooldownTime;

        //    if (cooldownText != null) cooldownText.gameObject.SetActive(true);

        //    while (currentCooldownTime > 0)
        //    {
        //        currentCooldownTime -= Time.deltaTime;
        //        yield return null;
        //    }

        //    if (cooldownText != null) cooldownText.gameObject.SetActive(false);

        //    isOnCooldown = false;
        //    //stealthBut.Interactable(true);
        //}

        void UpdateUI()
        {
            // 更新持续时间Slider
            if (isStealthActive && durationSlider != null)
            {
                durationSlider.value = currentDurationTime;
            }

            //更新冷却文本
            //if (isOnCooldown && cooldownText != null)
            //{
            //    cooldownText.text = Mathf.CeilToInt(currentCooldownTime).ToString();
            //}
        }

       
        private void CreateGhost()
        {
            if (ghostPrefab == null || Player == null)
                return;

            // 根据玩家朝向决定X偏移
            float xOffset = Player.transform.localScale.x >= 0 ? ghostOffset.x : -ghostOffset.x;

            // 创建残影父对象
            GameObject ghostRoot = new GameObject("GhostRoot");
            ghostRoot.transform.position = Player.transform.position + new Vector3(xOffset, ghostOffset.y, ghostOffset.z);
            ghostRoot.transform.rotation = Player.transform.rotation;

            // 每次生成残影时都动态获取当前的SpriteRenderer
            // 使用true参数获取所有子对象，包括非激活的
            SpriteRenderer[] currentSprites = Player.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var spriteRenderer in currentSprites)
            {
                // 关键：检查SpriteRenderer是否真正可见
                // 1. 组件本身必须启用
                // 2. GameObject必须在层级中激活
                // 3. Sprite不能为空（防止显示空白）
                if (spriteRenderer == null ||
                    !spriteRenderer.enabled ||
                    !spriteRenderer.gameObject.activeInHierarchy ||
                    spriteRenderer.sprite == null)
                {
                    continue;
                }

                // 创建残影子对象
                GameObject ghostPart = Instantiate(ghostPrefab, ghostRoot.transform);

                // 计算局部位置（使用世界坐标转换）
                Vector3 localPos = ghostRoot.transform.InverseTransformPoint(spriteRenderer.transform.position);
                ghostPart.transform.localPosition = localPos;
                ghostPart.transform.rotation = spriteRenderer.transform.rotation;
                ghostPart.transform.localScale = spriteRenderer.transform.lossyScale;

                // 设置Sprite和渲染属性
                SpriteRenderer ghostSR = ghostPart.GetComponent<SpriteRenderer>();
                if (ghostSR != null)
                {
                    ghostSR.sprite = spriteRenderer.sprite;
                    ghostSR.flipX = spriteRenderer.flipX;
                    ghostSR.flipY = spriteRenderer.flipY;
                    ghostSR.sortingLayerID = spriteRenderer.sortingLayerID;
                    ghostSR.sortingOrder = spriteRenderer.sortingOrder - 1;
                    ghostSR.color = spriteRenderer.color; // 使用原始颜色

                    // 应用潜行透明度
                    Color stealthColorWithAlpha = ghostSR.color;
                    stealthColorWithAlpha.a = ghostAlpha;
                    ghostSR.color = stealthColorWithAlpha;
                }
            }

            // 如果没有创建任何残影部分，销毁空的父对象
            if (ghostRoot.transform.childCount == 0)
            {
                Destroy(ghostRoot);
                return;
            }

            // 渐隐销毁
            StartCoroutine(FadeAndDestroyGhost(ghostRoot));
        }

        private IEnumerator FadeAndDestroyGhost(GameObject ghostRoot)
        {
            float elapsed = 0f;
            SpriteRenderer[] srs = ghostRoot.GetComponentsInChildren<SpriteRenderer>();

            while (elapsed < ghostLifetime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(ghostAlpha, 0f, elapsed / ghostLifetime);

                foreach (var sr in srs)
                {
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = alpha;
                        sr.color = c;
                    }
                }

                yield return null;
            }

            Destroy(ghostRoot);
        }




       

    }
}
