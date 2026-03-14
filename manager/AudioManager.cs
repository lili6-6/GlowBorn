using UnityEngine;


namespace shootstar
{
    /// <summary>
    /// 音频管理器
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        //[Header("爆炸音效")]
        //[SerializeField] public AudioSource ExplosionAudio;
        [Header("收集音效")]
        [SerializeField] public AudioSource collectedLightAudio;
        [SerializeField] public AudioSource collectedPieceAudio;
        [Header("入水")]
        [SerializeField]public AudioSource waterAudio;
        //[Header("藤曼开花")]
        //[SerializeField]public AudioSource flowerAudio;
        //[Header("刺猬恢复")]
        //[SerializeField]public AudioSource hedgehogAudio;
        //[Header("技能")]
        //[SerializeField]public AudioSource redAudio;
        //[SerializeField]public AudioSource blueAudio;
        //[SerializeField]public AudioSource greenAudio;
        //[SerializeField] public AudioSource HurtAudio;
        //[SerializeField] public AudioSource DeathAudio;
        //[SerializeField] public AudioSource ChangeAudio;
        public static AudioManager Instance;


        void Awake()
        {
            Instance = this;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}