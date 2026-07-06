using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using DG.Tweening;

namespace shootstar
{


    public class CameraTrigger2D : MonoBehaviour
    {
        public CinemachineCamera triggerCamera; // ×¨ÊôÐéÄâÏà»ú
        public int activePriority = 20;
        public bool keepPriorityOnExit = false;
public bool isFollowPlayer=false;
        public UnityEvent In;
        public UnityEvent Out;

        public float targetSize=-1;
        public float Duration;
        public Ease Easetype;


        public CameraManager camManager;

        private void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            In?.Invoke();
            
            if(isFollowPlayer)
            {
                //keepPriorityOnExit = true;
                camManager.FollowPlayer(triggerCamera);
            }
            camManager.TriggerEnter(triggerCamera, activePriority, keepPriorityOnExit);
            if(targetSize > 0)
            changeSize();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            Out?.Invoke();
            camManager.TriggerExit(triggerCamera, keepPriorityOnExit);
            camManager.FollowPlayer(null);
        }
        public void changeSize()
        {
            camManager.CameraSize(Duration, triggerCamera, Easetype, targetSize);
        }
    }



}