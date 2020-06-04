using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace CreatorKitCodeInternal
{
    /// <summary>
    /// Control the camera, mainly used as a reference to the main camera through the singleton instance, and to handle
    /// mouse wheel zooming
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; set; }

        public Camera GameplayCamera;

        /// <summary>
        /// Angle in degree (down compared to horizon) the camera will look at when at the closest of the character
        /// </summary>
        public float MinAngle = 5.0f;
        /// <summary>
        /// Angle in degree (down compared to horizon) the camera will look at when at the farthest of the character
        /// </summary>
        public float MaxAngle = 45.0f;
        /// <summary>
        /// Distance at which the camera is from the character when at the closest zoom level
        /// </summary>
        public float MinDistance = 5.0f;
        /// <summary>
        /// Distance at which the camera is from the character when at the max zoom level
        /// </summary>
        public float MaxDistance = 45.0f;

        public CinemachineVirtualCamera Camera { get; protected set; }

        protected float m_CurrentDistance = 1.0f;
        protected CinemachineFramingTransposer m_FramingTransposer;




    }
}