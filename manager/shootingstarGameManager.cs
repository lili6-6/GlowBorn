using System.Collections;
using UnityEngine;
using MoreMountains.CorgiEngine;

namespace shootstar
{
    public class shootingstarGameManager : MonoBehaviour
    {
        [Header("全局管理器")]
        //[SerializeField] public GlobalGameManager globalGameManager;
        [SerializeField] public WebManager webManager;
        [SerializeField]public UIManager uiManager;
        [SerializeField] public LevelSceneManager levelSceneManager;
        [SerializeField]public InputController inputController;
        [SerializeField]public eventManager eventManager;
        [SerializeField]public CameraManager cameraManager;
        [SerializeField]public AudioManager audioManager;
        [SerializeField] public RandomAnimation RandomAnimation;
        [SerializeField] public GameManager CorgiGameManager;
        [SerializeField] public GameObject Player;



        public static shootingstarGameManager Instance; // 单例实例

        
        void Awake()
        {
            // 确保只有一个实例存在
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // 如果已有实例，销毁新的实例
            }

        }
        private void Start()
        {

        }
        private void Update()
        {
        }
      
       


        public void meetInter()
        {
            if(GameManager.Instance.Paused) return;

            //Player.GetComponent<ColorAbility>().meetInter();    



        }
        public void ChangeColor()
        {
            if(GameManager.Instance.Paused) return;

            Player.GetComponent<ChangeColorManager>().ChangeColor();
    
        }

        

        public void AutoStopAudio( )
        {
           StartCoroutine(stopAudio());
      
            //StartCoroutine(stopAudio(duration));
        }
        public IEnumerator stopAudio()
        {
            yield return new WaitForSeconds(5);
            GlobalGameManager.Instance.globalAudioManager.audioMixer.SetFloat(GlobalGameManager.Instance.globalAudioManager.exposedParam, -60f);
            yield return new WaitForSeconds(1);
            GlobalGameManager.Instance.globalAudioManager.audioMixer.SetFloat (GlobalGameManager.Instance.globalAudioManager.exposedParam,-70f);
            yield return new WaitForSeconds(1);
            GlobalGameManager.Instance.globalAudioManager.audioMixer.SetFloat(GlobalGameManager.Instance.globalAudioManager.exposedParam, -80f);
            yield return new WaitForSeconds(1);
        }
    }
}
