using DG.Tweening;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using MoreMountains;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.Events;
namespace shootstar
{
    public enum ColorType
    {
        White,
        Red,
        Blue,
        Green
    }

    public class ChangeColorManager : CharacterAbility
    {
        [SerializeField] private Light2D li;

        [SerializeField] private Color32 red;
        [SerializeField] private Color32 blue;
        [SerializeField] private Color32 green;

       
        [HideInInspector] public GameObject Player; // 玩家
        [HideInInspector] private ButtonManager interBut;

        [SerializeField] private UnityEvent RedEvent;
        [SerializeField]private UnityEvent BlueEvent;
        [SerializeField] private UnityEvent GreenEvent;
        [SerializeField] private UnityEvent WhiteEvent;

        protected const string _ChangeColorAnimationParameterName = "ChangeColor";
        protected int _ChangeColorAnimationParameter;

       
        private float _ChangeColorAnimationLength = 1.5f; // 你的beHelped动画长度（秒）
        private float _animationTimer = 0f;    // 记录动画播放时间

        private bool _isPlayingChangeColor = false;
        [SerializeField] public AudioSource ChangeAudio;

        private Animator_extension animator_Extension;

        //事件
        public static event System.Action<ColorType> OnLightColorChanged;

        [HideInInspector] public ColorType currentColor { get; private set; } = ColorType.White; // 当前光源颜色 

        public ColorType CurrentColor => currentColor;

        public void ChangeColor(ColorType newColor)
        {
            if (currentColor == newColor) return;

            currentColor = newColor;

            // 这里以后可以加音效、特效、UI
            OnLightColorChanged?.Invoke(currentColor);
        }


        void Awake()
        {
            
            animator_Extension=this.GetComponent<Animator_extension>();
        }


        protected override void Initialization()
        {
            base.Initialization();
            _controller = _character.GetComponent<CorgiController>();
        }

        protected override void Start()
        {
            base.Start();
            interBut = shootingstarGameManager.Instance.uiManager.interBut;
            currentColor = ColorType.White;


        }

        // Update is called once per frame
        void Update()
        {
            Player = shootingstarGameManager.Instance.Player;
            if (_isPlayingChangeColor)
            {
                _animationTimer += Time.deltaTime;

                // 调试用：打印状态和时间（出问题时打开）
                // Debug.Log($"Animator state: {_animator.GetCurrentAnimatorStateInfo(0).IsName("beHelped")}, timer: {_animationTimer} / {_beHelpedAnimationLength}");

                if (_animationTimer >= _ChangeColorAnimationLength)
                {
                    _isPlayingChangeColor = false;
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    _animationTimer = 0f;
                }
            }
        }
        //public void ChangeColor()
        //{
        //    ChangeAudio.Play();
        //    if (currentColor == ColorType.White)
        //    {
        //        Debug.Log("变成红色");
        //        RedEvent.Invoke();
        //       Color32 targetColor = red;
        //        float originalRadius = li.pointLightOuterRadius;
        //        changeColor(li, targetColor, originalRadius);
        //        currentColor = ColorType.Red;
        //        interBut.Interactable(true);


        //        PlayAnimation();
        //    }
        //    else if (currentColor == ColorType.Red)
        //    {
        //        Debug.Log("变成蓝色");
        //        BlueEvent.Invoke();
        //        Color32 targetColor = blue;
        //        float originalRadius = li.pointLightOuterRadius;
        //        changeColor(li, targetColor, originalRadius);
        //        currentColor = ColorType.Blue;

        //        PlayAnimation();
        //    }
        //    else if (currentColor == ColorType.Blue)
        //    {
        //        Debug.Log("变成绿色");
        //        GreenEvent.Invoke();
        //        Color32 targetColor = green;
        //        float originalRadius = li.pointLightOuterRadius;
        //        changeColor(li, targetColor, originalRadius);
        //        currentColor = ColorType.Green;
        //        interBut.Interactable(true);
        //        Debug.Log(currentColor);
        //        StealthSkill.Instance.ResetSteal();

        //        PlayAnimation();
        //    }

        //    else if (currentColor == ColorType.Green)
        //    {
        //        interBut.Interactable(false);
        //        Debug.Log("变成白色");
        //        WhiteEvent.Invoke();
        //        Color32 targetColor = new Color32(255, 255, 255, 255);
        //        float originalRadius = li.pointLightOuterRadius;
        //        changeColor(li, targetColor, originalRadius);
        //        currentColor = ColorType.White;
        //        ColorAbility.Instance.ResetSlider();

