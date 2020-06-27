using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatorKitCodeInternal
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public Material BillboardMaterial => m_BillboardMaterial;

#pragma warning disable CS0649
        [SerializeField]
        Material m_BillboardMaterial;
#pragma warning restore CS0649

        void Awake()
        {
            Instance = this;
        }
    }
}