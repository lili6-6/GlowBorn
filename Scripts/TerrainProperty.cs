
//using UnityEngine;
//using MoreMountains.CorgiEngine;
//using DG.Tweening;

//namespace shootstar
//{
//    public class TerrainProperty : MonoBehaviour
//    {
//        [Header("移动/跳跃倍率")]
//        public float moveMultiplier = 1f;
//        public float jumpMultiplier = 1f;

//        [Header("音效 (建议设为Loop)")]
//        [SerializeField] private AudioSource[] audios;

//        private AudioSource walkaudio;
//        private int audioIndex = 0;

//        [Header("陷入")]
//        [SerializeField] private bool isSink = false;
//        [SerializeField] private float duration = 0.3f;
//        [SerializeField] private Ease type = Ease.OutSine;
//        [Tooltip("向下移的距离")]
//        [SerializeField] private float targetY = 0.1f;

//        private float originSpeedFactor;
//        private float originJumpHeight;

//        private bool isIn;
//        private Transform Player;

//        private CharacterJump characterJump;
//        private CorgiController corgiController;
//        private StealthSkill stealthskill;
//        private float stealthSpeedMultiplier;

//        private Collider2D PlatformCollider;
//        private Vector2 originColliderOffset;
//        private Tween sinkTween;

//        private void Start()
//        {
//            PlatformCollider = GetComponent<Collider2D>();
//            originColliderOffset = PlatformCollider.offset;
//            Player = shootingstarGameManager.Instance.Player.transform;
//            originSpeedFactor = Player.gameObject.GetComponent<CorgiController>().DefaultParameters.SpeedFactor;

//        }

//        private void OnTriggerEnter2D(Collider2D other)
//        {
//            if (!other.CompareTag("Player")) return;

//            Player = other.transform;
//            CachePlayerComponents();
//            ApplyPlayerMultiplier();

//            StartSink();

//            if (walkaudio != null && !walkaudio.isPlaying)
//            {
//                walkaudio.Play();

//            }
//           // walkaudio = null;

//            isIn = true;
//        }

//        private void OnTriggerExit2D(Collider2D other)
//        {
//            if (!other.CompareTag("Player")) return;

//            isIn = false;

//            ResetPlayerMultiplier();
//            StopAudio();
//            RestorePlatform();
//        }

//        void Update()
//        {
//            if (!isIn || Player == null || corgiController == null || audios == null || audios.Length == 0)
//                return;

//            float horizontalSpeed = Mathf.Abs(corgiController.Speed.x);
//            bool isMoving = horizontalSpeed > 0.15f;

//            if (isMoving)
//            {
//                if (walkaudio == null || !walkaudio.isPlaying)
//                {
//                    randomPlay();
//                }

//                if (walkaudio != null && walkaudio.isPlaying)
//                {
//                    // ✅ 关键：根据 DefaultParameters.SpeedFactor 动态调整音效速度
//                    // 让音效跟随技能/地形加成变化，而不是固定数值
//                    float currentFactor = corgiController.DefaultParameters.SpeedFactor;
//                    float relativeSpeed = currentFactor / originSpeedFactor;  // 计算相对倍率
//                    float targetPitch = Mathf.Clamp(relativeSpeed * moveMultiplier, 0.6f, 2f);

//                    walkaudio.pitch = targetPitch;
//                }
//            }
//            else
//            {
//                if (walkaudio != null && walkaudio.isPlaying)
//                {
//                    walkaudio.Stop();
//                    walkaudio = null;
//                }
//            }
//        }


//        #region --- Sink Methods ---
//        private void StartSink()
//        {
//            if (!isSink) return;

//            sinkTween?.Kill();
//            sinkTween = DOTween.To(
//                () => PlatformCollider.offset.y,
//                y => PlatformCollider.offset = new Vector2(PlatformCollider.offset.x, y),
//                originColliderOffset.y - targetY,
//                duration
//            ).SetEase(type);
//        }

//        private void RestorePlatform()
//        {
//            if (!isSink) return;

//            sinkTween?.Kill();
//            sinkTween = DOTween.To(
//                () => PlatformCollider.offset.y,
//                y => PlatformCollider.offset = new Vector2(PlatformCollider.offset.x, y),
//                originColliderOffset.y,
//                duration
//            ).SetEase(Ease.OutQuad);
//        }
//        #endregion

//        #region --- Player Multiplier Methods ---
//        private void CachePlayerComponents()
//        {
//            if (Player == null) return;
//            corgiController = Player.GetComponent<CorgiController>();

//            characterJump = Player.GetComponent<CharacterJump>();
//            stealthskill = Player.GetComponent<StealthSkill>();
//        }

