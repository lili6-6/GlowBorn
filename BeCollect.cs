using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace shootstar
{
    public class BeCollect : MonoBehaviour
    {
       

        public enum CollectType
        {
            Null,
            Light,
            Piece
        }
        [SerializeField]public CollectType collectType;

        //[SerializeField] private UnityEvent collectLights;
        //[SerializeField] private UnityEvent collectPieces;

  
        private AudioSource collectedAudio; // 收集音效

        //private float accumulatedRadius = 1f; // 初始光照半径

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (collectType == CollectType.Light) 
            {
                collectedAudio = AudioManager.Instance.collectedLightAudio;
            }
            else if (collectType == CollectType.Piece) 
            {
                collectedAudio = AudioManager.Instance.collectedPieceAudio;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collectType==CollectType.Null)
            {
                return;
            }
            if (collision.CompareTag("Player"))
            {
                if (collectType==CollectType.Light)
                {
                   eventManager.Instance.collectLight?.Invoke();
                    CollectLights();
                }
                else if (collectType==CollectType.Piece)
                {
                    eventManager.Instance.collectPiece?.Invoke(); 
                    CollectPieces();
                }
               
            }
        }

        public void CollectLights()
        {
            if (!this.enabled)
            {
                return;
            }
            if (collectedAudio != null)
            {
                collectedAudio.Play();
            }
            colliderManager.Instance. OnCollectItem(0.5f);
            Debug.Log("收集到光源");
            //shootingstarGameManager.Instance.playerAnimator.SetTrigger("Collect");
            int temp = shootingstarGameManager.Instance.uiManager.cLightNum += 1;
            shootingstarGameManager.Instance.uiManager.UpdateLightText(temp);
            this.gameObject.SetActive(false);
            Destroy(this.gameObject,0.1f);
        }

        public void CollectPieces()
        {
            if (collectedAudio != null)
            {
                collectedAudio.Play();
            }
            StartCoroutine(shootingstarGameManager.Instance.uiManager.PieceText());
            Debug.Log("收集到碎片");
            this.gameObject.SetActive(false);
        }

       
    }
    
}
