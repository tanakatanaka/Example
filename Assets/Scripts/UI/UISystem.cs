using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

namespace CreatorKitCodeInternal
{
    public class UISystem : MonoBehaviour
    {
        public static UISystem Instance { get; private set; }

        [Header("Player")]
        public CharacterControl PlayerCharacter;
        public Slider PlayerHealthSlider;
        public Text MaxHealth;
        public Text CurrentHealth;
        //public EffectIconUI[] TimedModifierIcones;
        public Text StatsText;

        [Header("Enemy")]
        public Slider EnemyHealthSlider;
        public Text EnemyName;
        //public EffectIconUI[] EnemyEffectIcones;

        [Header("Inventory")]
        //public InventoryUI InventoryWindow;
        public Button OpenInventoryButton;
        public AudioClip OpenInventoryClip;
        public AudioClip CloseInventoryClip;

        Sprite m_ClosedInventorySprite;
        Sprite m_OpenInventorySprite;

        void Awake()
        {
            Instance = this;

            //InventoryWindow.Init();
        }
    }
}
