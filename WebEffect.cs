using UnityEngine;

namespace shootstar
{


    public class WebEffect : MonoBehaviour
    {
        private float duration;
        private System.Action onPlayerEnter;

        public void Init(float duration, System.Action onPlayerEnter)
        {
            this.duration = duration;
            this.onPlayerEnter = onPlayerEnter;
            Destroy(gameObject, duration);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                onPlayerEnter?.Invoke();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                onPlayerEnter?.Invoke();
            }
        }
    }
}