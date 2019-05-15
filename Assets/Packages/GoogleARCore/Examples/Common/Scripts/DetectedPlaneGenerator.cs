//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGenerator.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using HelloAR;

    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// </summary>
    public class DetectedPlaneGenerator : MonoBehaviour
    {
        public static DetectedPlaneGenerator instance; //RK
        public List<DetectedPlaneVisualizer> PLANES = new List<DetectedPlaneVisualizer>(); //RK
        public List<GameObject> GRIDPLANES = new List<GameObject>(); //RK

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;
        public GameObject DetectedPlaneSavings;

        [SerializeField]
        private Transform m_SavedVisualsParent;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

        [SerializeField]
        private InstantPreviewTrackedPoseDriver m_TrackedPoseDriver;
        /// <summary>
        /// The Unity Update method.
        /// </summary>
        /// 
        void Awake()//RK
        {
            instance = this;
        }

        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }
            //m_TrackedPoseDriver.enabled = ARController.Scanning;
            if (ARController.Scanning)
            {
                // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
                Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
                for (int i = 0; i < m_NewPlanes.Count; i++)
                {
                    // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                    // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                    // coordinates.
                    GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                    GameObject savedObject = Instantiate(DetectedPlaneSavings, Vector3.zero, Quaternion.identity, m_SavedVisualsParent);

                    planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
                    savedObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);

                    GRIDPLANES.Add(planeObject);
                    PLANES.Add(savedObject.GetComponent<DetectedPlaneVisualizer>()); //RK
                }
            }
        }
        public void ToggleVisualizers(bool active)
        {
            for (int i = 0; i < GRIDPLANES.Count; i++)
            {
                if (GRIDPLANES[i] != null)
                {
                    GRIDPLANES[i].SetActive(false);
                }
            }
            for (int j = 0; j < PLANES.Count; j++)
            {
                PLANES[j].EnableCollider();
            }
        }
    }
}