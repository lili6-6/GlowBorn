//using UnityEngine;
//using UnityEngine.Audio;

//namespace shootstar
//{


//    public class GlobalAudioManager : MonoBehaviour
//    {
//        [Header("全局混音器")]
//        public AudioMixer audioMixer;

//        [Header("参数名（与Mixer中一致）")]
//        public string exposedParam = "MasterVolume";

//        private static GlobalAudioManager _instance;

//        private void Awake()
//        {

//            DontDestroyOnLoad(gameObject);
//        }

//        private void Start()
//        {
//            // 从存档加载音量（默认80）
//            float saved = PlayerPrefs.GetFloat(exposedParam, 80f);
//            SetMasterVolume(saved);
//        }

//        /// <summary>
//        /// 设置主音量 (Slider传入0~100)
//        /// </summary>
//        //public void SetMasterVolume(float value)
//        //{
//        //    if (value <= 0.0001f)
//        //        value = 0.0001f;

//        //    // 0~100 → 0~1
//        //    float normalized = value / 100f;

//        //    // 调整听感映射曲线（更接近线性）
//        //    float dB = Mathf.Lerp(-50f, 0f, Mathf.Pow(normalized, 0.6f));

//        //    audioMixer.SetFloat(exposedParam, dB);
//        //    PlayerPrefs.SetFloat(exposedParam, value);
//        //}
//        public void SetMasterVolume(float value)
//        {
//            Debug.Log(value);
//            value = Mathf.Clamp(value, 0.0001f, 1f);
//            float dB = Mathf.Log10(value) * 20f;
//            audioMixer.SetFloat(exposedParam, dB);
//            //PlayerPrefs.SetFloat(SettingKeys.Volume, value);
//        }


//    }
//}
using UnityEngine;
using UnityEngine.Audio;

namespace shootstar
{


    public class GlobalAudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public string exposedParam = "MasterVolume";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ApplyVolumeFromPrefs(); // 🔥 提前
        }

     
        public void ApplyVolumeFromPrefs()
        {
            float value = PlayerPrefs.GetFloat(SettingKeys.Volume, 80f);
            SetMasterVolume(value);
        }

        // Slider 传入：0 ~ 100
        public void SetMasterVolume(float value)
        {
            value = Mathf.Clamp(value, 0.0001f, 100f);

            float normalized = value / 100f;
            float dB = Mathf.Lerp(-50f, 0f, Mathf.Pow(normalized, 0.6f));

            audioMixer.SetFloat(exposedParam, dB);
        }
    }
}