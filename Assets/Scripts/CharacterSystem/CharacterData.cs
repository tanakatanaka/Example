using System.Collections;
using System.Collections.Generic;
using CreatorKitCodeInternal;
using UnityEngine;

using Random = UnityEngine.Random;

namespace CreatorKitCode
{
    public class CharacterData : HighlightableObject
    {
        public string CharacterName;
        public StatSystem Stats;

        //public Weapon StartingWeapon;
        public InventorySystem Inventory = new InventorySystem();
        //public EquipmentSystem Equipment = new EquipmentSystem();

        public AudioClip[] HitClip;

        //public Action OnDamage { get; set; }

        float m_AttackCoolDown;

        public bool CanAttack
        {
            get { return m_AttackCoolDown <= 0.0f; }
        }

        public void Init()
        {
            Stats.Init(this);
            /*
            Inventory.Init(this);
            Equipment.Init(this);

            if (StartingWeapon != null)
            {
                StartingWeapon.UsedBy(this);
                Equipment.InitWeapon(StartingWeapon, this);
            }
            */
        }

        private void Awake()
        {
            Animator anim = GetComponentInChildren<Animator>();
            if(anim != null)
            {
                //SceneLinkedSMB<CharacterData>.Initialise(anim, this);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            Stats.Tick();
            if (m_AttackCoolDown > 0.0f)
            {
                m_AttackCoolDown -= Time.deltaTime;
            }


        }

    }
}
