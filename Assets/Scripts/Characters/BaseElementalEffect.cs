using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using TMPro;
using UnityEngine;

namespace CreatorKitCode
{
    public abstract class BaseElementalEffect : IEquatable<BaseElementalEffect>
    {
        public bool Done => m_Timer <= 0.0f;
        public float CurrentTime => m_Timer;
        public float Duration => m_Duration;
        
        protected float m_Duration;
        protected float m_Timer;
        protected CharacterData m_Target;

        public BaseElementalEffect(float duration)
        {
            m_Duration = duration;
        }

        public virtual void Applied(CharacterData target)
        {
            m_Timer = m_Duration;
            m_Target = target;
        }

        public virtual void Removed()
        {
        
        }

        public virtual void Update(StatSystem statSystem)
        {
            m_Timer -= Time.deltaTime;
        }

        public abstract bool Equals(BaseElementalEffect other);
    }

    public class ElementalEffect : BaseElementalEffect
    {
        int m_Damage;
        StatSystem.DamageType m_DamageType;
        float m_DamageSpeed;
        float m_SinceLastDamage = 0.0f;

        VFXManager.VFXInstance m_FireInstance;

        public ElementalEffect(float duration, StatSystem.DamageType damageType, int damage, float speed = 1.0f) :
           base(duration)
        {
            m_Damage = damage;
            m_DamageType = damageType;
            m_DamageSpeed = speed;
        }

        public override void Update(StatSystem statSystem)
        {
            base.Update(statSystem);

            m_SinceLastDamage += Time.deltaTime;

            if (m_SinceLastDamage > m_DamageSpeed)
            {
                m_SinceLastDamage = 0;

                //Weapon.AttackData data = new Weapon.AttackData(m_Target);

                //data.AddDamage(m_DamageType, m_Damage);

                //statSystem.Damage(data);
            }

            //m_FireInstance.Effect.transform.position = m_Target.transform.position + Vector3.up;
        }

        public override bool Equals(BaseElementalEffect other)
        {
            ElementalEffect eff = other as ElementalEffect;

            if (other == null)
                return false;

            return eff.m_DamageType == m_DamageType;
        }

        public override void Applied(CharacterData target)
        {
            base.Applied(target);

            //m_FireInstance = VFXManager.GetVFX(VFXType.FireEffect);
            //m_FireInstance.Effect.transform.position = target.transform.position + Vector3.up;
        }

        public override void Removed()
        {
            base.Removed();

            //m_FireInstance.Effect.SetActive(false);
        }
    }
}
