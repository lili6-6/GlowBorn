using MoreMountains.CorgiEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace shootstar
{
    public enum _CharacterStates
    {
        Alive,
        Idle,
        Walk,
        Jump,
        Ability,
        Hurt,
        Die
    }

    public class Character_base : MonoBehaviour
    {
        public Character_Animation character_Animation { get; private set; }
        [HideInInspector]public _CharacterStates CurrentState;
        public Health bossHealth;
        private float currentHealth;
        // 受伤冷却（避免连续伤害时动画被刷爆）
        [Header("受伤触发间隔")]
        [SerializeField] private float hurtCooldown = 0.4f;
        private float lastHurtTime = -10f;

        [SerializeField] private AudioSource HurtAudio;
        [SerializeField] private AudioSource DeathAudio;
        [SerializeField] private UnityEvent OnHurt;
        [SerializeField] private UnityEvent OnDie;

        //public string CurrentState { get; set; }

        void Awake()
        {
            character_Animation = GetComponent<Character_Animation>();
            if (character_Animation == null)
            {
                Debug.LogError("Property_Animation component is missing on " + gameObject.name);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            CurrentState= _CharacterStates.Idle;
            character_Animation.ChangeAnimation();
            if(bossHealth==null)
                bossHealth = this.gameObject.GetComponent<Health>();
            currentHealth = bossHealth.CurrentHealth;

            
            bossHealth.OnHit += OnPlayerHurt;
            bossHealth.OnDeath += OnPlayerDeath;
            bossHealth.OnHit += () => Debug.Log("事件成功绑定！");
            bossHealth.OnDeath += () => Debug.Log("死亡事件绑定！");

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            //if(currentHealth<=0)
            //{
            //    CurrentState = "Die";
            //    character_Animation.ChangeAnimation();
            //}

        }
        /// <summary>
        /// 每次玩家受伤时触发（可用于播放受伤动画、闪红、震动等）
        /// </summary>
        private void OnPlayerHurt()
        {
            if (Time.time - lastHurtTime < hurtCooldown)
                return; // 防止连续触发太频繁

            lastHurtTime = Time.time;

            //HurtAudio?.Play();
            Debug.Log("💢 受伤事件触发");
            OnHurt?.Invoke();
            CurrentState = _CharacterStates.Hurt;
            character_Animation.ChangeAnimation();
        }

        /// <summary>
        /// 玩家死亡时触发（血量归零）
        /// </summary>
        private void OnPlayerDeath()
        {
            //DeathAudio?.Play();
            Debug.Log("💀 死亡事件触发");
            OnDie?.Invoke();
            CurrentState = _CharacterStates.Die;
            character_Animation.targetAnimator.SetBool("Alive", false); 
            character_Animation.ChangeAnimation();
        }
    }
}