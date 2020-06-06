using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CreatorKitCodeInternal
{
    public class InventoryUI : MonoBehaviour
    {
        public class DragData
        {
            public ItemEntryUI DraggedEntry;
            public RectTransform OriginalParent;
        }

        public RectTransform[] ItemSlots;
        public ItemEntryUI ItemEntryPrefab;
        //public ItemTooltip Tooltip;
        //public EquipmentUI EquipementUI;

        public Canvas DragCanvas;

        public DragData CurrentlyDragged { get; set; }
        public CanvasScaler DragCanvasScaler { get; private set; }

        public CharacterData Character
        {
            get { return m_Data; }
        }

        ItemEntryUI[] m_ItemEntries;
        ItemEntryUI m_HoveredItem;
        CharacterData m_Data;

        public void Init()
        {
            CurrentlyDragged = null;

            DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();
            m_ItemEntries = new ItemEntryUI[ItemSlots.Length];

            for (int i = 0; i < m_ItemEntries.Length; ++i)
            {
                m_ItemEntries[i] = Instantiate(ItemEntryPrefab, ItemSlots[i]);
                m_ItemEntries[i].gameObject.SetActive(false);
                m_ItemEntries[i].Owner = this;
                m_ItemEntries[i].InventoryEntry = i;
            }
            //EquipementUI.Init(this);
        }

        void OnEnable()
        {
            m_HoveredItem = null;
            //Tooltip.gameObject.SetActive(false);
        }

        public void Load(CharacterData data)
        {
            m_Data = data;
            //EquipementUI.UpdateEquipment(m_Data.Equipment, m_Data.Stats);

            for (int i = 0; i < m_ItemEntries.Length; ++i)
            {
                m_ItemEntries[i].UpdateEntry();
            }

        }



    }
}
