using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace shootstar
{


    public class UIManager : MonoBehaviour
    {
        [Header("UI习惯")]
        [SerializeField] public ButtonManager SetBut;
        [SerializeField] public ButtonManager interBut;
        [SerializeField] public ButtonManager ChangeBut;
        [SerializeField] public TextMeshProUGUI cLightNumText; // 用于显示收集到的光源数量的文本组件
        [SerializeField] public TextMeshProUGUI cPieceNumText; // 用于显示收集到的碎片数量的文本组件
        [SerializeField] public TextMeshProUGUI cFriends; // 用于显示当前帮助的小伙伴数量的文本组件
        [SerializeField] public Image PlayerImage;
        [SerializeField] public RectTransform healthBar;
        [SerializeField] public RectTransform MoveButs;
        [SerializeField] public ButtonManager JumpBut;
        [HideInInspector] public int cLightNum = 0; // 当前收集到的光源数量
        [HideInInspector] public int cFriendsNum = 0; // 当前帮助的小伙伴数量

        [Header("其他及ui")]
        public Slider durationSlider;
        public Slider chargeSlider;
        public Canvas uiCanvas;

        private GameObject[] habbitchange;
        private string originFriends;
        private string originLights;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            interBut.Interactable(false);
            habbitchange = new GameObject[10];
        }
        void Start()
        {
            savehabbitUI();
           
            Playerhabbit();
            originFriends = cFriends.text;
            originLights =cLightNumText.text;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void UpdateLightText(int num)
        {
            // Debug.Log("更新光源数量文本"+num);
            if (num < 0) num = 0;
            if(cLightNum<0) cLightNum = 0;
           
            cLightNumText.text = originLights + num.ToString();
        }
        public void UpdateFriendsText(int num)
        {
            if (num < 0) num = 0;
           
            cFriends.text = originFriends + num.ToString();
        }
        public IEnumerator PieceText()
        {
            cPieceNumText.gameObject.SetActive(true); // 显示文本
            cPieceNumText.text = "碎片+1";
            yield return new WaitForSeconds(2f); // 等待1秒
            cPieceNumText.gameObject.SetActive(false); // 隐藏文本
        }

        private void Playerhabbit()
        {
            if (GlobalGameManager.Instance.habbitchange)
            {
                foreach (var ui in habbitchange)
                {
                    RectTransform rt = ui.GetComponent<RectTransform>();
                    Vector3 pos = rt.localPosition;   // 使用 localPosition 更安全
                    pos.x *= -1;
                    rt.localPosition = pos;
                }
            }
        }
        private void savehabbitUI()
        {
            habbitchange[0] = SetBut.gameObject;
            habbitchange[1] = interBut.gameObject;
            habbitchange[2] = ChangeBut.gameObject;
            habbitchange[3] = cLightNumText.gameObject;
            habbitchange[4] = cPieceNumText.gameObject;
            habbitchange[5] = cFriends.gameObject;
            habbitchange[6] = PlayerImage.gameObject;
            habbitchange[7] = healthBar.gameObject;
            habbitchange[8] = MoveButs.gameObject;
            habbitchange[9] = JumpBut.gameObject;
        }
    }
}