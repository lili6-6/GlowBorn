using PixelCrushers;
using UnityEngine;
using UnityEngine.Events;

namespace shootstar
{


    public class TriggerEvent_2D : MonoBehaviour
    {
        [SerializeField] private UnityEvent In;
        [SerializeField] private UnityEvent Out;
        [SerializeField]private LayerMask layerMask;
        [SerializeField]private string targetTag;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if ((layerMask.value & (1 << collision.gameObject.layer)) > 0||collision.tag== targetTag)
            {
                Debug.Log("TriggerEvent_2D:OnTriggerEnter2D");
                In.Invoke();
            }
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if ((layerMask.value & (1 << collision.gameObject.layer)) > 0||collision.tag==targetTag)
            {
                Debug.Log("TriggerEvent_2D:OnTriggerExit2D");
                Out.Invoke();
            }
        }
    }
}