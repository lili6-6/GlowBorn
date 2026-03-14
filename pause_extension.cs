using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

namespace shootstar
{


    public class pause_extension : MonoBehaviour
    {

        private GameObject Player;
        [SerializeField] private string[] targetTag;
        private CharacterPause characterPause;
        [SerializeField] private float pausedTime=1f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (GetTargetTag(collision.tag))
            {
                if (collision.GetComponent<CharacterPause>() != null)
                {
                    characterPause = collision.GetComponent<CharacterPause>();
                    StartCoroutine(Pause());
                }
            }
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if(GetTargetTag(collision.tag))
            {
                if (collision.GetComponent<CharacterPause>() != null)
                {
                    characterPause = collision.GetComponent<CharacterPause>();
                    characterPause.UnPauseCharacter();
                }
            }
        }
        public bool GetTargetTag(string collision)
        {
            foreach (string tag in targetTag)
            {
                if (collision == tag)
                {
                    return true;
                }
            }
            return false;
        }
        public IEnumerator Pause()
        {
            characterPause.PauseCharacter();
            yield return new WaitForSeconds(pausedTime);
            characterPause.UnPauseCharacter();
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}