        //        PlayAnimation();
        //    }
        //    UpdateColor();
        //}
        public void ChangeColor()
        {
            ChangeAudio.Play();

            float originalRadius = li.pointLightOuterRadius;

            switch (currentColor)
            {
                case ColorType.White:
                    Debug.Log("变成红色");
                    RedEvent.Invoke();
                    changeColor(li, red, originalRadius);
                    SetColor(ColorType.Red);
                    interBut.Interactable(true);
                    break;

                case ColorType.Red:
                    Debug.Log("变成蓝色");
                    BlueEvent.Invoke();
                    changeColor(li, blue, originalRadius);
                    SetColor(ColorType.Blue);
                    break;

                case ColorType.Blue:
                    Debug.Log("变成绿色");
                    GreenEvent.Invoke();
                    changeColor(li, green, originalRadius);
                    SetColor(ColorType.Green);
                    interBut.Interactable(true);
                    StealthSkill.Instance.ResetSteal();
                    break;

                case ColorType.Green:
                    Debug.Log("变成白色");
                    WhiteEvent.Invoke();
                    changeColor(li, new Color32(255, 255, 255, 255), originalRadius);
                    SetColor(ColorType.White);
                    interBut.Interactable(false);
                    ColorAbility.Instance.ResetSlider();
                    break;
            }

            PlayAnimation();
        }
        private void SetColor(ColorType newColor)
        {
            if (currentColor == newColor) return;

            currentColor = newColor;

            Debug.Log("【广播颜色变化】" + currentColor);
            // ✅ 这是你整个系统的“广播中心”
            OnLightColorChanged?.Invoke(currentColor);

            UpdateColor();
        }

        public void changeColor(Light2D li, Color32 targetColor, float originalRadius)//TweenCallback onComplete)
        {
            // 停止所有正在运行的动画，防止冲突
            li.DOKill();

            // 转换Color32到Color（因为Light2D的color属性是Color类型）
            Color target = targetColor;
            Color startColor = li.color;

            // 保存原始半径并开始动画序列
            DG.Tweening.Sequence sequence = DOTween.Sequence();

            // 2. 依次过渡RGB分量
            float componentDuration = 0.2f; // 每个分量的过渡时间

            // 红色分量过渡
            sequence.Append(DOTween.To(
                () => li.color.r,
                value =>
                {
                    Color newColor = li.color;
                    newColor.r = value;
                    li.color = newColor;
                },
                target.r,
                componentDuration
            ).SetEase(Ease.InOutSine));

            // 绿色分量过渡
            sequence.Append(DOTween.To(
                () => li.color.g,
                value =>
                {
                    Color newColor = li.color;
                    newColor.g = value;
                    li.color = newColor;
                },
                target.g,
                componentDuration
            ).SetEase(Ease.InOutSine));

            // 蓝色分量过渡
            sequence.Append(DOTween.To(
                () => li.color.b,
                value =>
                {
                    Color newColor = li.color;
                    newColor.b = value;
                    li.color = newColor;
                },
                target.b,
                componentDuration
            ).SetEase(Ease.InOutSine));

            // 3. 恢复光源半径
            //sequence.Append(DOTween.To(
            //    () => li.pointLightOuterRadius,
            //    x => li.pointLightOuterRadius = x,
            //    originalRadius, 0.2f
            //).SetEase(Ease.InOutSine));

            // 4. 动画完成回调
            //sequence.OnComplete(onComplete);
        }
        public void UpdateColor()
        {
            this.GetComponent<ColorAbility>().currentColor = currentColor;
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_ChangeColorAnimationParameterName, AnimatorControllerParameterType.Bool, out _ChangeColorAnimationParameter);
            //RegisterAnimatorParameter(_ChangeColor_WTRAnimationParameterName, AnimatorControllerParameterType.Bool, out _ChangeColor_WTRAnimationParameter);
            //RegisterAnimatorParameter(_ChangeColor_RTBAnimationParameterName, AnimatorControllerParameterType.Bool, out _ChangeColor_RTBAnimationParameter);
            //RegisterAnimatorParameter(_ChangeColor_BTGAnimationParameterName, AnimatorControllerParameterType.Bool, out _ChangeColor_BTGAnimationParameter);
            //RegisterAnimatorParameter(_ChangeColor_GTWAnimationParameterName, AnimatorControllerParameterType.Bool, out _ChangeColor_GTWAnimationParameter);
        }

        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(
                _animator,
                _ChangeColorAnimationParameter,
                (_movement.CurrentState == CharacterStates.MovementStates.ChangeColor),
                _character._animatorParameters
                );

