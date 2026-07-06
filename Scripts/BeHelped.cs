//using MoreMountains;
//using MoreMountains.CorgiEngine;
//using MoreMountains.Tools;
//using shootstar;
//using UnityEngine;
//using UnityEngine.Assertions.Must;
//using UnityEngine.Events;
//using UnityEngine.UI;

//namespace shootstar
//{


//    public class BeHelped : CharacterAbility
//    {
//        protected const string _beHelpedAnimationParameterName = "BeHelped";
//        protected int _beHelpedAnimationParameter;

//        protected const string _fullStateAnimationParameterName = "FullState";
//        protected int _fullStateAnimationParameter;



//        private float _beHelpedAnimationLength = 1.5f; // 你的beHelped动画长度（秒）
//        private float _animationTimer = 0f;    // 记录动画播放时间
//        public UnityEvent helpFriends;
//        // Start is called once before the first execution of Update after the MonoBehaviour is created
//        private bool _isPlayingBeHelped = false;
//        private ChangeColorManager ChangeColorManager;
//        private GameObject player;
//        private colliderManager collManager;
//        private bool isHelp = false;

//        private ColorType currentColor;


//        protected override void Initialization()
//        {
//            base.Initialization();
//            _controller = _character.GetComponent<CorgiController>();
//        }

//        protected override void Start()
//        {
//            base.Start();


//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (_isPlayingBeHelped)
//            {
//                _animationTimer += Time.deltaTime;

//                // 调试用：打印状态和时间（出问题时打开）
//                // Debug.Log($"Animator state: {_animator.GetCurrentAnimatorStateInfo(0).IsName("beHelped")}, timer: {_animationTimer} / {_beHelpedAnimationLength}");

//                if (_animationTimer >= _beHelpedAnimationLength)
//                {
//                    _isPlayingBeHelped = false;
//                    if (GetComponent<Health>().CurrentHealth >= GetComponent<Health>().MaximumHealth)
//                    {
//                        _movement.ChangeState(CharacterStates.MovementStates.FullState);
//                        //_animator.SetBool(_fullStateAnimationParameter, true);
//                        Debug.Log("小伙伴已满血，进入FullState状态");
//                    }
//                    else
//                    {
//                        _movement.ChangeState(CharacterStates.MovementStates.Idle);
//                    }
//                    _animationTimer = 0f;
//                }
//            }
//            player = shootingstarGameManager.Instance.Player;
//            collManager = player.GetComponent<colliderManager>();
//            if(GetComponent<Health>().CurrentHealth< GetComponent<Health>().MaximumHealth)
//            {
//                _movement.ChangeState(CharacterStates.MovementStates.Idle);
//            }


//        }

//        private void OnTriggerEnter2D(Collider2D collision)
//        {

//            if (collision.CompareTag("Player"))
//            {
//                collision.GetComponent<ColorAbility>().BeHelped =GetComponent<BeHelped>();
//                currentColor = collision.GetComponent<ChangeColorManager>().currentColor;
//                if (currentColor == ColorType.Green)
//                {
//                    shootingstarGameManager.Instance.eventManager.beHelped = this.GetComponent<BeHelped>();
//                    Debug.Log("帮助小伙伴");
//                    player.GetComponent<colliderManager>().Inter = this.gameObject.tag;
//                    Debug.Log(collManager.Inter+"BeHelped");
//                }
//                else
//                {
//                   Debug.Log("颜色不对，无法帮助小伙伴");
//                }
//            }
//        }

//        private void OnTriggerExit2D(Collider2D collision)
//        {
//            if (collision.CompareTag("Player"))
//            {
//                collManager.Inter = null;
//            }
//        }
//        public void meetHelpFriend()
//        {
//            if (shootingstarGameManager.Instance.uiManager. cLightNum < 2||isHelp)
//            {
//                Debug.Log("光源不足，无法帮助小伙伴");
//                return;
//            }

