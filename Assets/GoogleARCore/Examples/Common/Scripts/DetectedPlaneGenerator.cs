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

    public class DetectedPlaneGenerator : MonoBehaviour
    {
        public static DetectedPlaneGenerator Instance;
        public GameObject DetectedPlanePrefab;

        [SerializeField]
        private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();
        [SerializeField]
        private List<DetectedPlaneVisualizer> m_Visualizers = new List<DetectedPlaneVisualizer>();

        [SerializeField]
        private bool m_RequireSaving;

        private void Awake()
        {
            Instance = this;
        }
        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }
            Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity);
                planeObject.transform.parent = transform;
                planeObject.transform.localPosition = Vector3.zero;

                DetectedPlaneVisualizer visualizer = planeObject.GetComponent<DetectedPlaneVisualizer>();
                DetectedPlaneVisualizer visualizerchild = planeObject.transform.GetChild(0).GetComponent<DetectedPlaneVisualizer>();

                visualizer.Initialize(m_NewPlanes[i]);
                visualizerchild.Initialize(m_NewPlanes[i]);

                m_Visualizers.Add(visualizer);
            }
        }
        public void ToggleVisualizers(bool toggle)
        {
            for (int i = 0; i < m_Visualizers.Count; i++)
            {
                m_Visualizers[i].ToggleMesh(toggle);
            }
        }
        public DetectedPlane GetRandomPlane()
        {
            return m_NewPlanes[0];
        }
    }
}