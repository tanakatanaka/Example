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


    }
}
