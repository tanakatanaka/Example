﻿using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CreatorKitCode
{
    [System.Serializable]
    public class StatSystem
    {
        public enum DamageType
        {
            Physical,
            Fire,
            Cold,
            Electric
            //ADD YOUR CUSTOM TYPE AFTER
        }

        [System.Serializable]
        public class Stats
        {
            public int health;
            public int strength;
            public int defense;
            public int agility;

            public int[] elementalProtection = new int[Enum.GetValues(typeof(DamageType)).Length];
            public int[] elementalBoosts = new int[Enum.GetValues(typeof(DamageType)).Length];

            public void Copy(Stats other)
            {
                health = other.health;
                strength = other.strength;
                defense = other.defense;
                agility = other.agility;

                Array.Copy(other.elementalProtection, elementalProtection, other.elementalProtection.Length);
                Array.Copy(other.elementalBoosts, elementalBoosts, other.elementalBoosts.Length);
            }


            [System.Serializable]
            public class StatModifier
            {
                public enum Mode
                {
                    Percentage,
                    Absolute
                }

                public Mode ModifierMode = Mode.Absolute;
                public Stats Stats = new Stats();
            }

            public void Modify(StatModifier modifier)
            {
                if (modifier.ModifierMode == StatModifier.Mode.Percentage)
                {
                    health += Mathf.FloorToInt(health * (modifier.Stats.health / 100.0f));
                    strength += Mathf.FloorToInt(strength * (modifier.Stats.strength / 100.0f));
                    defense += Mathf.FloorToInt(defense * (modifier.Stats.defense / 100.0f));
                    agility += Mathf.FloorToInt(agility * (modifier.Stats.agility / 100.0f));


                    for (int i = 0; i < elementalProtection.Length; ++i)
                        elementalProtection[i] += Mathf.FloorToInt(elementalProtection[i] * (modifier.Stats.elementalProtection[i] / 100.0f));

                    for (int i = 0; i < elementalBoosts.Length; ++i)
                        elementalBoosts[i] += Mathf.FloorToInt(elementalBoosts[i] * (modifier.Stats.elementalBoosts[i] / 100.0f));
                }
                else
                {
                    health += modifier.Stats.health;
                    strength += modifier.Stats.strength;
                    defense += modifier.Stats.defense;
                    agility += modifier.Stats.agility;

                    for (int i = 0; i < elementalProtection.Length; ++i)
                        elementalProtection[i] += modifier.Stats.elementalProtection[i];

                    for (int i = 0; i < elementalBoosts.Length; ++i)
                        elementalBoosts[i] += modifier.Stats.elementalBoosts[i];
                }
            }
        }

        public class StatModifier
        {
            public enum Mode
            {
                Percentage,
                Absolute
            }

            public Mode ModifierMode = Mode.Absolute;
            public Stats Stats = new Stats();

        }


        [System.Serializable]
        public class TimedStatModifier
        {
            public string Id;
            public StatModifier Modifier;

            public Sprite EffectSprite;

            public float Duration;
            public float Timer;

            public void Reset()
            {
                Timer = Duration;
            }

            public Stats baseStats;
            public Stats stats { get; set; } = new Stats();

            public int CurrentHealth { get; private set; }
            //public List<BaseElementalEffect> ElementalEffects => m_ElementalEffects;
            public List<TimedStatModifier> TimedModifierStack => m_TimedModifierStack;

            CharacterData m_Owner;
            List<StatModifier> m_ModifiersStack = new List<StatModifier>();
            List<TimedStatModifier> m_TimedModifierStack = new List<TimedStatModifier>();
            List<BaseElementalEffect> m_ElementalEffects = new List<BaseElementalEffect>();
        }


        public Stats baseStats;
        public Stats stats { get; set; } = new Stats();

        public int CurrentHealth { get; private set; }
        public List<BaseElementalEffect> ElementalEffects => m_ElementalEffects;
        public List<TimedStatModifier> TimedModifierStack => m_TimedModifierStack;

        CharacterData m_Owner;

        List<StatModifier> m_ModifiersStack = new List<StatModifier>();
        List<TimedStatModifier> m_TimedModifierStack = new List<TimedStatModifier>();
        List<BaseElementalEffect> m_ElementalEffects = new List<BaseElementalEffect>();

        public void Init(CharacterData owner)
        {
            stats.Copy(baseStats);
            CurrentHealth = stats.health;
            m_Owner = owner;
        }

        public void AddModifier(StatModifier modifier)
        {
            m_ModifiersStack.Add(modifier);
            UpdateFinalStats();
        }

        public void ChangeHealth(int amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, stats.health);
        }

        void UpdateFinalStats()
        {
            bool maxHealthChange = false;
            int previousHealth = stats.health;

            stats.Copy(baseStats);

            foreach (var modifier in m_ModifiersStack)
            {
                if (modifier.Stats.health != 0)
                    maxHealthChange = true;

                //stats.Modify(modifier);
            }

            foreach (var timedModifier in m_TimedModifierStack)
            {
                if (timedModifier.Modifier.Stats.health != 0)
                    maxHealthChange = true;

                //stats.Modify(timedModifier.Modifier);
            }

            //if we change the max health we update the current health to it's new value
            if (maxHealthChange)
            {
                float percentage = CurrentHealth / (float)previousHealth;
                CurrentHealth = Mathf.RoundToInt(percentage * stats.health);
            }

            /// <summary>
            /// Will damage (change negatively health) of the amount of damage stored in the attackData. If the damage are
            /// negative, this heal instead.
            ///
            /// This will also notify the DamageUI so a damage number is displayed.
            /// </summary>
            /// <param name="attackData"></param>
            /*
            public void Damage(Weapon.AttackData attackData)
            {
                int totalDamage = attackData.GetFullDamage();

                ChangeHealth(-totalDamage);
                DamageUI.Instance.NewDamage(totalDamage, m_Owner.transform.position);
            }
            */
        }
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(StatSystem.Stats))]
public class StatsDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int enumTypesCount = Enum.GetValues(typeof(StatSystem.DamageType)).Length;
        int lineCount = enumTypesCount + 7;
        float extraHeight = 6f;
        float propertyHeight = lineCount * EditorGUIUtility.singleLineHeight + extraHeight;

        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        var currentRect = position;
        currentRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.DropShadowLabel(currentRect, property.displayName);

        currentRect.y += EditorGUIUtility.singleLineHeight + 6f;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.health)));

        currentRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.strength)));

        currentRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.defense)));

        currentRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.agility)));

        currentRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(currentRect, "Elemental Protection/Boost", style);

        currentRect.y += EditorGUIUtility.singleLineHeight;
        currentRect.width *= 0.3f;

        currentRect.x += currentRect.width;
        EditorGUI.LabelField(currentRect, "Protection (%)", style);
        currentRect.x += currentRect.width;
        EditorGUI.LabelField(currentRect, "Boost (%)", style);

        var names = Enum.GetNames(typeof(StatSystem.DamageType));

        var elementalProtectionProp = property.FindPropertyRelative(nameof(StatSystem.Stats.elementalProtection));
        var elementalBoostProp = property.FindPropertyRelative(nameof(StatSystem.Stats.elementalBoosts));

        for (int i = 0; i < names.Length; ++i)
        {
            currentRect.x -= currentRect.width * 2;
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(currentRect, names[i]);

            currentRect.x += currentRect.width;
            EditorGUI.PropertyField(currentRect, elementalProtectionProp.GetArrayElementAtIndex(i), GUIContent.none);

            currentRect.x += currentRect.width;
            EditorGUI.PropertyField(currentRect, elementalBoostProp.GetArrayElementAtIndex(i), GUIContent.none);
        }

        EditorGUI.EndProperty();
    }

}

#endif