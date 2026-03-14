using UnityEngine;
using UnityEngine.UI;

namespace shootstar
{
    public class ColorAbility_ChargeExtension : MonoBehaviour
    {
        [Header("Ability")]
        [SerializeField] private ColorAbility colorAbility;

        [Header("Follow Target")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

        [Header("Reverse Support")]
        [SerializeField] private bool useReverseZone = true;

        private RectTransform rect;
        [HideInInspector]public CanvasGroup canvasGroup;
        private Canvas uiCanvas;
        private Camera uiCamera;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            SetVisible(false);
        }
        private void Start()
        {
           
            uiCanvas = shootingstarGameManager.Instance.uiManager.uiCanvas;
            uiCamera = shootingstarGameManager.Instance.cameraManager.uiCamera;

            Slider slider = GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
            Debug.Log(shootingstarGameManager.Instance.Player);
            if (shootingstarGameManager.Instance.Player != null)
            {
                shootingstarGameManager.Instance.Player.GetComponent<ColorAbility>().colorAbility_ChargeExtensions.Add(this);
            }
        }

        void Update()
        {
            if (followTarget == null)
                followTarget = shootingstarGameManager.Instance.Player.transform;
            if (colorAbility == null)
                colorAbility = shootingstarGameManager.Instance.Player
                                   .GetComponent<ColorAbility>();

            if (colorAbility == null) return;

            bool holding = colorAbility.isGreenHolding;

            SetVisible(holding);

            if (!holding) return;

            UpdatePosition();
            UpdateCharge();
        }

        void SetVisible(bool visible)
        {
            // ❗关键：不 SetActive
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }

        void UpdateCharge()
        {
            if (followTarget.gameObject.GetComponent<ColorAbility>().currentColor != ColorType.Green)
            {
                SetVisible(false);
                colorAbility.isGreenHolding=false;
                return;
            }
            // 你 Ability 里算好的进度
            GetComponent<Slider>().value = colorAbility.GreenChargePercent;
            Debug.Log(colorAbility.GreenChargePercent);
        }

        void UpdatePosition()
        {
            if (followTarget == null || uiCanvas == null)
                return;

            float yOffset = offset.y;

            if (useReverseZone &&
                colorAbility.TryGetComponent(out StealthSkill stealth) &&
                stealth.isInReverseZone)
            {
                yOffset = -Mathf.Abs(offset.y);
            }

            Vector3 worldPos = followTarget.position + new Vector3(offset.x, yOffset, offset.z);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // ⭐关键：根据 Canvas 模式决定 Camera
            Camera cam = null;
            if (uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cam = uiCanvas.worldCamera;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas.transform as RectTransform,
                screenPos,
                cam,
                out Vector2 uiPos
            );

            rect.localPosition = uiPos;
        }

    }
}