//        private void ApplyPlayerMultiplier()
//        {
//            if (corgiController != null)
//            {
//                if (stealthskill != null && stealthskill.isStealthActive)
//                {
//                    stealthSpeedMultiplier = stealthskill != null ? stealthskill.stealthSpeedMultiplier : 1f;

//                    corgiController.DefaultParameters.SpeedFactor = originSpeedFactor * moveMultiplier * stealthSpeedMultiplier;
//                    Debug.Log("Apply:" + originSpeedFactor + " *" + stealthSpeedMultiplier + "*" + moveMultiplier);
//                }
//                else
//                {

//                    corgiController.DefaultParameters.SpeedFactor *= moveMultiplier;
//                }

//            }

//            if (characterJump != null)
//            {
//                originJumpHeight = characterJump.JumpHeight;
//                characterJump.JumpHeight *= jumpMultiplier;
//            }
//        }

//        private void ResetPlayerMultiplier()
//        {
//            if (stealthskill.isStealthActive)
//            {
//                if (corgiController != null)
//                    stealthSpeedMultiplier = stealthskill.stealthSpeedMultiplier;
//                corgiController.DefaultParameters.SpeedFactor = originSpeedFactor * stealthSpeedMultiplier;
//                Debug.Log("Reset:" + originSpeedFactor + " *" + stealthSpeedMultiplier + "*" + moveMultiplier);
//            }
//            else
//            {
//                if (corgiController != null)
//                    corgiController.DefaultParameters.SpeedFactor = originSpeedFactor;
//            }


//            if (characterJump != null)
//                characterJump.JumpHeight = originJumpHeight;
//        }
//        #endregion

//        #region --- Audio ---
//        private void StopAudio()
//        {
//            //if (walkaudio != null && walkaudio.isPlaying)
//            //    randomPlay();
//            if (walkaudio != null && walkaudio.isPlaying)
//            {
//                walkaudio.Stop();
//                walkaudio = null;
//            }
//        }
//        #endregion

//        private void randomPlay()
//        {
//            if (audios == null || audios.Length == 0) return;

//            audioIndex = Random.Range(0, audios.Length);
//            walkaudio = audios[audioIndex];

//            if (walkaudio != null)
//            {
//                walkaudio.pitch = 1f;  // 初始 pitch 不要乘 moveMultiplier，这会叠加出问题
//                walkaudio.loop = true; // 建议脚步声设成循环
//                walkaudio.Play();
//                Debug.Log("播放音效：" + walkaudio.name);
//            }
//        }



//    }
//}
using UnityEngine;
using MoreMountains.CorgiEngine;
using DG.Tweening;
using System.Collections;

namespace shootstar
{
    public class TerrainProperty : MonoBehaviour
    {
        [Header("移动/跳跃倍率")]
        public float moveMultiplier = 1f;
        public float jumpMultiplier = 1f;

        [Header("音效 (建议设为Loop)")]
        [SerializeField] private AudioSource[] audios;

        private AudioSource walkaudio;
        private int audioIndex = 0;

        [Header("陷入")]
        [SerializeField] private bool isSink = false;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private Ease type = Ease.OutSine;
        [Tooltip("向下移的距离")]
        [SerializeField] private float targetY = 0.1f;

        private float originSpeedFactor;
        private float originJumpHeight;

        private bool isIn;
        private Transform Player;

        private CharacterJump characterJump;
        private CorgiController corgiController;
        private StealthSkill stealthskill;
        private float stealthSpeedMultiplier;

        private Collider2D PlatformCollider;
        private Vector2 originColliderOffset;
        private Tween sinkTween;
        private Tween fadeTween;