//            // 检查小伙伴生命值是否已满（使用Health组件）
//            Health friendHealth = this.GetComponent<Health>();
//            if (friendHealth != null && friendHealth.CurrentHealth >= friendHealth.MaximumHealth)
//            {
//                Debug.Log("小伙伴生命值已满，无需帮助");
//                return;
//            }
//            int temp = shootingstarGameManager.Instance.uiManager.cLightNum -= 2;
//            collManager.OnCollectItem(-1f);
//            shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
//            helpFriends.Invoke();
//            Debug.Log(helpFriends.ToString());
//            shootingstarGameManager.Instance.uiManager. UpdateFriendsText(shootingstarGameManager.Instance.uiManager.cFriendsNum += 1);
//            eventManager.Instance.BeHelped.Invoke();
//            Debug.Log("帮助小伙伴成功，当前帮助的小伙伴数量：" + shootingstarGameManager.Instance.uiManager. cFriendsNum);
//            //播放音效
//            //AudioManager.Instance.greenAudio.Play();
//            // 治疗小伙伴
//            if (friendHealth != null)
//            {
//                friendHealth.GetHealth(50, gameObject);
//                Debug.Log("小伙伴恢复50点生命值");
//            }
//            isHelp = true;
//            if (friendHealth.CurrentHealth >= friendHealth.MaximumHealth)
//            {
//                Debug.Log("小伙伴生命值已满");

//            }
//            PlayBeHelpedAnimation(); // 切换动画并开始检测


//        }

//        protected override void InitializeAnimatorParameters()
//        {
//            RegisterAnimatorParameter(_beHelpedAnimationParameterName, AnimatorControllerParameterType.Bool, out _beHelpedAnimationParameter);
//           RegisterAnimatorParameter(_fullStateAnimationParameterName, AnimatorControllerParameterType.Bool, out _fullStateAnimationParameter);
//        }

//        public override void UpdateAnimator()
//        {
//            MMAnimatorExtensions.UpdateAnimatorBool(
//                _animator, 
//                _beHelpedAnimationParameter, 
//                (_movement.CurrentState==CharacterStates.MovementStates.BeHelped),
//                _character._animatorParameters
//                );

//            MMAnimatorExtensions.UpdateAnimatorBool(
//                _animator,
//                _fullStateAnimationParameter,
//                (_movement.CurrentState == CharacterStates.MovementStates.FullState),
//                _character._animatorParameters
//                );
//        }



//        /// <summary>
//        /// 检测指定动画是否播放完毕，如果播放完毕则切回 Idle
//        /// </summary>
//        /// <param name="animationName">动画状态名</param>
//        /// <returns>动画播放完毕返回 true，否则返回 false</returns>

//        public void PlayBeHelpedAnimation()
//        {
//            // 切换状态（Corgi）
//            _movement.ChangeState(CharacterStates.MovementStates.BeHelped);
//            Debug.Log("播放帮助小伙伴动画");

//            // 重置计时器，开启播放标识
//            _animationTimer = 0f;
//            _isPlayingBeHelped = true;

//            // 获取动画片段长度（从 RuntimeAnimatorController）
//            _beHelpedAnimationLength = GetAnimationClipLength("beHelped"); // 传入 clip 名（不区分大小写）
//            Debug.Log(_beHelpedAnimationLength);
//        }
//        private float GetAnimationClipLength(string clipName)
//        {
//            if (_animator == null || _animator.runtimeAnimatorController == null) return 1.5f;
//            var clips = _animator.runtimeAnimatorController.animationClips;
//            for (int i = 0; i < clips.Length; i++)
//            {
//                if (clips[i].name.Equals(clipName, System.StringComparison.OrdinalIgnoreCase))
//                {
//                    return clips[i].length;
//                }
//            }
//            // fallback：尝试包含关系匹配
//            for (int i = 0; i < clips.Length; i++)
//            {
//                if (clips[i].name.IndexOf(clipName, System.StringComparison.OrdinalIgnoreCase) >= 0)
//                {
//                    return clips[i].length;
//                }
//            }
//            return 1.5f; // 默认长度
//        }
//        public void OnGreenHealStart()
//        {
//            if (_movement.CurrentState == CharacterStates.MovementStates.BeHelped)
//                return;

//            _movement.ChangeState(CharacterStates.MovementStates.BeHelped);
//            Debug.Log("Help：进入被治疗状态（BeHelped）");
//        }

//        public void OnGreenHealStop(bool isFullyHealed)
//        {
//            if (isFullyHealed)
//            {
//                _movement.ChangeState(CharacterStates.MovementStates.FullState);
//                Debug.Log("Help：治疗完成，进入 FullState");
//            }
//            else
//            {
//                _movement.ChangeState(CharacterStates.MovementStates.Idle);
//                Debug.Log("Help：治疗中断，回 Idle");
//            }
//        }

