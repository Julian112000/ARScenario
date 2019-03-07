using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;

public class ARSurfaceManager : MonoBehaviour
{
	[SerializeField] Material m_surfaceMaterial;
	List<DetectedPlane> m_newPlanes = new List<DetectedPlane>();

	void Update()
	{
		Session.GetTrackables(m_newPlanes, TrackableQueryFilter.New);

		foreach (var plane in m_newPlanes)
		{
			GameObject surfaceObj = new GameObject("ARSurface");
            ARSurface arSurface = surfaceObj.AddComponent<ARSurface>();
			//arSurface.SetTrackedPlane(plane, m_surfaceMaterial);
		}
	}
}
