using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.AI;

namespace CreatorKitCodeInternal
{
    public class SimpleEnemyController : MonoBehaviour,
        AnimationControllerDispatcher.IAttackFrameReceiver,
        AnimationControllerDispatcher.IFootstepFrameReceiver
    {


        public enum State
        {
            IDLE,
            PURSUING,
            ATTACKING
        }

        public float Speed = 6.0f;
        public float detectionRadius = 10.0f;

        public AudioClip[] SpottedAudioClip;

        Vector3 m_StartingAnchor;
        Animator m_Animator;
        NavMeshAgent m_Agent;
        CharacterData m_CharacterData;

        CharacterAudio m_CharacterAudio;

        int m_SpeedAnimHash;
        int m_AttackAnimHash;
        int m_DeathAnimHash;
        int m_HitAnimHash;
        bool m_Pursuing;
        float m_PursuitTimer = 0.0f;

        State m_State;

        LootSpawner m_LootSpawner;

        // Start is called before the first frame update
        void Start()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Agent = GetComponent<NavMeshAgent>();

            m_SpeedAnimHash = Animator.StringToHash("Speed");
            m_AttackAnimHash = Animator.StringToHash("Attack");
            m_DeathAnimHash = Animator.StringToHash("Death");
            m_HitAnimHash = Animator.StringToHash("Hit");

            m_CharacterData = GetComponent<CharacterData>();
            m_CharacterData.Init();

            m_CharacterAudio = GetComponentInChildren<CharacterAudio>();

            m_CharacterData.OnDamage += () =>
            {
                m_Animator.SetTrigger(m_HitAnimHash);
                m_CharacterAudio.Hit(transform.position);
            };

            m_Agent.speed = Speed;
            m_LootSpawner = GetComponent<LootSpawner>();
            m_StartingAnchor = transform.position;

            // Update is called once per frame
            void Update()
            {
                //See the Update function of CharacterControl.cs for a comment on how we could replace
                //this (polling health) to a callback method.
                if (m_CharacterData.Stats.CurrentHealth == 0)
                {
                    m_Animator.SetTrigger(m_DeathAnimHash);

                    m_CharacterAudio.Death(transform.position);
                    m_CharacterData.Death();

                    if (m_LootSpawner != null)
                    {
                        m_LootSpawner.SpawnLoot();
                    }

                    Destroy(m_Agent);
                    Destroy(GetComponent<Collider>());
                    Destroy(this);
                    return;
                }

                Vector3 playerPosition = CharacterControl.Instance.transform.position;
                CharacterData playerData = CharacterControl.Instance.Data;

                switch (m_State)
                {
                    case State.IDLE:
                    {
                        if (Vector3.SqrMagnitude(playerPosition - transform.position) < detectionRadius * detectionRadius)
                        {

                        }
                    }
                        break;

                    case State.PURSUING:
                    {

                    }
                        break;

                    case State.ATTACKING:
                    {

                    }
                        break;
                    
                }
            }
        }
    }
}