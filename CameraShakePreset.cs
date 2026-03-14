//using DG.Tweening;
//using UnityEngine;

//namespace Halabang.Plugin {
//  [CreateAssetMenu(fileName = "shake_", menuName = "Geminum/Camera/Shake Preset")]
//  public class CameraShakePreset : ScriptableObject {

//    public float Duration;
//    public float StrengthFloat;
//    [Tooltip("Strengh of shaking in each axis")]
//    public Vector3 StrengthVector;
//    [Tooltip("Frequency of shaking")]
//    public int Vibrato;
//    [Tooltip("When checked, fade out will not be applied")]
//    public bool ManuallyFadeout;
//    [Tooltip("When checked, shaking will also affact camera Z axis")]
//    public bool IncludeZAxis;
//    [Tooltip("randomness of shaking, over 90-180 not used here")]
//    [Range(0, 90)]
//    public float Randomness;
//    [Tooltip("Harmonic is more balanced and visually more pleasant")]
//    public ShakeRandomnessMode randomnessMode;

//    [Header("Tweener setting")]
//    [Tooltip("Delay of seconds to start the tweener")]
//    public float Delay;
//    [Tooltip("Ease type")]
//    public Ease EaseType;
//    [Tooltip("Loop count, -1 is infinity")]
//    public int TweenLoopCycle;
//    [Tooltip("Loop type")]
//    public LoopType TweenLoopType;
//  }
//}
using DG.Tweening;
using UnityEngine;

namespace Halabang.Plugin
{
    // 相机震动参数预设，可在资源菜单中创建
    [CreateAssetMenu(fileName = "shake_", menuName = "Geminum/Camera/Shake Preset")]
    public class CameraShakePreset : ScriptableObject
    {

        public float Duration; // 震动持续时间
        public float StrengthFloat; // 震动强度（单一数值）
        [Tooltip("各轴向的震动强度")]
        public Vector3 StrengthVector; // 各轴向的震动强度
        [Tooltip("震动频率")]
        public int Vibrato; // 震动频率
        [Tooltip("勾选后不自动淡出")]
        public bool ManuallyFadeout; // 勾选后不自动淡出
        [Tooltip("勾选后震动会影响Z轴")]
        public bool IncludeZAxis; // 勾选后震动会影响Z轴
        [Tooltip("震动的随机性")]
        [Range(0, 90)]
        public float Randomness; // 震动的随机性（0-90，越大越乱）
        [Tooltip("随机模式")]
        public ShakeRandomnessMode randomnessMode; // 随机模式，Harmonic更平滑

        [Header("Tweener setting")]
        [Tooltip("延迟多少秒开始震动")]
        public float Delay; // 延迟多少秒开始震动
        [Tooltip("缓动类型")]
        public Ease EaseType; // 缓动类型
        [Tooltip("循环次数")]
        public int TweenLoopCycle; // 循环次数，-1为无限循环
        [Tooltip("循环类型")]
        public LoopType TweenLoopType; // 循环类型
    }
}