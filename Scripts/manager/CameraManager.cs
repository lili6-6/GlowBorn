

using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using Halabang.Plugin; // 引用你定义 ScriptableObject 的命名空间

namespace shootstar
{


    public class CameraManager : MonoBehaviour
    {
        [Header("主摄像头")]
        [SerializeField]public Camera mainCamera;
        [SerializeField] public Camera uiCamera;
        [Header("主摄像机上的 CinemachineBrain")]
        public CinemachineBrain brain;

        [Header("全局震动配置 (ScriptableObject)")]
        public CameraShakePreset bulletShakePreset; // 在Inspector中直接拖入ScriptableObject

        // ===== 触发器管理 =====
        private static List<CameraTriggerData> activeTriggers = new List<CameraTriggerData>();

        // 用于记录每个虚拟相机的默认优先级
        private Dictionary<CinemachineCamera, int> defaultPriorities = new Dictionary<CinemachineCamera, int>();

        [HideInInspector]public List<CinemachineCamera> cinemachineCameras;
        //public static CameraManager Instance { get; set; }

        private void Awake()
        {
            if (brain == null)
                brain = Camera.main.GetComponent<CinemachineBrain>();
          
        }
        private Tween ShakeTween;
        // =========================
        // 触发器逻辑接口
        // =========================
        public void TriggerEnter(CinemachineCamera vCam, int activePriority, bool keepPriorityOnExit)
        {
            if (!defaultPriorities.ContainsKey(vCam))
                defaultPriorities[vCam] = vCam.Priority;

            // 添加到活跃触发器列表
            activeTriggers.Add(new CameraTriggerData
            {
                vCam = vCam,
                priority = activePriority,
                keepOnExit = keepPriorityOnExit
            });

            // 设置高优先级
            vCam.Priority = activePriority;
        }

        public void TriggerExit(CinemachineCamera vCam,bool keepPriorityOnExit)
        {
            //CameraTriggerData data = activeTriggers.FindLast(t => t.vCam == vCam);
            //if (data == null) return;

            //if (!data.keepOnExit)
            //    activeTriggers.Remove(data);

            //if (activeTriggers.Count > 0)
            //{
            //    var last = activeTriggers[activeTriggers.Count - 1];
            //    last.vCam.Priority = last.priority;
            //}
            //else
            //{
            //    if (defaultPriorities.ContainsKey(vCam))
            //        vCam.Priority = defaultPriorities[vCam];
            //}
            if(keepPriorityOnExit!=true)
            vCam.Priority = -1;
        }

        // =========================
        // 震动逻辑
        // =========================
        public void ShakeCurrentCamera(CameraShakePreset preset = null)
        {
            if (brain == null || brain.ActiveVirtualCamera == null) return;

            // ActiveVirtualCamera 是 ICinemachineCamera，需要转换
            CinemachineCamera activeCam = brain.ActiveVirtualCamera as CinemachineCamera;
            if (activeCam == null) return;

            // 获取或添加 Perlin 组件
            var noise = activeCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                noise = activeCam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
            }
            Debug.Log(activeCam);

            // 使用全局或自定义传入的 CameraShakePreset
            if (preset == null && bulletShakePreset != null)
                preset = bulletShakePreset;

            if (preset == null) return;

            // 停止当前的震动Tween
            DOTween.Kill(noise);

            // 根据 ScriptableObject 配置设置震动
            if (preset.StrengthVector != Vector3.zero)
            {
                // 如果使用 Vector3 强度（每轴独立设置）
                noise.AmplitudeGain = preset.StrengthVector.magnitude;
            }
            else
            {
                // 使用单一 float 强度
                noise.AmplitudeGain = preset.StrengthFloat;
            }

            noise.FrequencyGain = preset.Vibrato; // 用 Vibrato 作为频率

            // 按照配置实现衰减动画
             ShakeTween= DOTween.To(() => noise.AmplitudeGain,
                       x => noise.AmplitudeGain = x,
                       0f,
                       preset.Duration)
                .SetLoops(preset.TweenLoopCycle, preset.TweenLoopType)
                   .SetEase(preset.EaseType);

            Debug.Log("Camera Shake Triggered via ScriptableObject");
        }

        // =========================
        // 手动优先级控制
        // =========================
        public void SwitchCamera(CinemachineCamera vCam, int priority)
        {
            if (vCam == null) return;
            if (!defaultPriorities.ContainsKey(vCam))
                defaultPriorities[vCam] = vCam.Priority;

            vCam.Priority = priority;
        }

        public void ResetCameraPriority(CinemachineCamera vCam)
        {
            if (vCam == null) return;
            if (defaultPriorities.ContainsKey(vCam))
                vCam.Priority = defaultPriorities[vCam];
        }

        // 内部类，用于记录触发器状态
        private class CameraTriggerData
        {
            public CinemachineCamera vCam;
            public int priority;
            public bool keepOnExit;
        }

        public void FollowPlayer(CinemachineCamera targetCamera)
        {
            if (targetCamera == null) return;
            targetCamera.Follow = shootingstarGameManager.Instance.Player.transform;
        }

        public void StopShake()
        {
            // 1. 安全停止当前震动Tween
            if (ShakeTween != null && ShakeTween.IsActive())
            {
                ShakeTween.Kill();
                ShakeTween = null;
            }

            // 2. 获取当前激活的虚拟相机
            if (brain == null || brain.ActiveVirtualCamera == null) return;
            CinemachineCamera activeCam = brain.ActiveVirtualCamera as CinemachineCamera;
            if (activeCam == null) return;

            // 3. 获取 Perlin 噪声组件
            var noise = activeCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                // 4. 重置震动参数（彻底停止晃动）
                noise.AmplitudeGain = 0f;
                noise.FrequencyGain = 0f;
            }

            Debug.Log("Camera Shake fully stopped.");
        }


        public void CameraSize(float duration, CinemachineCamera targetCamera,Ease easeType,float targetSize)
        {
            if (targetCamera == null) return;
            float originsize=targetCamera.Lens.OrthographicSize;

            DOTween.To(()=>targetCamera.Lens.OrthographicSize,
                x=>targetCamera.Lens.OrthographicSize=x,
                targetSize,
                duration)
                .SetEase(easeType);

        }
        public void Rebirth()
        {
            foreach (var vCam in cinemachineCameras)
            {
                Debug.Log(vCam.gameObject.name);
                vCam.gameObject.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
            }
        }
    }

}