//    }
//}
using MoreMountains;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace shootstar
{
    public interface ILightReactive
    {
        void ApplyLight(ColorType color);
    }
}

namespace shootstar
{
    public class BeHelped : CharacterAbility, ILightReactive
    {
        protected const string _beHelpedAnimationParameterName = "BeHelped";
        protected const string _fullStateAnimationParameterName = "FullState";

        protected int _beHelpedAnimationParameter;
        protected int _fullStateAnimationParameter;

        private float _animationTimer;
        private float _beHelpedAnimationLength = 1.5f;

        private bool _isPlayingBeHelped;
        private bool isHelp;

        private GameObject player;
        private colliderManager collManager;

        private ColorType currentColor;

        public UnityEvent helpFriends;

        protected override void Initialization()
        {
            base.Initialization();
            _controller = _character.GetComponent<CorgiController>();
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_beHelpedAnimationParameterName, AnimatorControllerParameterType.Bool, out _beHelpedAnimationParameter);
            RegisterAnimatorParameter(_fullStateAnimationParameterName, AnimatorControllerParameterType.Bool, out _fullStateAnimationParameter);
        }

        void Update()
        {
            if (_isPlayingBeHelped)
            {
                _animationTimer += Time.deltaTime;
                if (_animationTimer >= _beHelpedAnimationLength)
                {
                    _isPlayingBeHelped = false;
                    _movement.ChangeState(
                        GetComponent<Health>().CurrentHealth >= GetComponent<Health>().MaximumHealth
                        ? CharacterStates.MovementStates.FullState
                        : CharacterStates.MovementStates.Idle
                    );
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            player = collision.gameObject;
            collManager = player.GetComponent<colliderManager>();

            collision.GetComponent<ColorAbility>().BeHelped = this;

            Debug.Log("进入帮助范围，等待光色");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            if (collManager != null)
                collManager.Inter = null;
        }

        /// <summary>
        /// ✅ 被 LightTriggerListener2D 调用
        /// </summary>
        public void ApplyLight(ColorType color)
        {
            currentColor = color;

            if (color == ColorType.Green)
            {
                Debug.Log("BeHelped：绿色光 → 允许帮助");

                shootingstarGameManager.Instance.eventManager.beHelped = this;
                if (collManager != null)
                    collManager.Inter = gameObject.tag;
            }
            else
            {
                Debug.Log("BeHelped：非绿色光 → 禁止帮助");

                if (collManager != null)
                    collManager.Inter = null;
            }
        }

        public void meetHelpFriend()
        {
            if (isHelp || shootingstarGameManager.Instance.uiManager.cLightNum < 2)
                return;

            Health hp = GetComponent<Health>();
            if (hp.CurrentHealth >= hp.MaximumHealth)
                return;

            shootingstarGameManager.Instance.uiManager.cLightNum -= 2;
            shootingstarGameManager.Instance.uiManager.UpdateLightText(
                shootingstarGameManager.Instance.uiManager.cLightNum
            );

            hp.GetHealth(50, gameObject);
            helpFriends?.Invoke();

            isHelp = true;
            PlayBeHelpedAnimation();
        }

        private void PlayBeHelpedAnimation()
        {
            _movement.ChangeState(CharacterStates.MovementStates.BeHelped);
            _animationTimer = 0f;
            _isPlayingBeHelped = true;
        }

        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(
                _animator,
                _beHelpedAnimationParameter,
                _movement.CurrentState == CharacterStates.MovementStates.BeHelped,
                _character._animatorParameters
            );

            MMAnimatorExtensions.UpdateAnimatorBool(
                _animator,
                _fullStateAnimationParameter,
                _movement.CurrentState == CharacterStates.MovementStates.FullState,
                _character._animatorParameters
            );
        }
        public void OnGreenHealStart()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.BeHelped)
                return;

            _movement.ChangeState(CharacterStates.MovementStates.BeHelped);
            Debug.Log("Help：进入被治疗状态（BeHelped）");
        }

        public void OnGreenHealStop(bool isFullyHealed)
        {
            if (isFullyHealed)
            {
                _movement.ChangeState(CharacterStates.MovementStates.FullState);
                Debug.Log("Help：治疗完成，进入 FullState");
            }
            else
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                Debug.Log("Help：治疗中断，回 Idle");
            }
        }
    }
}