        private void Start()
        {
            PlatformCollider = GetComponent<Collider2D>();
            originColliderOffset = PlatformCollider.offset;

            //Player = shootingstarGameManager.Instance.Player.transform;
            //if (Player == null) Debug.Log("meiyou");
            StartCoroutine(DelayedStart());
            
        }
        // 在TerrainProperty.cs的Start方法中
        private IEnumerator DelayedStart()
        {
            yield return null; // 等待1帧

            // 现在再获取Player
            if (shootingstarGameManager.Instance?.Player != null)
            {
                Player = shootingstarGameManager.Instance.Player.transform;
                originSpeedFactor = Player.gameObject.GetComponent<CorgiController>().DefaultParameters.SpeedFactor;
                //Debug.Log(originSpeedFactor);
            }
            else
            {
                Debug.Log("meiyou: Manager的Player未赋值");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            Player = other.transform;
            CachePlayerComponents();
            ApplyPlayerMultiplier();

            StartSink();

            isIn = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            isIn = false;
            Debug.Log("退出地形");
            ResetPlayerMultiplier();
            StopAudioSmooth(); // ✅ 改为平滑停止
            RestorePlatform();
        }

        void Update()
        {
            if (!isIn || Player == null || corgiController == null || audios == null || audios.Length == 0)
                return;

            float horizontalSpeed = Mathf.Abs(corgiController.Speed.x);
            bool isMoving = horizontalSpeed > 0.15f;

            if (isMoving)
            {
                if (walkaudio == null || !walkaudio.isPlaying)
                {
                    randomPlay();
                }

                if (walkaudio != null && walkaudio.isPlaying)
                {
                    // ✅ 根据当前 DefaultParameters.SpeedFactor 实时调整 pitch
                    float currentFactor = corgiController.DefaultParameters.SpeedFactor;
                    float relativeSpeed = currentFactor / originSpeedFactor;
                    float targetPitch = Mathf.Clamp(relativeSpeed * moveMultiplier, 0.6f, 2f);

                    walkaudio.pitch = targetPitch;
                }
            }
            else
            {
                // ✅ 改为平滑淡出，避免杂音
                if (walkaudio != null && walkaudio.isPlaying)
                {
                    StopAudioSmooth();
                }
            }
        }

        #region --- Sink Methods ---
        private void StartSink()
        {
            if (!isSink) return;

            sinkTween?.Kill();
            sinkTween = DOTween.To(
                () => PlatformCollider.offset.y,
                y => PlatformCollider.offset = new Vector2(PlatformCollider.offset.x, y),
                originColliderOffset.y - targetY,
                duration
            ).SetEase(type);
        }

        private void RestorePlatform()
        {
            if (!isSink) return;

            sinkTween?.Kill();
            sinkTween = DOTween.To(
                () => PlatformCollider.offset.y,
                y => PlatformCollider.offset = new Vector2(PlatformCollider.offset.x, y),
                originColliderOffset.y,
                duration
            ).SetEase(Ease.OutQuad);
        }
        #endregion

        #region --- Player Multiplier Methods ---
        private void CachePlayerComponents()
        {
            if (Player == null) return;
            corgiController = Player.GetComponent<CorgiController>();
            characterJump = Player.GetComponent<CharacterJump>();
            stealthskill = Player.GetComponent<StealthSkill>();
        }

        private void ApplyPlayerMultiplier()
        {
            if (corgiController != null)
            {
                if (stealthskill != null && stealthskill.isStealthActive)
                {
                    stealthSpeedMultiplier = stealthskill != null ? stealthskill.stealthSpeedMultiplier : 1f;
                    corgiController.DefaultParameters.SpeedFactor = originSpeedFactor * moveMultiplier * stealthSpeedMultiplier;
                    Debug.Log("Apply:" + originSpeedFactor + " *" + stealthSpeedMultiplier + "*" + moveMultiplier);
                }
                else
                {
                    corgiController.DefaultParameters.SpeedFactor *= moveMultiplier;
                }
            }

            if (characterJump != null)
            {
                originJumpHeight = characterJump.JumpHeight<1?3:characterJump.JumpHeight;//------------------写死暂时
                characterJump.JumpHeight *= jumpMultiplier;
            }
        }

        private void ResetPlayerMultiplier()
        {
            if (stealthskill.isStealthActive)
            {
                if (corgiController != null)
                    stealthSpeedMultiplier = stealthskill.stealthSpeedMultiplier;
                corgiController.DefaultParameters.SpeedFactor = originSpeedFactor * stealthSpeedMultiplier;
                Debug.Log("Reset:" + originSpeedFactor + " *" + stealthSpeedMultiplier + "*" + moveMultiplier);
            }
            else
            {
                if (corgiController != null)
                    corgiController.DefaultParameters.SpeedFactor = originSpeedFactor;
            }

            if (characterJump != null)
                characterJump.JumpHeight = originJumpHeight;
        }
        #endregion

        #region --- Audio ---
        private void StopAudioSmooth()
        {
            if (walkaudio == null || !walkaudio.isPlaying)
                return;

            fadeTween?.Kill();
            fadeTween = DOTween.To(
                () => walkaudio.volume,
                v => walkaudio.volume = v,
                0f,
                0.25f // 淡出时间（可调）
            ).OnComplete(() =>
            {
                walkaudio.Stop();
                walkaudio.volume = 1f; // 重置音量
                walkaudio = null;
            });
        }

        private void randomPlay()
        {
            audioIndex = Random.Range(0, audios.Length);
            walkaudio = audios[audioIndex];
            walkaudio.pitch = moveMultiplier;
            walkaudio.volume = 1f;
            walkaudio.Play();
            Debug.Log("播放音效：" + walkaudio.name);
        }
        #endregion
    }
}
