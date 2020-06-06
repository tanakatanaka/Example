using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace CreatorKitCodeInternal
{
    public class CharacterControl : MonoBehaviour
         //AnimationControllerDispatcher.IAttackFrameReceiver,
        //AnimationControllerDispatcher.IFootstepFrameReceiver
    {
        public static CharacterControl Instance { get; protected set; }
        public float Speed = 10.0f;
        public CharacterData Data => m_CharacterData;
        public CharacterData CurrentTarget => m_CurrentTargetCharacterData;

        public Transform WeaponLocator;

        [Header("Audio")]
        public AudioClip[] SpurSoundClips;
        Vector3 m_LastRaycastResult;
        Animator m_Animator;
        NavMeshAgent m_Agent;
        CharacterData m_CharacterData;

        HighlightableObject m_Highlighted;

        RaycastHit[] m_RaycastHitCache = new RaycastHit[16];

        int m_SpeedParamID;
        int m_AttackParamID;
        int m_HitParamID;
        int m_FaintParamID;
        int m_RespawnParamID;

        bool m_IsKO = false;
        float m_KOTimer = 0.0f;

        int m_InteractableLayer;
        int m_LevelLayer;
        Collider m_TargetCollider;

        InteractableObject m_TargetInteractable = null;
        Camera m_MainCamera;

        NavMeshPath m_CalculatedPath;

        CharacterAudio m_CharacterAudio;

        int m_TargetLayer;
        CharacterData m_CurrentTargetCharacterData = null;
        //this is a flag that tell the controller it need to clear the target once the attack finished.
        //usefull for when clicking elwswhere mid attack animation, allow to finish the attack and then exit.
        bool m_ClearPostAttack = false;

        SpawnPoint m_CurrentSpawn = null;

        enum State
        {
            DEFAULT,
            HIT,
            ATTACKING
        }

        State m_CurrentState;

        void Awake()
        {
            Instance = this;
            m_MainCamera = Camera.main;
        }

        // Start is called before the first frame update
        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            m_CalculatedPath = new NavMeshPath();
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponentInChildren<Animator>();

            m_Agent.speed = Speed;
            m_Agent.angularSpeed = 360.0f;

            m_LastRaycastResult = transform.position;

            m_SpeedParamID = Animator.StringToHash("Speed");
            m_AttackParamID = Animator.StringToHash("Attack");
            m_HitParamID = Animator.StringToHash("Hit");
            m_FaintParamID = Animator.StringToHash("Faint");
            m_RespawnParamID = Animator.StringToHash("Respawn");

            m_CharacterData = GetComponent<CharacterData>();

            m_CharacterData.Equipment.OnEquiped += item =>
            {
                if (item.Slot == (EquipmentItem.EquipmentSlot)666)
                {
                    var obj = Instantiate(item.WorldObjectPrefab, WeaponLocator, false);
                    //Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
                }
            };

            m_CharacterData.Equipment.OnUnequip += item =>
            {
                if (item.Slot == (EquipmentItem.EquipmentSlot)666)
                {
                    foreach (Transform t in WeaponLocator)
                    {
                        Destroy(t.gameObject);
                    }
                }
            };

            m_CharacterData.Init();

            m_InteractableLayer = 1 << LayerMask.NameToLayer("Interactable");
            m_LevelLayer = 1 << LayerMask.NameToLayer("Level");
            m_TargetLayer = 1 << LayerMask.NameToLayer("Target");

            m_CurrentState = State.DEFAULT;

            //m_CharacterAudio = GetComponent<CharacterAudio>();

            m_CharacterData.OnDamage += () =>
            {
                m_Animator.SetTrigger(m_HitParamID);
                //m_CharacterAudio.Hit(transform.position);
            };
        }
        // Update is called once per frame

        void Update()
        {
            Vector3 pos = transform.position;

            if (m_IsKO)
            {
                m_KOTimer += Time.deltaTime;
                if (m_KOTimer > 3.0f)
                {
                    //GoToRespawn();
                }
                return;
            }
            //The update need to run, so we can check the health here.
            //Another method would be to add a callback in the CharacterData that get called
            //when health reach 0, and this class register to the callback in Start
            //(see CharacterData.OnDamage for an example)
            if (m_CharacterData.Stats.CurrentHealth == 0)
            {
                m_Animator.SetTrigger(m_FaintParamID);

                m_Agent.isStopped = true;
                m_Agent.ResetPath();
                m_IsKO = true;
                m_KOTimer = 0.0f;

                Data.Death();

                m_CharacterAudio.Death(pos);
                return;
            }

            Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);

            if (m_TargetInteractable != null)
            {
                CheckInteractableRange();
            }

            if (m_CurrentTargetCharacterData != null)
            {
                if (m_CurrentTargetCharacterData.Stats.CurrentHealth == 0)
                {
                    m_CurrentTargetCharacterData = null;
                }
                else
                {
                    CheckAttack();
                }
            }

            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            if (!Mathf.Approximately(mouseWheel, 0.0f))
            {
                Vector3 view = m_MainCamera.ScreenToViewportPoint(Input.mousePosition);
                if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
                {
                    CameraController.Instance.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
                }
            }

            if (Input.GetMouseButtonDown(0))
            { //if we click the mouse button, we clear any previously et targets

                if (m_CurrentState != State.ATTACKING)
                {
                    m_CurrentTargetCharacterData = null;
                    m_TargetInteractable = null;
                }
                else
                {
                    m_ClearPostAttack = true;
                }
            }

            if (!EventSystem.current.IsPointerOverGameObject() && m_CurrentState != State.ATTACKING)
            {
                //Raycast to find object currently under the mouse cursor
                ObjectsRaycasts(screenRay);

                if (Input.GetMouseButton(0))
                {
                    if (m_TargetInteractable == null && m_CurrentTargetCharacterData == null)
                    {
                        InteractableObject obj = m_Highlighted as InteractableObject;
                        if (obj)
                        {
                            InteractWith(obj);
                        }
                        else
                        {
                            CharacterData data = m_Highlighted as CharacterData;
                            if (data != null)
                            {
                                m_CurrentTargetCharacterData = data;
                            }
                            else
                            {
                                MoveCheck(screenRay);
                            }
                        }
                    }
                }

                m_Animator.SetFloat(m_SpeedParamID, m_Agent.velocity.magnitude / m_Agent.speed);

                if (Input.GetKeyUp(KeyCode.I))
                {
                    //UISystem.Instance.ToggleInventory();
                }
            }
        }

        public void InteractWith(InteractableObject obj)
        {

        }


        void CheckInteractableRange()
        {

        }

        void CheckAttack()
        {


        }

        void ObjectsRaycasts(Ray screenRay)
        {


        }

        void MoveCheck(Ray screenRay)
        {

        }
    }
}

