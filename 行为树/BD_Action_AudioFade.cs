using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Halabang")]
[TaskDescription("根据枚举播放或停止音效，支持音量渐变")]
public class BD_Action_AudioFade : Action
{
    public enum AudioAction
    {
        Play,
        Stop
    }

    [BehaviorDesigner.Runtime.Tasks.Tooltip("音效动作：播放或停止")]
    public AudioAction action = AudioAction.Play;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("要控制的音频源")]
    public AudioSource audioSource;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("渐变时间 (秒)")]
    public float fadeTime = 1f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("目标音量")]
    public float targetVolume = 1f;

    private float initialVolume;
    private float timer;

    public override void OnStart()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource 未赋值！");
            return;
        }

        timer = 0f;

        if (action == AudioAction.Play)
        {
            // 播放前先把音量置0
            audioSource.volume = 0f;
            audioSource.Play();
            initialVolume = 0f;
        }
        else if (action == AudioAction.Stop)
        {
            if (!audioSource.isPlaying)
            {
                // 已经停止了，直接完成
                timer = fadeTime;
            }
            else
            {
                initialVolume = audioSource.volume;
            }
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (audioSource == null) return TaskStatus.Failure;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fadeTime);

        if (action == AudioAction.Play)
        {
            audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, t);
            if (t >= 1f) return TaskStatus.Success;
            return TaskStatus.Running;
        }
        else // Stop
        {
            audioSource.volume = Mathf.Lerp(initialVolume, 0f, t);
            if (t >= 1f)
            {
                audioSource.Stop();
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }

    public override void OnReset()
    {
        action = AudioAction.Play;
        audioSource = null;
        fadeTime = 1f;
        targetVolume = 1f;
    }
}
