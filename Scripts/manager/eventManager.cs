using UnityEngine;
using UnityEngine.Events;

namespace shootstar
{

    public class eventManager : MonoBehaviour
    {
        [Header("交互事件")]
        public UnityEvent pose;
        public UnityEvent continuegame;
        public UnityEvent mushroom;
        //public UnityEvent helpFriends;
        public UnityEvent collectLight;
        public UnityEvent collectPiece;
        public UnityEvent TransIn;
        //public UnityEvent transOut;
        public UnityEvent meetBoss;
        public UnityEvent Floweropen;
        public UnityEvent BeHelped;
        //public UnityEvent meetMushroom;
        //public UnityEvent exitMushroom;
        //[Header("颜色事件")]
        //public UnityEvent WhiteEvent;
        //public UnityEvent RedEvent;
        //public UnityEvent BlueEvent;
        //public UnityEvent GreenEvent;
        //[Header("状态事件")]
        //public UnityEvent OnJump;
        //public UnityEvent OnFall;
        //public UnityEvent OnLand;
        [Header("全局")]
        public UnityEvent OnDie;

        [HideInInspector] public BeHelped beHelped;

        public static eventManager Instance { get; set; } 


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}