            //MMAnimatorExtensions.UpdateAnimatorBool(
            //    _animator,
            //    _ChangeColor_WTRAnimationParameter,
            //    (_movement.CurrentState == CharacterStates.MovementStates.ChangeColor_WTR),
            //    _character._animatorParameters
            //    );
            //MMAnimatorExtensions.UpdateAnimatorBool(
            //    _animator,
            //    _ChangeColor_RTBAnimationParameter,
            //    (_movement.CurrentState == CharacterStates.MovementStates.ChangeColor_RTB),
            //    _character._animatorParameters
            //    );
            //MMAnimatorExtensions.UpdateAnimatorBool(
            //    _animator,
            //    _ChangeColor_BTGAnimationParameter,
            //    (_movement.CurrentState == CharacterStates.MovementStates.ChangeColor_BTG),
            //    _character._animatorParameters
            //    );
            //MMAnimatorExtensions.UpdateAnimatorBool(
            //    _animator,
            //    _ChangeColor_GTWAnimationParameter,
            //    (_movement.CurrentState == CharacterStates.MovementStates.ChangeColor_GTW),
            //    _character._animatorParameters
            //    );
        }
        /// <summary>
        /// 检测指定动画是否播放完毕，如果播放完毕则切回 Idle
        /// </summary>
        /// <param name="animationName">动画状态名</param>
        /// <returns>动画播放完毕返回 true，否则返回 false</returns>

        private float GetAnimationClipLength(string clipName)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null) return 1.5f;
            var clips = _animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name.Equals(clipName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return clips[i].length;
                }
            }
            // fallback：尝试包含关系匹配
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name.IndexOf(clipName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return clips[i].length;
                }
            }
            return 1.5f; // 默认长度
        }
        public void PlayAnimation()
        {
            _movement.ChangeState(CharacterStates.MovementStates.ChangeColor);

            _animationTimer = 0f;
            _isPlayingChangeColor = true;

            _ChangeColorAnimationLength = GetAnimationClipLength("ChangeColor");
           // _ChangeColorAnimationLength += 0.05f;

            animator_Extension.InitializeLayers();
            switch (currentColor)
            {
                case ColorType.Red:
                    animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.RedForm);
                    break;
                case ColorType.Blue:
                    animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.BlueForm);
                    break;
                case ColorType.Green:
                    animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.GreenForm);
                    break;
                case ColorType.White:
                    animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.WhiteForm);
                    break;
            }
        }
        public void PlayWTRAnimation()
        {
            // 切换状态（Corgi）
            _movement.ChangeState(CharacterStates.MovementStates.ChangeColor_WTR);

            // 重置计时器，开启播放标识
            _animationTimer = 0f;
            _isPlayingChangeColor = true;

            // 获取动画片段长度（从 RuntimeAnimatorController）
            _ChangeColorAnimationLength = GetAnimationClipLength("ChangeColor_WTR"); // 传入 clip 名（不区分大小写）
            // 给一点缓冲（避免过短的过渡问题）
            _ChangeColorAnimationLength += 0.05f;

            // （可选）强制硬切，减少过渡延迟（取决于你的 Animator 设置）
            // _animator.CrossFadeInFixedTime("beHelped", 0.05f, 0);
            animator_Extension.InitializeLayers();
            animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.RedForm);
        }
        public void PlayRTBAnimation()
        {
            // 切换状态（Corgi）
            _movement.ChangeState(CharacterStates.MovementStates.ChangeColor_RTB);

            // 重置计时器，开启播放标识
            _animationTimer = 0f;
            _isPlayingChangeColor = true;

            // 获取动画片段长度（从 RuntimeAnimatorController）
            _ChangeColorAnimationLength = GetAnimationClipLength("ChangeColor_RTB"); // 传入 clip 名（不区分大小写）
            // 给一点缓冲（避免过短的过渡问题）
            _ChangeColorAnimationLength += 0.05f;

            // （可选）强制硬切，减少过渡延迟（取决于你的 Animator 设置）
            // _animator.CrossFadeInFixedTime("beHelped", 0.05f, 0);
            animator_Extension.InitializeLayers();
            animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.BlueForm);
        }
        public void PlayBTGAnimation()
        {
            // 切换状态（Corgi）
            _movement.ChangeState(CharacterStates.MovementStates.ChangeColor_BTG);

            // 重置计时器，开启播放标识
            _animationTimer = 0f;
            _isPlayingChangeColor = true;

            // 获取动画片段长度（从 RuntimeAnimatorController）
            _ChangeColorAnimationLength = GetAnimationClipLength("ChangeColor_BTG"); // 传入 clip 名（不区分大小写）
            // 给一点缓冲（避免过短的过渡问题）
            _ChangeColorAnimationLength += 0.05f;

            // （可选）强制硬切，减少过渡延迟（取决于你的 Animator 设置）
            // _animator.CrossFadeInFixedTime("beHelped", 0.05f, 0);
            animator_Extension.InitializeLayers();
            animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.GreenForm);
        }
        public void PlayGTWAnimation()
        {
            // 切换状态（Corgi）
            _movement.ChangeState(CharacterStates.MovementStates.ChangeColor_GTW);

            // 重置计时器，开启播放标识
            _animationTimer = 0f;
            _isPlayingChangeColor = true;

            // 获取动画片段长度（从 RuntimeAnimatorController）
            _ChangeColorAnimationLength = GetAnimationClipLength("ChangeColor_GTW"); // 传入 clip 名（不区分大小写）
            // 给一点缓冲（避免过短的过渡问题）
            _ChangeColorAnimationLength += 0.05f;

            // （可选）强制硬切，减少过渡延迟（取决于你的 Animator 设置）
            // _animator.CrossFadeInFixedTime("beHelped", 0.05f, 0);
            animator_Extension.InitializeLayers();
            animator_Extension.ChangeLayer(Animator_extension.AnimatorLayer.WhiteForm);
        }
    }
}
