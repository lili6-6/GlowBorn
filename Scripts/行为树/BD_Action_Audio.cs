using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("执行音乐音效相关的各种行为（基于Unity AudioSource实现）")]
    public class BD_Action_Audio : Action
    {
        public enum ACTION_NAME
        {
            NULL,
            PLAY_AUDIO,
            STOP_AUDIO,
            PAUSE_AUDIO,
            TWEEN_VOLUME,
            RESET_VOLUME,
            PLAY_BGM
        }

        [BehaviorDesigner.Runtime.Tasks.Tooltip("要操作的音源")]
        public AudioSource audioSource;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("音频片段，仅在播放音效时使用")]
        public AudioClip audioClip;

        public ACTION_NAME triggerAction;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("目标音量")]
        public float targetValue = 1f;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Tween时间")]
        public float tweenTime = 0.5f;

        private float originalVolume;

        public override void OnStart()
        {
            if (audioSource != null)
                originalVolume = audioSource.volume;

            CallAction();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        private void CallAction()
        {
            if (audioSource == null)
            {
                Debug.LogWarning("BD_Action_Audio: audioSource未设置！");
                return;
            }

            switch (triggerAction)
            {
                case ACTION_NAME.PLAY_AUDIO:
                    if (audioClip != null)
                        audioSource.PlayOneShot(audioClip);
                    break;

                case ACTION_NAME.PLAY_BGM:
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = audioClip;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                    break;

                case ACTION_NAME.STOP_AUDIO:
                    audioSource.Stop();
                    break;

                case ACTION_NAME.PAUSE_AUDIO:
                    audioSource.Pause();
                    break;

                case ACTION_NAME.TWEEN_VOLUME:
                    DOTween.To(() => audioSource.volume, x => audioSource.volume = x, targetValue, tweenTime)
                           .SetEase(Ease.InOutSine);
                    break;

                case ACTION_NAME.RESET_VOLUME:
                    DOTween.To(() => audioSource.volume, x => audioSource.volume = x, originalVolume, tweenTime)
                           .SetEase(Ease.InOutSine);
                    break;
            }
        }
    }
}
